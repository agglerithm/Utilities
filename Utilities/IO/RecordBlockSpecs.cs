namespace Utilities.IO
{
    public enum FileType { Delimited, FixedRecordSize, Xml}

    public interface IRecordBlockSpecs
    {
        int RecordSize { get; }
        int GrossRecordSize { get; }
        int RecordCount { get;  }
        string Delimiter { get; }
        int DelimiterLength { get; }
        int BlockSize { get; }
        FileRecordBase GetFileRecordSpecs();
        int GetBlockCount(long fileSize);
    }

    public class RecordBlockSpecs<T> : IRecordBlockSpecs where T:FileRecordBase, new()
    {
        private readonly T _recSpecs;

        public RecordBlockSpecs(int recordSize, int recordCount, string delimiter)
        { 
            RecordCount = recordCount; 
            _recSpecs = new T {RecordSize = recordSize, Delimiter = delimiter};
        }
        public RecordBlockSpecs(T frSpecs, int recordCount)
        { 
            RecordCount = recordCount; 
            _recSpecs = frSpecs;
        }
        public int RecordSize { get { return _recSpecs.RecordSize; } }
        public int GrossRecordSize { get { return _recSpecs.TotalSize; } } 
        public int RecordCount { get; set; }
        public string Delimiter { get { return _recSpecs.Delimiter; } }
        public int DelimiterLength { get { return Delimiter.Length; } }
        public int BlockSize
        {
            get { return GrossRecordSize*RecordCount; } 
        }

        public FileRecordBase GetFileRecordSpecs()
        {
            return _recSpecs;
        }
        public int GetBlockCount(long fileSize)
        {
            var blockCount = (int)(fileSize / BlockSize);
            if (fileSize % BlockSize > 0) blockCount++;
            return blockCount;
        }

    }
}