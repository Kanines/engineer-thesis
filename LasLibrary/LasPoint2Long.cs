namespace LASLibrary
{
    public class LasPoint2Long : LasPointLong, ILasPointWithRgb
    {
        readonly ushort _red;
        readonly ushort _green;
        readonly ushort _blue;

        public LasPoint2Long(byte bitsField, ClassificationType classification, ushort intensity, ushort pointSourceId,
            sbyte scanAngleRank, byte userData, int x, int y, int z, ushort red, ushort green, ushort blue)
            : base(bitsField, classification, intensity, pointSourceId, scanAngleRank, userData, x, y, z)
        {
            _red = red;
            _green = green;
            _blue = blue;
        }

        public LasPoint2Long(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
        {
            _red = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 20);
            _green = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 22);
            _blue = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 24);
        }

        public ushort Red
        {
            get { return _red; }
        }

        public ushort Green
        {
            get { return _green; }
        }

        public ushort Blue
        {
            get { return _blue; }
        }

        public override string ToString()
        {
            return string.Format("{0}, Red: {1}, Green: {2}, Blue: {3}", base.ToString(), _red, _green, _blue);
        }
    }
}