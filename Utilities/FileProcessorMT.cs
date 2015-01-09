using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Utilities
{
    public class FileProcessorMt<T> where T: FileRecordBase, new()
    {
        private readonly string _file;
        private RecordBlockSpecs<T> _specs; 
        private int _blockCount;
        private long _fileSize;
        private int _blocksRead;
        private object _counterLocker = new object();
        private readonly ManualResetEvent _doneEvent = new ManualResetEvent(false);
        private int _maxThreads;


        public FileProcessorMt(string file, RecordBlockSpecs<T> specs)
        {
            _file = file;
            _specs = specs;
        }

        public int BlockCount { get { return _specs.RecordCount; }}

        public void Read()
        { 
        }

        private class WorkObject
        {
            internal WorkObject(RecordBlock<T> block, Action<T> procAction)
            {
                Block = block;
                ProcAction = procAction;
            }

            public Action<T> ProcAction { get; private set; }

            public RecordBlock<T> Block { get; private set; }
             
        }

        [Obsolete("For test only!")]
        public void SingleThreadReadAndProcess(Action<T> proc)
        { 
            _specs = new RecordBlockSpecs<T>(57,1000000,"\r\n");
            var blocks = analyzeFile();
            var strm = File.OpenRead(_file);
            blocks[0].Read(strm, proc);
            strm.Close();
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
        public void ReadAndProcess(Action<T> proc, int maxThreads)
        { 
            var blocks = analyzeFile();
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
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
            if (_fileSize%_specs.BlockSize > 0) _blockCount++;
            var lst = new List<RecordBlock<T>>();
            for(int i = 0; i < _blockCount; i++)
                lst.Add(new RecordBlock<T>(_specs, i));
            return lst.ToArray();
        }
    }
}