namespace LASLibrary
{
    public static class MyBitConverter
    {
        public static short ToInt16(byte[] rawBytes, int position)
        {
            return (short) (rawBytes[position] | (rawBytes[position + 1] << 8));
        }

        public static int ToInt32(byte[] rawBytes, int position)
        {
            return rawBytes[position] | (rawBytes[position + 1] << 8) | (rawBytes[position + 2] << 16) |
                   (rawBytes[position + 3] << 24);
        }

        public static long ToInt64(byte[] rawBytes, int position)
        {
            return rawBytes[position] | (rawBytes[position + 1] << 8) | (rawBytes[position + 2] << 16) |
                   (rawBytes[position + 3] << 24) | (rawBytes[position + 4] << 32) | (rawBytes[position + 5] << 40) |
                   (rawBytes[position + 6] << 48) | (rawBytes[position + 7] << 54);
        }
    }
}