using System.Collections;

namespace LASLibrary
{
    public struct BitsField
    {
        readonly BitArray _bitsArray;
        public BitArray ReturnNumber;
        public BitArray NumberOfReturns;
        public bool ScanDirectionFlag;
        public bool EdgeOfFlightLine;

        public BitsField(byte by)
        {
            _bitsArray = new BitArray(new[] {by});
            ReturnNumber = Slice(_bitsArray, 0, 3);
            NumberOfReturns = Slice(_bitsArray, 3, 6);
            ScanDirectionFlag = _bitsArray[6];
            EdgeOfFlightLine = _bitsArray[7];
        }

        public override string ToString()
        {
            return BitsToString(_bitsArray);
        }

        static string BitsToString(BitArray bits)
        {
            var returnString = string.Empty;
            for (var i = 0; i < bits.Length; i++)
                returnString += bits[i] ? '1' : '0';
            return returnString;
        }

        static BitArray Slice(BitArray source, int start, int end)
        {
            if (end < 0)
                end = source.Length + end;

            int len = end - start;

            var res = new BitArray(len);
            for (int i = 0; i < len; i++)
                res[i] = source[i + start];

            return res;
        }
    }

    public abstract class LasPointLong : LasPoint
    {
        protected BitsField _bitsField;
        protected bool _classificationSynthetic;
        protected bool _classificationKeyPoint;
        protected bool _classificationWithheld;
        protected sbyte _scanAngleRank;
        protected byte _userData;

        protected LasPointLong(byte bitsField, ClassificationType classification, ushort intensity,
            ushort pointSourceId,
            sbyte scanAngleRank, byte userData, int x, int y, int z)
            : base(classification, intensity, pointSourceId, x, y, z)
        {
            _bitsField = new BitsField(bitsField);
            _scanAngleRank = scanAngleRank;
            _userData = userData;
        }

        protected LasPointLong(byte[] rawBytes, int startPosition) : base(rawBytes, startPosition)
        {
            var position = startPosition;
            position += 3*sizeof(int) + sizeof(ushort);

            _bitsField = new BitsField(rawBytes[position]);
            position += 1;

            _classificationSynthetic = BitFromByte(rawBytes[position], 5);
            _classificationKeyPoint = BitFromByte(rawBytes[position], 6);
            _classificationWithheld = BitFromByte(rawBytes[position], 7);
            position += 1;

            _scanAngleRank = (sbyte) rawBytes[position];
            position += 1;

            _userData = rawBytes[position];
        }

        static bool BitFromByte(byte value, int numberOfBit)
        {
            return (value & (1 << numberOfBit - 1)) != 0;
        }

        public sbyte ScanAngleRank
        {
            get { return _scanAngleRank; }
        }

        public byte UserData
        {
            get { return _userData; }
        }

        public BitArray ReturnNumber
        {
            get { return _bitsField.ReturnNumber; }
        }

        public BitArray NumberOfReturns
        {
            get { return _bitsField.NumberOfReturns; }
        }

        public bool ScanDirectionFlag
        {
            get { return _bitsField.ScanDirectionFlag; }
        }

        public bool EdgeOfFlightLine
        {
            get { return _bitsField.EdgeOfFlightLine; }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "_bitsField: {0}, Classification: {1}, Intensity: {2}, PointSourceId: {3}, ScanAngleRank: {4}, UserData: {5}, X: {6}, Y: {7}, Z: {8}",
                    _bitsField, Classification, _intensity, _pointSourceId, _scanAngleRank, _userData, _x, _y, _z);
        }
    }
}