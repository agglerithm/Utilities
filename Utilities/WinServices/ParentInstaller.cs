using System.Collections;
using System.Configuration.Install;
using Microsoft.Win32;

namespace Utilities.WinServices
{
    internal class ParentInstaller  : Installer
    {
        private readonly string _arguments;
        private ServiceSettings _settings;

        public ParentInstaller(Installer[] installers, string arguments, ServiceSettings settings)
        {
            Installers.AddRange(installers);
            _arguments = arguments;
            _settings = settings;
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            updateRegistry();
        }

        private void updateRegistry()
        {
            using (RegistryKey system = Registry.LocalMachine.OpenSubKey("System"))
            using (RegistryKey currentControlSet = system.OpenSubKey("CurrentControlSet"))
            using (RegistryKey services = currentControlSet.OpenSubKey("Services"))
            using (RegistryKey service = services.OpenSubKey(_settings.ServiceName, true))
            {
                service.SetValue("Description", _settings.Description);
                service.SetValue("ImagePath", (string) service.GetValue("ImagePath") + _arguments);
            }
        }
    }
}