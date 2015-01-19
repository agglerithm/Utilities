using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;
using Utilities.IO;
using Utilities.Test;

namespace UtilTest
{
    [TestClass]
    public class JumboFileTester
    {
        private FileProcessorMt<FileRecord> _fileProc;
        private List<FileRecord> _recList = new List<FileRecord>();
        private object _listLocker = new object();
        private long _fileSize;

        [TestInitialize]
        public void setup()
        { 
            set_file_size();
        }
        [TestMethod, Ignore]
        public void can_create_jumbo_file()
        {
            using (TestTimer.Start("Finished creating large file!"))
            {
                var rec = new FileRecord();
                var str = File.CreateText("myBigFile");
                for (int i = 0; i < 1000000; i++)
                    str.WriteLine(rec.getRecord());
                str.Close();
            }
        } 

        [TestMethod]
        public void can_read_multithread_file()
        {
            _recList.Clear();
            _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(),250000));
            using (TestTimer.Start("Finished reading large file (multi-threaded)!"))
            {
                _fileProc.ReadAndProcess(readProc, proc,_fileSize);
                _recList.Count.ShouldEqual(1000000);
            }
        }



        [TestMethod]
        public void can_read_max_threads()
        { 
            _recList.Clear();
            using (TestTimer.Start("Finished reading large file (max-threads)!"))
            {
                _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(), 100000));
                _fileProc.ReadAndProcess(readProc, proc, _fileSize, 4);
                //var lastRec = _recList.Last();
                _recList.Count.ShouldEqual(1000000);
            }
        }

        [TestMethod]
        public void can_read_one_thread()
        { 
            _recList.Clear();
            using (TestTimer.Start("Finished reading large file (single-threaded)!"))
            { 
                _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(), 1000000));
                _fileProc.SingleThreadReadAndProcess(readProc, proc, _fileSize); 
            }
        }

        [TestMethod]
        public void can_copy_one_thread()
        { 
            _recList.Clear();
            using (TestTimer.Start("Finished copying large file (single-threaded)!"))
            {
                _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(), 1000000));
                _fileProc.SingleThreadCopy(readProc, writeProc, _fileSize); 
            }
        }

        [TestMethod]
        public void can_copy_large_file()
        { 
            using (TestTimer.Start("Finished copying large file (multi-threaded)!"))
            {
                _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(), 250000));
                _fileProc.Copy(readProc, writeProc, _fileSize);
            }

        }

        private void set_file_size()
        {
            _fileSize = new FileInfo("myBigFile").Length;
        }

        private Stream writeProc(byte[] buff)
        {
            Console.WriteLine("Getting ready to write a block.");
            return new FileStream("myCopiedBigFile", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write); 
        }

        private Stream readProc()
        {
            return File.OpenRead("myBigFile");
        }

        private void proc(FileRecord obj)
        {
            if (obj == null) return;
            lock(_listLocker)
            { 
                _recList.Add(obj); 
            }
        }
    }
}