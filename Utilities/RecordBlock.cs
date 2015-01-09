using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.Extensions;

namespace Utilities
{
    public class RecordBlock<T> where T:FileRecordBase, new()
    { 
        private readonly int _blockIndex;
        private readonly byte[] _buff;
        private readonly RecordBlockSpecs<T> _specs;
        private int _recordCount; 

        public RecordBlock(RecordBlockSpecs<T> specs, int blockIndex)
        { 
            _blockIndex = blockIndex;
            _specs = specs;
            _recordCount = specs.RecordCount;
            _buff = new byte[Offset + BlockSize];
        }

        public T[] Read(Stream outputStr)
        { 
            var lines = new List<T>();

            Read(outputStr, lines.Add);

            return lines.ToArray();
        }

        public Stream Write()
        {
            return new MemoryStream(_buff);
        }

        public int BlockSize
        {
            get { return RecordSize * _recordCount; }
        }

        public int RecordSize { get { return _specs.GrossRecordSize; } }

        public int ActualRecordCount { get { return _recordCount;  } }

        public int Offset { get { return _blockIndex*RecordSize*_specs.RecordCount; } }

        public void Read(Stream outputStr, Action<T> process)
        {
            var sz = outputStr.Read(_buff, Offset, BlockSize);
            if (sz != BlockSize)
                _recordCount = sz / RecordSize;
           var str = _buff.BuffToString(); 

            var lineText = str.Split(_specs.Delimiter).Where(f => !f.IsNullOrEmpty());

            foreach (string t in lineText)
            { 
                var line = new T
                {
                    Delimiter = _specs.Delimiter,
                    RecordSize = _specs.RecordSize
                };
                line.Parse(t);
                process(line);
            } 
        }
 
    }
}