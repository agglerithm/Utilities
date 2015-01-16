using System;

namespace Utilities.WinServices
{
    public interface IServiceBootstrapper
    {
        void Register(Func<string[], bool> start, Action stop, Action<string> logAction);

        void Run();

        string ServiceName { get; set; }

        string ServiceDescription { get; set; }
    }
}

 
