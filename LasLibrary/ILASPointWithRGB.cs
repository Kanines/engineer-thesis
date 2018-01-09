namespace LASLibrary
{
    public interface ILasPointWithRgb
    {
        ushort Red { get; }
        ushort Blue { get; }
        ushort Green { get; }
    }
}