using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;
using Utilities.IO;

namespace UtilTest
{
    [TestClass]
    public class RecordBlockTester
    {
        private RecordBlock<FileRecord> _sut;
        private List<FileRecord> _lst = new List<FileRecord>();

        [TestInitialize]
        public void setup()
        {
            var rec = new FileRecord() ;
            var blockSpecs = new RecordBlockSpecs<FileRecord>(rec, 1000);
            _sut = new RecordBlock<FileRecord>(blockSpecs,5); 
            
        }
        [TestMethod]
        public void can_read_part_of_file()
        {
            var strm = _sut.Read(read, process);
            strm.Close();
            Assert.AreEqual(_lst.Count, 1000);
        }

        private Stream read()
        {
            return File.OpenRead("myBigFile"); 
        }

        private void process(FileRecord obj)
        {
            _lst.Add(obj);
        }
    }
}