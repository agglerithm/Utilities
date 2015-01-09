using System;
using System.Diagnostics;
using System.ServiceProcess;
using Utilities.Extensions;

namespace Utilities
{
    public interface IServiceFixture
    {
        void Register(Func<string[],  bool> start, Action stop);

        void Run();
    }

    public class ServiceFixture : ServiceBase, IServiceFixture
    {
        private Func<string[],  bool> _start;
        private Action _stop;
        private bool _isRunningAsService; 


        protected override void OnStart(string[] args)
        {
            if (!_start(args))
                throw new SystemException("Could not start the service!");
        }

        protected override void OnStop()
        {
            _stop();
        }

        public void Register(Func<string[],  bool> start, Action stop)
        {
            _start = start;
            _stop = stop;
            _isRunningAsService = Process.GetCurrentProcess().IsRunningAsAService();
        }

        public void Run()
        { 
 
                if(_isRunningAsService)
                    Run(this);
                else
                {
                    _start(Environment.GetCommandLineArgs()); 
                } 
 
        }
 
    }

}
