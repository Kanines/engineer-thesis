using System;

namespace LASLibrary
{
    public class LasPoint1Short : LasPointShort
    {
        readonly double _gpsTime;

        public LasPoint1Short(ClassificationType classification, ushort intensity, ushort pointSourceId, int x,
            int y, int z, double gpsTime) : base(classification, intensity, pointSourceId, x, y, z)
        {
            _gpsTime = gpsTime;
        }

        public LasPoint1Short(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
        {
            _gpsTime = BitConverter.ToDouble(rawBytes, startPosition + 20);
        }

        public double GpsTime
        {
            get { return _gpsTime; }
        }

        public override string ToString()
        {
            return string.Format("{0}, GpsTime: {1}", base.ToString(), _gpsTime);
        }
    }
}