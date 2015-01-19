using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rebex.Net;
using Utilities;
using Utilities.Test;

namespace UtilTest
{
    [TestClass]
    public class FTPSFileTester
    {
        private FileProcessorMt<FileRecord> _fileProc;
        private List<FileRecord> _recList = new List<FileRecord>();
        private object _listLocker = new object();
        private long _fileSize;
        private Ftp _ftp;
        [TestInitialize]
        public void setup()
        {
            set_file_size();
            _ftp = new WebRequestMethods.Ftp();
            var str = _ftp.Connect("ftp.johnmreese.net"); 
            Console.WriteLine(str);
            _ftp.SendCommand("");
        }

        private void set_file_size()
        {
            _fileSize = 50;
        }

 

        [TestMethod]
        public void can_read_multithread_file()
        {
            _recList.Clear();
            _fileProc = new FileProcessorMt<FileRecord>(new RecordBlockSpecs<FileRecord>(new FileRecord(), 250000));
            using (TestTimer.Start("Finished reading large file (multi-threaded)!"))
            {
 
            }
        }

        private void proc(FileRecord obj)
        {
            throw new NotImplementedException();
        }

        private Stream readProc()
        {
            throw new NotImplementedException();
        }
    }
}