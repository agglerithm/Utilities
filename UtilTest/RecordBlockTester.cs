using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UtilTest
{
    [TestClass]
    public class RecordBlockTester
    {
        private RecordBlock<FileRecord> _sut;
        private List<FileRecord> _lst = new List<FileRecord>();
        
        [TestMethod]
        public void can_read_part_of_file()
        {
            var rec = new FileRecord() ;
            var blockSpecs = new RecordBlockSpecs<FileRecord>(rec, 1000);
            _sut = new RecordBlock<FileRecord>(blockSpecs,5);
            var strm = File.OpenRead("myBigFile");
            _sut.Read(strm, process);
            Assert.AreEqual(_lst.Count, 1000);
        }

        private void process(FileRecord obj)
        {
            _lst.Add(obj);
        }
    }
}