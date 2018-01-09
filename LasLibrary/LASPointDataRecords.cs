using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LASLibrary
{
    public enum PointsFormat
    {
        Format0,
        Format1,
        Format2,
        Format3
    }

    public class LasPointDataRecords : ICollection<LasPoint>
    {
        readonly bool _shortFormat;
        readonly List<LasPoint> _points;
        readonly List<int>[,] _mapPoints;
        readonly Dictionary<LasPoint.ClassificationType, List<int>> _classificationMap;
        readonly LasHeader _header;
        readonly int _columns;
        readonly int _rows;
        readonly double _stepX;
        readonly double _stepY;
        ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public LasPointDataRecords(LasHeader header, int cols, bool useShortFormat)
        {
            double lengthY = header.MaxY - header.MinY;
            double lengthX = header.MaxX - header.MinX;
            double ratio = lengthY / lengthX;
            _columns = cols;
            _rows = (int)Math.Ceiling(ratio * cols);
            _mapPoints = new List<int>[_rows, cols];
            _classificationMap = new Dictionary<LasPoint.ClassificationType, List<int>>();
            _stepX = lengthX / _columns;
            _stepY = lengthY / _rows;

            _points = new List<LasPoint>((int)header.NumberOfPointRecords);
            _shortFormat = useShortFormat;
            _header = header;
            Format = (PointsFormat)header.PointDataFormatId;

            ReadPoints();
        }

        public LasPoint this[int index]
        {
            get
            {
                if (index >= _header.NumberOfPointRecords) return null;
                return _points[index];
            }
        }

        public LasPoint SynchGet(int index)
        {
            try
            {
                locker.EnterReadLock();
                return this[index];
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public Tuple<int, int> GetSectorPosition(LasPoint point)
        {
            double stepToMinX = _header.ScaleX(point.X) - _header.MinX;
            double stepToMaxY = _header.MaxY - _header.ScaleY(point.Y);

            int x = (int)Math.Floor(stepToMinX / _stepX);
            int y = (int)Math.Floor(stepToMaxY / _stepY);

            if (x == _columns)
                x--;
            if (y == _rows)
                y--;

            return Tuple.Create(x, y);
        }

        public List<int> GetSector(LasPoint point)
        {
            var positionInArray = GetSectorPosition(point);
            return GetSector(positionInArray.Item1, positionInArray.Item2);
        }

        public List<int> GetSector(int x, int y)
        {
            if (x >= _columns || x < 0 || y >= _rows || y < 0) return null;

            return _mapPoints[y, x];
        }

        public List<Tuple<int, int>> GetNeighboursSector(LasPoint point, int radius = 1, bool onlyBorder = false)
        {
            var positionInArray = GetSectorPosition(point);
            return GetNeighboursSectors(positionInArray.Item1, positionInArray.Item2, radius, onlyBorder);
        }

        public List<Tuple<int, int>> GetNeighboursSectors(int x, int y, int radius = 1, bool onlyBorder = false)
        {
            if (x >= _columns || x < 0 || y >= _rows || y < 0) return null;

            List<Tuple<int, int>> list = new List<Tuple<int, int>>(9);
            for (int xPos = x - radius; xPos <= x + radius; xPos++)
            {
                for (int yPos = y - radius; yPos <= y + radius; yPos++)
                {
                    if (IsValidSectorPosition(xPos, yPos) &&
                        (!onlyBorder || (RadiusSectorsPosition(x, y, xPos, yPos) == radius)))
                        list.Add(new Tuple<int, int>(xPos, yPos));
                }
            }
            return list;
        }

        private int RadiusSectorsPosition(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
                return 0;
            int x = Math.Max(x1, x2) - Math.Min(x1, x2);
            int y = Math.Max(y1, y2) - Math.Min(y1, y2);

            return Math.Max(x, y);
        }

        private bool IsValidSectorPosition(int xPos, int yPos)
        {
            return xPos >= 0 && xPos < _columns && yPos >= 0 && yPos < _rows && GetSector(xPos, yPos) != null;
        }

        public List<List<int>> GetNeighboursSectorsIndex(List<Tuple<int, int>> sectors)
        {
            List<List<int>> sectorIndex = new List<List<int>>(sectors.Count);
            foreach (var item in sectors)
            {
                sectorIndex.Add(GetSector(item.Item1, item.Item2));
            }

            return sectorIndex;
        }

        public List<LasPoint> GetNeighbours(LasPoint point, int count = 25, int radiusSectors = 1, bool getAllFromRadiusSectors = false)
        {
            var sectorIndex = GetNeighboursSectorsIndex(GetNeighboursSector(point, radiusSectors));
            Func<LasPoint, double> distance = (p) => (Math.Sqrt(Math.Pow(point.X - p.X, 2) + Math.Pow(point.Y - p.Y, 2)));

            var listOfNeigh = sectorIndex.SelectMany(x => x).Select(el => this[el]).ToList();
            while (count > 0 && listOfNeigh.Count < count)
            {
                sectorIndex = GetNeighboursSectorsIndex(GetNeighboursSector(point, ++radiusSectors, true));
                listOfNeigh.AddRange(sectorIndex.SelectMany(x => x).Select(el => this[el]));
            }
            listOfNeigh.Remove(point);

            return getAllFromRadiusSectors ? listOfNeigh.OrderBy(c => distance(c)).ToList() : listOfNeigh.OrderBy(c => distance(c)).Take(count).ToList();
        }

        private void AddPoint(LasPoint point, int index)
        {
            if (point != null)
            {
                var sector = GetSector(point);
                if (sector == null)
                {
                    var pos = GetSectorPosition(point);
                    _mapPoints[pos.Item2, pos.Item1] = new List<int>();
                    sector = _mapPoints[pos.Item2, pos.Item1];
                }
                List<int> classificationMapValue;
                _classificationMap.TryGetValue(point.Classification, out classificationMapValue);
                if (classificationMapValue == null)
                    _classificationMap[point.Classification] = new List<int>();

                _classificationMap[point.Classification].Add(index);
                _points.Add(point);
                sector.Add(index);
            }
        }

        void ReadPoints()
        {
            byte[] by;

            int fileFrameSize = (int)(_header.NumberOfPointRecords * _header.PointDataRecordLength);
            using (var binaryReader = new BinaryReader(File.Open(_header.LasFile.Filename, FileMode.Open)))
            {
                binaryReader.BaseStream.Seek(_header.OffsetToPointData, SeekOrigin.Begin);
                by = binaryReader.ReadBytes(fileFrameSize);
            }

            var numberOfPoint = by.Length / _header.PointDataRecordLength;
            LasPoint point;
            switch (Format)
            {
                case PointsFormat.Format0:
                    for (var i = 0; i < numberOfPoint; i++)
                    {
                        if (_shortFormat)
                            point =
                                new LasPoint0Short(by, i * _header.PointDataRecordLength);
                        else
                            point =
                                new LasPoint0Long(by, i * _header.PointDataRecordLength);
                        AddPoint(point, i);
                    }
                    break;
                case PointsFormat.Format1:
                    for (var i = 0; i < numberOfPoint; i++)
                    {
                        if (_shortFormat)
                            point =
                                new LasPoint1Short(by, i * _header.PointDataRecordLength);
                        else
                            point =
                                new LasPoint1Long(by, i * _header.PointDataRecordLength);
                        AddPoint(point, i);
                    }
                    break;
                case PointsFormat.Format2:
                    for (var i = 0; i < numberOfPoint; i++)
                    {
                        if (_shortFormat)
                            point =
                                new LasPoint2Short(by, i * _header.PointDataRecordLength);
                        else
                            point =
                                new LasPoint2Long(by, i * _header.PointDataRecordLength);
                        AddPoint(point, i);
                    }
                    break;
                case PointsFormat.Format3:
                    for (var i = 0; i < numberOfPoint; i++)
                    {
                        if (_shortFormat)
                            point =
                                new LasPoint3Short(by, i * _header.PointDataRecordLength);
                        else
                            point =
                                new LasPoint3Long(by, i * _header.PointDataRecordLength);
                        AddPoint(point, i);
                    }
                    break;
            }
        }

        public List<LasPoint> GetPointsByClassification(LasPoint.ClassificationType classType)
        {
            return _classificationMap[classType].Select(el => this[el]).ToList();
        }

        public List<LasPoint> GetPointsByClassification(LasPoint.ClassificationType classType, double frac)
        {
            var random = new Random();
            List<int> points;
            if (!_classificationMap.TryGetValue(classType, out points))
                points = new List<int>();
            int count = (int)Math.Floor(points.Count * frac);
            return Enumerable.Range(0, count).Select(i => points[random.Next(points.Count-1)]).Select(el => this[el]).ToList();
        }

        public List<LasPoint> GetRandomPointsByClass(IEnumerable<LasPoint.ClassificationType> listClass, double frac)
        {
            List<LasPoint> toReturn = new List<LasPoint>((int)Math.Floor(Count * frac) / 4);
            foreach (var item in listClass)
            {
                toReturn.AddRange(GetPointsByClassification(item, frac));
            }
            return toReturn;
        }

        public PointsFormat Format { get; set; }

        public List<LasPoint> Points
        {
            get { return _points; }
        }

        public int Columns
        {
            get { return _columns; }
        }

        public int Rows
        {
            get { return _rows; }
        }

        public int Count
        {
            get { return (int)_header.NumberOfPointRecords; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(LasPoint item)
        {
            _points.Add(item);
        }

        public void Clear()
        {
            _points.Clear();
        }

        public bool Contains(LasPoint item)
        {
            return _points.Contains(item);
        }

        public void CopyTo(LasPoint[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }

        public bool Remove(LasPoint item)
        {
            return _points.Remove(item);
        }

        public IEnumerator<LasPoint> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}