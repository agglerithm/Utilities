using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Utilities.IO
{
    public class MultiThreadFileProc<T> where T: FileRecordBase, new()
    { 
        private IRecordBlockSpecs _specs; 
        private int _blockCount;
        private long _fileSize;
        private int _blocksRead;
        private readonly object _counterLocker = new object();
        private readonly ManualResetEvent _doneEvent = new ManualResetEvent(false);
        private int _maxThreads;

        public MultiThreadFileProc(IRecordBlockSpecs specs)
        { 
            _specs = specs;
        }
 
        private class WorkObject
        {
            internal WorkObject(IRecordBlock<T> block, Func<Stream> readStreamInitializer, 
                Func<byte[],Stream> writeStreamInitializer, Action<T> procAction)
            {
                Block = block;
                ProcAction = procAction;
                InitializeWrite = writeStreamInitializer;
                InitializeRead = readStreamInitializer;
            }



            public Func<Stream> InitializeRead { get; private set; }

            public Func<byte[], Stream> InitializeWrite { get; private set; }
             

            public Action<T> ProcAction { get; private set; }

            public IRecordBlock<T> Block { get; private set; }
        }

        private class StreamlessWorkObject
        {             public StreamlessWorkObject(RecordBlock<T> block, Func<Stream> readStreamInitializer, 
            Action<Stream, int, int> writeAction)
            {
                Block = block; 
                WriteAction = writeAction;
                InitializeRead = readStreamInitializer;
            }
            public Func<Stream> InitializeRead { get; private set; }
            public IRecordBlock<T> Block { get; private set; }
            public Action<Stream, int, int> WriteAction { get; private set; }
        }

        public void Copy(Func<Stream> readStreamInitializer, Func<byte[], Stream> writeStreamInitializer, long fileSize)
        {
            var blocks = analyzeFile(fileSize);
            foreach (var block in blocks)
            {
                ThreadPool.QueueUserWorkItem(work, new WorkObject(block, readStreamInitializer, 
                    writeStreamInitializer, null));
            }
            _doneEvent.WaitOne(); 
        }

        public void Copy(Func<Stream> readStreamInitializer, Action<Stream,  int, int> writeAction, long fileSize)
        {
            var blocks = analyzeFile(fileSize);
            foreach (var block in blocks)
            {
                ThreadPool.QueueUserWorkItem(workStreamlessWrite, new StreamlessWorkObject(block, readStreamInitializer,
                    writeAction));
            }
            _doneEvent.WaitOne();
        }

        private void workStreamlessWrite(object state)
        {
            var obj = (StreamlessWorkObject)state;
            var block = obj.Block; 
            var write = obj.WriteAction;
            var read = obj.InitializeRead;
            var strm = block.Read(read, null);
            if (write != null)
            {
                block.StreamlessWrite(strm, write); 
            }
            strm.Close();
            lock (_counterLocker)
            {
                if (++_blocksRead == _blockCount)
                    _doneEvent.Set();
            }
        }

        public void ReadAndProcess(Func<Stream> readStreamInitializer, Action<T> proc, long fileSize)
        {
            var blocks = analyzeFile(fileSize); 
            foreach (var block in blocks)
            {
                ThreadPool.QueueUserWorkItem(work, new WorkObject(block, readStreamInitializer, null, proc));
            }
            _doneEvent.WaitOne();

        }
        public void ReadAndProcess(Func<Stream> readStreamInitializer, Action<T> proc, long fileSize, int maxThreads)
        {
            var blocks = analyzeFile(fileSize);
            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            foreach (var block in blocks)
            {
                ThreadPool.QueueUserWorkItem(work, new WorkObject(block, readStreamInitializer, null, proc));
            }
            _doneEvent.WaitOne();

        }

 
        private void work(object state)
        {
            var obj = (WorkObject)state;
            var block = obj.Block;
            var process = obj.ProcAction;
            var write = obj.InitializeWrite;
            var read = obj.InitializeRead;
            var strm = block.Read(read, process);
            strm.Close();
            if(write != null)
            {
                var writeStrm = block.Write(write);
                writeStrm.Close();
            }
            lock (_counterLocker)
            {
                if (++_blocksRead == _blockCount)
                    _doneEvent.Set();
            }
        }

        private RecordBlock<T>[] analyzeFile(long fileSize)
        {  
            _fileSize = fileSize;
            _blockCount = _specs.GetBlockCount(_fileSize);
            var lst = new List<RecordBlock<T>>();
            for(int i = 0; i < _blockCount; i++)
                lst.Add(new RecordBlock<T>(_specs, i));
            return lst.ToArray();
        }



        [Obsolete("For test only!")]
        public void SingleThreadCopy(Func<Stream> readProc, Func<byte[], Stream> writeProc, long fileSize)
        {
            _specs = new RecordBlockSpecs<T>(57, 1000000, "\r\n");
            var blocks = analyzeFile(fileSize); 
            var strm = blocks[0].Read(readProc, null);
            strm.Close();
            var wstrm = blocks[0].Write(writeProc);
            wstrm.Close();
        }

        [Obsolete("For test only!")]
        public void SingleThreadReadAndProcess(Func<Stream> readProc, Action<T> proc, long fileSize)
        {
            _specs = new RecordBlockSpecs<T>(57, 1000000, "\r\n");
            var blocks = analyzeFile(fileSize);
            var strm = blocks[0].Read(readProc, null);
            strm.Close();
        }
    }
}