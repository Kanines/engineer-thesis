using System.Collections;
using System.Collections.Generic;

namespace LASLibrary
{
    public class LasPoint
    {
        public enum ClassificationType
        {
            Created,
            Unclassified,
            Ground,
            LowVegetation,
            MediumVegetation,
            HighVegetation,
            Building,
            Noise,
            MassPoint,
            Water,
            OverlapPoints = 12,
            Other
        }

        protected byte _classification;
        protected ushort _intensity;
        protected ushort _pointSourceId;
        protected int _x;
        protected int _y;
        protected int _z;

        public LasPoint(ClassificationType classification, ushort intensity, ushort pointSourceId,
            int x, int y, int z)
        {
            _classification = (byte) classification;
            _intensity = intensity;
            _pointSourceId = pointSourceId;
            _x = x;
            _y = y;
            _z = z;
        }

        protected LasPoint(byte[] rawBytes, int startPosition)
        {
            var position = startPosition;

            //_x = BitConverter.ToInt32(rawBytes, position);
            _x = MyBitConverter.ToInt32(rawBytes, position);
            position += sizeof(int);

            //_y = BitConverter.ToInt32(rawBytes, position);
            _y = MyBitConverter.ToInt32(rawBytes, position);
            position += sizeof(int);

            //_z = BitConverter.ToInt32(rawBytes, position);
            _z = MyBitConverter.ToInt32(rawBytes, position);
            position += sizeof(int);

            _intensity = (ushort) MyBitConverter.ToInt16(rawBytes, position);
            position += sizeof(ushort) + 1;

            _classification = (byte) (rawBytes[position] & 0x1F);
            position += 3;

            _pointSourceId = (ushort) MyBitConverter.ToInt16(rawBytes, position);
        }

        public ushort Intensity
        {
            get { return _intensity; }
        }

        public ushort PointSourceId
        {
            get { return _pointSourceId; }
        }

        public ClassificationType Classification
        {
            get
            {
                if (_classification > 9 && _classification != 12) return ClassificationType.Other;
                return (ClassificationType) _classification;
            }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int Z
        {
            get { return _z; }
        }

        public static LasPoint AverageOfPointsList(List<LasPoint> list)
        {
            if (list == null || list.Count == 0) return null;

            int intensity = 0;
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < list.Count; i++)
            {
                LasPoint point = list[i];
                x += point.X;
                y += point.Y;
                z += point.Z;
                intensity += point.Intensity;
            }
            x /= list.Count;
            y /= list.Count;
            z /= list.Count;
            intensity /= list.Count;
            return new LasPoint(((LasPoint) list[0]).Classification, (ushort) intensity,
                ((LasPoint) list[0]).PointSourceId, x, y, z);
        }
    }
}