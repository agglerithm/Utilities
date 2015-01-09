using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Utilities;

namespace UtilTest
{
    [TestClass]
    public class ServiceFixtureTester
    {
        private ServiceFixture _fixture = new ServiceFixture();
        private bool _started = false;
        private bool _stopped;

        [TestMethod]
        public void can_run_service_in_console()
        {
            _fixture.Register(start,stop);
            _fixture.Run();

            Assert.IsTrue(_started);
            Assert.IsFalse(_stopped);
        }

        private void stop()
        {
            _stopped = true;
        }

        private bool start(string[] arg)
        {
            Console.WriteLine("There are " + arg.Length + " arguments.");
            _started = true;
            return _started;
        }
    }


}