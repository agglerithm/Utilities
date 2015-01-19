using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap.AutoMocking.NSubstitute;
using Utilities;
using Utilities.Extensions;

namespace UtilTest
{
    [TestClass]
    public class JumboFileTester
    {
        private FileProcessorMt<FileRecord> _fileProc;
        private List<FileRecord> _recList = new List<FileRecord>();
        private object _listLocker = new object();

//        [TestMethod]
//        public void can_create_jumbo_file()
//        {
//            var startTime = DateTime.Now;
//            var rec = new FileRecord(){createDate = DateTime.Parse("12/29/2014")};
//            var str = File.CreateText("myBigFile"); 
//            for(int i = 0; i < 1000000; i++)
//                str.WriteLine(rec.getRecord());
//            str.Close();
//            Console.WriteLine("Done! Took " + (DateTime.Now - startTime).Seconds + " seconds.");
//        } 

        [TestMethod]
        public void can_read_multithread_file()
        {
            _recList.Clear();
            _fileProc = new FileProcessorMt<FileRecord>("myBigFile", 
                new RecordBlockSpecs<FileRecord>(new FileRecord(),100000));
            var startTime = DateTime.Now;
            _fileProc.ReadAndProcess(proc);
            _recList.Count.ShouldEqual(1000000);
            Console.WriteLine("Multithread file read done! Blocks of {0} took {1} seconds.", _fileProc.BlockCount, (DateTime.Now - startTime).Seconds);
 

        }
        [TestMethod]
        public void can_read_max_threads()
        {
            _recList.Clear();
            var startTime = DateTime.Now;
            _fileProc = new FileProcessorMt<FileRecord>("myBigFile",
                new RecordBlockSpecs<FileRecord>(new FileRecord(), 100000));
            _fileProc.ReadAndProcess(proc, 4);
            _recList.Count.ShouldEqual(1000000);
            Console.WriteLine("Multithread file read done! Max of {0} threads took {1} seconds.", 4, (DateTime.Now - startTime).Seconds);
        }

        [TestMethod]
        public void can_read_one_thread()
        {
            _recList.Clear();
            var startTime = DateTime.Now;
            _fileProc = new FileProcessorMt<FileRecord>("myBigFile",
                new RecordBlockSpecs<FileRecord>(new FileRecord(), 1000000));
            _fileProc.SingleThreadReadAndProcess(proc);
            _recList.Count.ShouldEqual(1000000);
            Console.WriteLine("Single thread file read done! Took {0} seconds.",  (DateTime.Now - startTime).Seconds);
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