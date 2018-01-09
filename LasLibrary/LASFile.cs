using System.IO;

namespace LASLibrary
{
    public class LasFile
    {
        public LasFile()
        {
        }

        public LasFile(string filename, bool useShortFormat = true)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(string.Format("File {0} don`t exist!", filename), filename);
            Filename = filename;
            LasHeader = new LasHeader(filename, this);
            LasVariableLengthRecords = new LasVariableLengthRecords(filename, LasHeader.NumberOfVariableLengthRecords);
            LasPointDataRecords = new LasPointDataRecords(LasHeader, 250, useShortFormat);
        }

        public LasHeader LasHeader { get; private set; }
        public LasVariableLengthRecords LasVariableLengthRecords { get; private set; }
        public LasPointDataRecords LasPointDataRecords { get; private set; }
        public string Filename { get; private set; }
    }
}