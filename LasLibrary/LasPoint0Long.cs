namespace LASLibrary
{
    public class LasPoint0Long : LasPointLong
    {
        public LasPoint0Long(byte bitsField, ClassificationType classification, ushort intensity, ushort pointSourceId,
            sbyte scanAngleRank, byte userData, int x, int y, int z)
            : base(bitsField, classification, intensity, pointSourceId, scanAngleRank, userData, x, y, z)
        {
        }

        public LasPoint0Long(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
        {
        }
    }
}