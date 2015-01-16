using System; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.WinServices;

namespace UtilTest
{
    [TestClass]
    public class ServiceFixtureTester
    {
        private ServiceBootstrapper sut;
       
        private bool _started = false;
        private bool _stopped;

        [TestInitialize]
        public void setup()
        {
            sut = new ServiceBootstrapper(new TestHelper(new string[] { "/c" }, false))
            {
                ServiceName = "MyTestService",
                ServiceDescription = "A Test Service"
            };
        }
        [TestMethod]
        public void can_run_service_in_console()
        {
            sut.Register(start,stop,Console.WriteLine);
            sut.Run();

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

    internal class TestHelper : IServiceHelper
    {
        public TestHelper(string[] args, bool isRunningAsSvc)
        {
            CommandLineArguments = args;
            IsRunningAsService = isRunningAsSvc;
        }
        public bool IsRunningAsService { get; private set; }
        public string[] CommandLineArguments { get; private set; }
        public bool UserIsAdministrator { get; private set; }

        public bool IsInstalled(string serviceName)
        {
            throw new NotImplementedException();
        }
    }
}