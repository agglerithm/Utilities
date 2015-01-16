using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.IO
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
            _buff = new byte[BlockSize];
        }

        public T[] Read(Func<Stream> read)
        { 
            var lines = new List<T>();

            var strm = Read(read, lines.Add);

            strm.Close();

            return lines.ToArray();
        }

        public Stream Write(Func<byte[], Stream> write)
        {
            var strm = write(_buff);
            strm.Position = Offset;
            strm.Write(_buff, 0, BlockSize);
            //Console.WriteLine("Written {0} bytes to offset {1}; position is now {2}", BlockSize, Offset, strm.Position);
            return strm;
        }

        public int BlockSize
        {
            get { return RecordSize * _recordCount; }
        }

        public int RecordSize { get { return _specs.GrossRecordSize; } }

        public int ActualRecordCount { get { return _recordCount;  } }

        public int Offset { get { return _blockIndex*RecordSize*_specs.RecordCount; } }

        public Stream Read(Func<Stream> read, Action<T> process)
        {
            var outputStr = read();
            outputStr.Position = Offset;
            var sz = outputStr.Read(_buff, 0, BlockSize);
            if (sz != BlockSize)
                _recordCount = sz / RecordSize;
           var str = _buff.BuffToString();

           var lineText = str.Split(_specs.Delimiter).Where(f => !f.SafeTrim().IsNullOrEmpty());

            foreach (string t in lineText)
            { 
                var line = new T
                {
                    Delimiter = _specs.Delimiter,
                    RecordSize = _specs.RecordSize
                };
                line.Parse(t);
                if(process != null)
                    process(line);
            }
            return outputStr;
        }
 
    }
}