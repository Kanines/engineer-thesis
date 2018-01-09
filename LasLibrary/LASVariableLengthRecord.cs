using System;

namespace LASLibrary
{
    public class LasVariableLengthRecord
    {
        ushort _reserved;
        string _userId = string.Empty;
        ushort _recordId;
        ushort _recordLengthAfterHeader;
        string _description = string.Empty;

        public static int Size = 54;

        public LasVariableLengthRecord()
        {
        }

        public LasVariableLengthRecord(byte[] by)
        {
            Converter(by);
        }

        void Converter(byte[] rawBytes)
        {
            var position = 0;

            _reserved = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            for (var i = 0; i < 16; i++)
            {
                if (rawBytes[position + i] == 0) break;
                _userId += (char) rawBytes[position + i];
            }
            position += 16;

            _recordId = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            _recordLengthAfterHeader = BitConverter.ToUInt16(rawBytes, position);
            position += sizeof(ushort);

            for (var i = 0; i < 32; i++)
            {
                if (rawBytes[position + i] == 0) break;
                _description += (char) rawBytes[position + i];
            }
        }

        public ushort Reserved
        {
            get { return _reserved; }
        }

        public string UserId
        {
            get { return _userId; }
        }

        public ushort RecordId
        {
            get { return _recordId; }
        }

        public ushort RecordLengthAfterHeader
        {
            get { return _recordLengthAfterHeader; }
        }

        public string Description
        {
            get { return _description; }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Reserved: {0}, UserId: {1}, RecordId: {2}, RecordLengthAfterHeader: {3}, Description: {4}",
                    _reserved, _userId, _recordId, _recordLengthAfterHeader, _description);
        }
    }
}