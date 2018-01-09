using System;
using System.IO;

namespace LASLibrary
{
    public class LasHeader
    {
        string _fileSignature = string.Empty;
        ushort _fileSourceId;
        ushort _globalEncoding;
        uint _guiDdata1;
        ushort _guiDdata2;
        ushort _guiDdata3;
        readonly byte[] _guiDdata4 = new byte[8];
        byte _versionMajor;
        byte _versionMinor;
        string _systemIdentifier = string.Empty;
        string _generatingSoftware = string.Empty;
        ushort _fileCreationDayOfYear;
        ushort _fileCreationYear;
        ushort _headerSize;
        uint _offsetToPointData;
        uint _numberOfVariableLengthRecords;
        byte _pointDataFormatId;
        ushort _pointDataRecordLength;
        uint _numberOfPointRecords;
        readonly uint[] _numberOfPointsByReturn = new uint[5];
        double _scaleFactorX;
        double _scaleFactorY;
        double _scaleFactorZ;
        double _offsetX;
        double _offsetY;
        double _offsetZ;
        double _maxX;
        double _minX;
        double _maxY;
        double _minY;
        double _maxZ;
        double _minZ;

        readonly LasFile _lasFile;

        public static int SIZE = 227;

        public LasHeader()
        {
        }

        public LasHeader(string filename, LasFile lasFile)
        {
            byte[] by;
            _lasFile = lasFile;
            using (var binaryReader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                by = binaryReader.ReadBytes(SIZE);
            }
            Converter(by);
        }

        void Converter(byte[] rawBytes)
        {
            var position = 0;
            for (var i = 0; i < 4; i++)
                _fileSignature += (char) rawBytes[position + i];
            position += 4;

            if (_fileSignature != "LASF")
                throw new LasException(string.Format("File {0} isn't right LAS file!", _lasFile.Filename),
                    LasError.LasFatalError);

            _fileSourceId = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _globalEncoding = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _guiDdata1 = BitConverter.ToUInt32(rawBytes, position);
            position += sizeof(uint);

            _guiDdata2 = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _guiDdata3 = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            for (var i = 0; i < _guiDdata4.Length; i++)
                _guiDdata4[i] = rawBytes[position + i];
            position += _guiDdata4.Length;

            _versionMajor = rawBytes[position];
            position += 1;

            _versionMinor = rawBytes[position];
            position += 1;

            for (var i = 0; i < 32; i++)
            {
                if (rawBytes[position + i] == 0) break;
                _systemIdentifier += (char) rawBytes[position + i];
            }
            position += 32;

            for (var i = 0; i < 32; i++)
            {
                if (rawBytes[position + i] == 0) break;
                _generatingSoftware += (char) rawBytes[position + i];
            }
            position += 32;

            _fileCreationDayOfYear = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _fileCreationYear = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _headerSize = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _offsetToPointData = BitConverter.ToUInt32(rawBytes, position);
            position += sizeof(uint);

            _numberOfVariableLengthRecords = BitConverter.ToUInt32(rawBytes, position);
            position += sizeof(uint);

            _pointDataFormatId = rawBytes[position];
            position += 1;

            _pointDataRecordLength = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _numberOfPointRecords = BitConverter.ToUInt32(rawBytes, position);
            position += sizeof(uint);

            for (var i = 0; i < _numberOfPointsByReturn.Length; i++)
                _numberOfPointsByReturn[i] = BitConverter.ToUInt32(rawBytes, position + i*4);
            position += _numberOfPointsByReturn.Length*sizeof(uint);

            _scaleFactorX = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _scaleFactorY = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _scaleFactorZ = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _offsetX = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _offsetY = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _offsetZ = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _maxX = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _minX = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _maxY = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _minY = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _maxZ = BitConverter.ToDouble(rawBytes, position);
            position += sizeof(double);

            _minZ = BitConverter.ToDouble(rawBytes, position);
        }

        public double ScaleX(double x)
        {
            return x*_scaleFactorX + _offsetX;
        }

        public double ScaleY(double y)
        {
            return y*_scaleFactorY + _offsetY;
        }

        public double ScaleZ(double z)
        {
            return z*_scaleFactorZ + _offsetZ;
        }

        public string FileSignature
        {
            get { return _fileSignature; }
        }

        public ushort FileSourceId
        {
            get { return _fileSourceId; }
        }

        public ushort GlobalEncoding
        {
            get { return _globalEncoding; }
        }

        public uint GuiDdata1
        {
            get { return _guiDdata1; }
        }

        public ushort GuiDdata2
        {
            get { return _guiDdata2; }
        }

        public ushort GuiDdata3
        {
            get { return _guiDdata3; }
        }

        public byte[] GuiDdata4
        {
            get { return _guiDdata4; }
        }

        public byte VersionMajor
        {
            get { return _versionMajor; }
        }

        public byte VersionMinor
        {
            get { return _versionMinor; }
        }

        public string SystemIdentifier
        {
            get { return _systemIdentifier; }
        }

        public string GeneratingSoftware
        {
            get { return _generatingSoftware; }
        }

        public ushort FileCreationDayOfYear
        {
            get { return _fileCreationDayOfYear; }
        }

        public ushort FileCreationYear
        {
            get { return _fileCreationYear; }
        }

        public ushort HeaderSize
        {
            get { return _headerSize; }
        }

        public uint OffsetToPointData
        {
            get { return _offsetToPointData; }
        }

        public uint NumberOfVariableLengthRecords
        {
            get { return _numberOfVariableLengthRecords; }
        }

        public byte PointDataFormatId
        {
            get { return _pointDataFormatId; }
        }

        public ushort PointDataRecordLength
        {
            get { return _pointDataRecordLength; }
        }

        public uint NumberOfPointRecords
        {
            get { return _numberOfPointRecords; }
        }

        public uint[] NumberOfPointsByReturn
        {
            get { return _numberOfPointsByReturn; }
        }

        public double ScaleFactorX
        {
            get { return _scaleFactorX; }
        }

        public double ScaleFactorY
        {
            get { return _scaleFactorY; }
        }

        public double ScaleFactorZ
        {
            get { return _scaleFactorZ; }
        }

        public double OffsetX
        {
            get { return _offsetX; }
        }

        public double OffsetY
        {
            get { return _offsetY; }
        }

        public double OffsetZ
        {
            get { return _offsetZ; }
        }

        public double MaxX
        {
            get { return _maxX; }
        }

        public double MinX
        {
            get { return _minX; }
        }

        public double MaxY
        {
            get { return _maxY; }
        }

        public double MinY
        {
            get { return _minY; }
        }

        public double MaxZ
        {
            get { return _maxZ; }
        }

        public double MinZ
        {
            get { return _minZ; }
        }

        public LasFile LasFile
        {
            get { return _lasFile; }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "FileSignature: {0}, FileSourceId: {1}, GlobalEncoding: {2}, VersionMajor: {3}, VersionMinor: {4}, SystemIdentifier: {5}, GeneratingSoftware: {6}, FileCreationDayOfYear: {7}, FileCreationYear: {8}, HeaderSize: {9}, OffsetToPointData: {10}, NumberOfVariableLengthRecords: {11}, PointDataFormatId: {12}, PointDataRecordLength: {13}, NumberOfPointRecords: {14}, NumberOfPointsByReturn: {15}, ScaleFactorX: {16}, ScaleFactorY: {17}, ScaleFactorZ: {18}, OffsetX: {19}, OffsetY: {20}, OffsetZ: {21}, MaxX: {22}, MinX: {23}, MaxY: {24}, MinY: {25}, MaxZ: {26}, MinZ: {27}",
                    FileSignature, FileSourceId, GlobalEncoding, VersionMajor, VersionMinor, SystemIdentifier,
                    GeneratingSoftware, FileCreationDayOfYear, FileCreationYear, HeaderSize, OffsetToPointData,
                    NumberOfVariableLengthRecords, PointDataFormatId, PointDataRecordLength, NumberOfPointRecords,
                    NumberOfPointsByReturn, ScaleFactorX, ScaleFactorY, ScaleFactorZ, OffsetX, OffsetY, OffsetZ, MaxX,
                    MinX, MaxY, MinY, MaxZ, MinZ);
        }
    }
}