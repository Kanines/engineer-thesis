namespace LASLibrary
{
    public class LasPoint0Short : LasPointShort
    {
        public LasPoint0Short(ClassificationType classification, ushort intensity, ushort pointSourceId, int x,
            int y, int z) : base(classification, intensity, pointSourceId, x, y, z)
        {
        }

        public LasPoint0Short(byte[] rawBytes, int startPosition = 0) : base(rawBytes, startPosition)
        {
        }
    }
}