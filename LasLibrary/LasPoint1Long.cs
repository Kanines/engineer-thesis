using System;

namespace LASLibrary
{
    public class LasPoint1Long : LasPointLong
    {
        readonly double _gpsTime;

        public LasPoint1Long(byte bitsField, ClassificationType classification, ushort intensity, ushort pointSourceId,
            sbyte scanAngleRank, byte userData, int x, int y, int z, double gpsTime)
            : base(bitsField, classification, intensity, pointSourceId, scanAngleRank, userData, x, y, z)
        {
            _gpsTime = gpsTime;
        }

        public LasPoint1Long(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
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