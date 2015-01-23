using System;

namespace Utilities.IO
{
    public class AdHocRecordBlockSpecs : IRecordBlockSpecs
    {
        public int RecordSize { get; private set; }

        public AdHocRecordBlockSpecs(int recordSize, int recordsPerBlock)
        {
            RecordSize = recordSize;
            RecordCount = recordsPerBlock;
            BlockSize = recordSize * recordsPerBlock;
        }

        public AdHocRecordBlockSpecs() : this(5000, 100)
        {
            
        }

        public int GrossRecordSize
        {
            get { return RecordSize; }
        }

        public int RecordCount { get; private set; }

        public string Delimiter { get { return ""; } }

        public int DelimiterLength
        {
            get { return 0; }
        }
         
        public int BlockSize { get; private set; }

        public FileRecordBase GetFileRecordSpecs()
        {
            return new AdHocRecord();
        }

        public int GetBlockCount(long fileSize)
        {
            var blockCount = fileSize/BlockSize;
            if (fileSize%BlockSize > 0)
                blockCount++;
            return (int)blockCount;
        }
    }
}