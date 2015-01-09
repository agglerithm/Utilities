using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Utilities;

namespace UtilTest
{
    public class JumboFileProcessor<T> where T: FileRecordBase, new()
    {
        private readonly string _file;
        private readonly RecordBlockSpecs<T> _specs; 
        private int _blockCount;
        private long _fileSize;
        private int _blocksRead;
        private object _counterLocker;
        private readonly ManualResetEvent _doneEvent = new ManualResetEvent(false);
        

        public JumboFileProcessor(string file, RecordBlockSpecs<T> specs)
        {
            _file = file;
            _specs = specs;
        }

        public void Read()
        { 
        }

        internal class WorkObject
        {
            internal WorkObject(RecordBlock<T> block, Action<T> procAction)
            {
                Block = block;
                ProcAction = procAction;
            }

            public Action<T> ProcAction { get; private set; }

            public RecordBlock<T> Block { get; private set; }
             
        }

        public void ReadAndProcess(Action<T> proc)
        {
            var blocks = analyzeFile(); 
            foreach (var block in blocks)
            {  
                ThreadPool.QueueUserWorkItem(work, new WorkObject(block, proc));
            }
            _doneEvent.WaitOne();

        }

        private void work(object state)
        {
            var obj = (WorkObject) state;
            var block = obj.Block;
            var process = obj.ProcAction;
            var strm = File.OpenRead(_file);
            block.Read(strm, process); 
            strm.Close();
            lock (_counterLocker)
            {
                if (++_blocksRead == _blockCount)
                    _doneEvent.Set();
            }
        }

        private RecordBlock<T>[] analyzeFile()
        {
            var info = new FileInfo(_file);
            _fileSize = info.Length;
            _blockCount = (int) (_fileSize/_specs.BlockSize);
            if (_fileSize%_specs.RecordSize > 0) _blockCount++;
            var lst = new List<RecordBlock<T>>();
            for(int i = 0; i < _blockCount; i++)
                lst.Add(new RecordBlock<T>(_specs, i));
            return lst.ToArray();
        }
    }
}