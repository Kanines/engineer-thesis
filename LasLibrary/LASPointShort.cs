namespace LASLibrary
{
    public abstract class LasPointShort : LasPoint
    {
        protected LasPointShort(ClassificationType classification, ushort intensity, ushort pointSourceId,
            int x, int y, int z) : base(classification, intensity, pointSourceId, x, y, z)
        {
        }

        protected LasPointShort(byte[] rawBytes, int startPosition) : base(rawBytes, startPosition)
        {
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Classification: {0}, Intensity: {1}, PointSourceId: {2}, X: {3}, Y: {4}, Z: {5}",
                    Classification, _intensity, _pointSourceId, _x, _y, _z);
        }
    }
}