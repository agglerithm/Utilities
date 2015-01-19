using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace Utilities.WinServices
{
    public class WinServiceInstaller
    { 
        private readonly TransactedInstaller transactedInstaller;

        public WinServiceInstaller(ServiceSettings settings){    

            var installer = CreateInstaller(settings);

            transactedInstaller = CreateTransactedInstaller(installer,settings.Args);
        }  
		 
        static Installer CreateInstaller(ServiceSettings settings)
        {
            var installers = new Installer[]
            {
                ConfigureServiceInstaller(settings, settings.Dependencies, settings.StartMode),
                ConfigureAccountInstaller(settings.Account, settings.Username, settings.Password)
            };

            return CreateHostInstaller(settings, installers);
        }
		
        static Installer CreateHostInstaller(ServiceSettings settings, Installer[] installers)
        {
            string arguments = " ";

            if (!string.IsNullOrEmpty(settings.InstanceName))
                arguments += string.Format(" -instance \"{0}\"", settings.InstanceName);

            if (!string.IsNullOrEmpty(settings.DisplayName))
                arguments += string.Format(" -displayname \"{0}\"", settings.DisplayName);

            if (!string.IsNullOrEmpty(settings.Name))
                arguments += string.Format(" -servicename \"{0}\"", settings.Name);

            var parentInstaller = new ParentInstaller(installers, arguments,settings);

            return parentInstaller;

        }

        public void Install(Action<string> logAction)
        {
            try
            {
                transactedInstaller.Install(new Hashtable()); 
            }
            catch (Exception ex)
            {
                logAction(string.Format("There was an error installing the service: {0}", ex));
            }
        }

        static ServiceProcessInstaller ConfigureAccountInstaller(ServiceAccount account, string username,
            string password)
        {
            var installer = new ServiceProcessInstaller
            {
                Username = username,
                Password = password,
                Account = account,
            };

            return installer;
        }
		
        static TransactedInstaller CreateTransactedInstaller(Installer installer, string[] args)
        {
            var transactedInstaller = new TransactedInstaller();

            transactedInstaller.Installers.Add(installer);

            Assembly assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                throw new Exception("Assembly.GetEntryAssembly() is null for some reason.");

            string path = string.Format("/assemblypath={0}", assembly.Location);

            var commandLine = new List<string> {path};

            commandLine.AddRange(args);

            var context = new InstallContext(null, commandLine.ToArray());

            transactedInstaller.Context = context;

            return transactedInstaller;
        }

        private static ServiceInstaller ConfigureServiceInstaller(ServiceSettings settings, string[] dependencies,
            ServiceStartMode startMode)
        {
            var installer = new ServiceInstaller
            {
                ServiceName = settings.ServiceName,
                Description = settings.Description,
                DisplayName = settings.DisplayName,
                ServicesDependedOn = dependencies,
                StartType = startMode
            };

            return installer;
        }
    }
}