using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.IO;
using Utilities.Test;

namespace UtilTest
{
    [TestClass]
    public class IoUtilitiesTester
    {
        [TestMethod]
        public void CanReadSegmentOfXmlFile()
        {
            var strm = File.Open("sampleXml.txt", FileMode.Open);
            var str = StreamUtilities.GetBlockText(strm, 3000, "<Select", "</Select>");
            Console.Write(str.Text);
            str.Records.Count().ShouldEqual(2);
            str.LeadingGarbage.ShouldNotBeNull();
            str.TrailingGarbage.ShouldNotBeNull();
        }
    }
}