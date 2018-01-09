using System;
using System.Collections;
using System.IO;

namespace LASLibrary
{
    public class LasVariableLengthRecords : ICollection
    {
        readonly LasVariableLengthRecord[] _records;
        readonly uint _count;

        public LasVariableLengthRecords(string fileName, uint count)
        {
            _count = count;
            _records = new LasVariableLengthRecord[count];
            using (var b = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                b.BaseStream.Seek(LasHeader.SIZE, SeekOrigin.Begin);
                for (uint i = 0; i < count; i++)
                {
                    var by = b.ReadBytes(LasVariableLengthRecord.Size);
                    _records[i] = new LasVariableLengthRecord(by);
                    b.BaseStream.Seek(_records[i].RecordLengthAfterHeader, SeekOrigin.Current);
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var record in _records)
            {
                yield return record;
            }
        }

        public void CopyTo(Array array, int index)
        {
            foreach (var i in _records)
            {
                array.SetValue(i, index);
                index = index + 1;
            }
        }

        public int Count
        {
            get { return (int) _count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public LasVariableLengthRecord[] VariableLengthRecords
        {
            get { return _records; }
        }
    }
}