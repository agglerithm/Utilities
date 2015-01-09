using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace UtilTest
{
    [TestClass]
    public class ProcCollectionTester
    {
        private ProcessCollection _procs = new ProcessCollection();
        [TestMethod]
        public void can_enumerate_processes()
        {
            var proc = _procs.First();

            Assert.IsNotNull(proc);

            do
            {
                Console.WriteLine(proc.ProcessName);
                proc = _procs.Next();
            } while (proc != null);
        }

        [TestMethod]
        public void can_get_procs_by_name()
        {
            var procList = ProcessCollection.GetProcessesByName("chrome");

            foreach(var p in procList)
                Console.WriteLine(p.MainWindowTitle);
        }
    }
}
