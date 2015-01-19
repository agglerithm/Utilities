using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Utilities.Extensions;

namespace Utilities.WinServices
{
    public interface IServiceHelper
    {
        bool IsRunningAsService { get;  }
        string[] CommandLineArguments { get;   }
        bool UserIsAdministrator { get; }
        bool IsInstalled(string serviceName);
    }

    public class ServiceHelper : IServiceHelper
    {
        public bool IsRunningAsService
        {
            get { return Process.GetCurrentProcess().IsRunningAsAService(); }
        }

        public string[] CommandLineArguments
        {
            get { return Environment.GetCommandLineArgs(); }
        }

        public bool IsInstalled(string serviceName)
        {
            return ServiceController.GetServices()
                    .Any(service => string.CompareOrdinal(service.ServiceName, serviceName) == 0);
        }

        public bool UserIsAdministrator
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                if (null == identity) return false;
                var principal = new WindowsPrincipal(identity);

                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}