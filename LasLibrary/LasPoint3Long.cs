using System;

namespace LASLibrary
{
    public class LasPoint3Long : LasPointLong, ILasPointWithRgb
    {
        readonly ushort _blue;
        readonly double _gpsTime;
        readonly ushort _green;
        readonly ushort _red;

        public LasPoint3Long(byte bitsField, ClassificationType classification, ushort intensity, ushort pointSourceId,
            sbyte scanAngleRank, byte userData, int x, int y, int z, double gpsTime, ushort red, ushort green,
            ushort blue) : base(bitsField, classification, intensity, pointSourceId, scanAngleRank, userData, x, y, z)
        {
            _gpsTime = gpsTime;
            _red = red;
            _green = green;
            _blue = blue;
        }

        public LasPoint3Long(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
        {
            _gpsTime = BitConverter.ToDouble(rawBytes, startPosition + 20);
            _red = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 28);
            _green = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 30);
            _blue = (ushort) MyBitConverter.ToInt16(rawBytes, startPosition + 32);
        }

        public double GpsTime
        {
            get { return _gpsTime; }
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
            return string.Format("{0}, GpsTime: {1}, Red: {2}, Green: {3}, Blue: {4}", base.ToString(), _gpsTime, _red,
                _green, _blue);
        }
    }
}