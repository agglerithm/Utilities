using System.Collections.Generic;
using System.ServiceProcess;

namespace Utilities.WinServices
{
    public class ServiceSettings
    {
        public string[] Dependencies { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public ServiceAccount Account { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string InstanceName { get; set; }
        public string Name { get; set; }
        public string[] Args { get; private set; }

        public void SetArguments(string[] args)
        {
            var lst = new List<string>();
            int startNdx = 0;
            if (args[0] == "/i" || args[0] == "-i" || args[0] == "install")
            { 
                DoInstall = true;
                startNdx = 1;
            }
            for(int i = startNdx; i < args.Length; i++)
                    lst.Add(args[i]);
            Args = lst.ToArray();
        }

        public bool DoInstall { get; private set; }
    }
}