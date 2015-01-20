using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Utilities.WinMockups;
using ServiceBase = Utilities.WinMockups.ServiceBase;

namespace Utilities.WinServices
{
    public class ServiceBootstrapper : ServiceBase, IServiceBootstrapper
    {
        private readonly IServiceHelper helper;
        private Func<string[],  bool> _start;
        private Action _stop;
        private bool _isRunningAsService; 
        private Action<string> _logAction;  
        private IntPtr statusHandle;  
        public const int MaxNameLength = 80; 
        private NativeMethods.ServiceControlCallback commandCallback;
        private NativeMethods.ServiceControlCallbackEx commandCallbackEx;
        private NativeMethods.ServiceMainCallback mainCallback;
        private IntPtr handleName;
        private ManualResetEvent startCompletedSignal;
        private int acceptedCommands;
        private bool autoLog;
        private string serviceName;
        private EventLog eventLog;
        private bool nameFrozen;
        private bool commandPropsFrozen;
        private bool disposed;
        private bool initialized;
        private bool isServiceHosted;

        public ServiceBootstrapper(IServiceHelper helper)
        {
            this.helper = helper;
            StartMode = ServiceStartMode.Automatic;
        }

        protected override void OnStart(string[] args)
        {
            _logAction("Starting service " + ServiceName);
            if (!_start(args))
                throw new SystemException("Could not start the service!");
        }

        protected override void OnStop()
        {
            _logAction("Stopping service " + ServiceName);
            _stop();
        }

        public void Register(Func<string[],  bool> start, Action stop, Action<string> logAction)
        {
            _start = start;
            _stop = stop;
            _isRunningAsService = helper.IsRunningAsService; 
        }

        public new void Run()
        { 
            var settings = getSettings();
            if (_isRunningAsService)
            {
                if (settings.DoInstall)
                { 
                    var installer = new WinServiceInstaller(settings);
                    installer.Install(_logAction);
                    return;
                }
                base.Run();
            }
            else
            {
                _start(settings.Args); 
            } 
 
        }

 

        private ServiceSettings getSettings()
        {
            var settings = new ServiceSettings
            {
                ServiceName = ServiceName,
                Name = ServiceName,
                DisplayName = ServiceName,
                Description = ServiceDescription,
                StartMode = this.StartMode
            };
            settings.SetArguments(helper.CommandLineArguments);
            return settings;
        }

        public string ServiceDescription { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string ServiceName { get; set; }

//        private void Initialize(bool multipleServices)
//        {
//            if (this.initialized)
//                return;
//            if (this.disposed)
//                throw new ObjectDisposedException(this.GetType().Name);
//            this.status.serviceType = multipleServices ? 32 : 16;
//            this.status.currentState = 2;
//            this.status.controlsAccepted = 0;
//            this.status.win32ExitCode = 0;
//            this.status.serviceSpecificExitCode = 0;
//            this.status.checkPoint = 0;
//            this.status.waitHint = 0;
//            this.mainCallback = new NativeMethods.ServiceMainCallback(this.ServiceMainCallback);
//            this.commandCallback = new NativeMethods.ServiceControlCallback(this.ServiceCommandCallback);
//            this.commandCallbackEx = new NativeMethods.ServiceControlCallbackEx(this.ServiceCommandCallbackEx);
//            this.handleName = Marshal.StringToHGlobalUni(this.ServiceName);
//            this.initialized = true;
//        }

//        private NativeMethods.SERVICE_TABLE_ENTRY GetEntry()
//        {
//            NativeMethods.SERVICE_TABLE_ENTRY serviceTableEntry = new NativeMethods.SERVICE_TABLE_ENTRY();
//            this.nameFrozen = true;
//            serviceTableEntry.callback = (Delegate)this.mainCallback;
//            serviceTableEntry.name = this.handleName;
//            return serviceTableEntry;
//        }
//        public static void Run(ServiceBootstrapper[] services)
//        {
//            if (services == null || services.Length == 0)
//                throw new ArgumentException("NoServices");
// 
//            else
//            {
//                IntPtr entry = Marshal.AllocHGlobal((IntPtr)((services.Length + 1) * Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY))));
//                NativeMethods.SERVICE_TABLE_ENTRY[] serviceTableEntryArray = new NativeMethods.SERVICE_TABLE_ENTRY[services.Length];
//                bool multipleServices = services.Length > 1;
//                IntPtr num = (IntPtr)0;
//                for (int index = 0; index < services.Length; ++index)
//                {
//                    services[index].Initialize(multipleServices);
//                    serviceTableEntryArray[index] = services[index].GetEntry();
//                    IntPtr ptr = (IntPtr)((long)entry + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY)) * index));
//                    Marshal.StructureToPtr((object)serviceTableEntryArray[index], ptr, true);
//                }
//                NativeMethods.SERVICE_TABLE_ENTRY serviceTableEntry = new NativeMethods.SERVICE_TABLE_ENTRY();
//                serviceTableEntry.callback = (Delegate)null;
//                serviceTableEntry.name = (IntPtr)0;
//                IntPtr ptr1 = (IntPtr)((long)entry + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY)) * services.Length));
//                Marshal.StructureToPtr((object)serviceTableEntry, ptr1, true);
//                bool flag = NativeMethods.StartServiceCtrlDispatcher(entry);
//                string str = "";
//                if (!flag)
//                {
//                    str = new Win32Exception().Message;
//                    string string1 = "CantStartFromCommandLine";
//                    if (Environment.UserInteractive)
//                    {
//                        string string2 = "CantStartFromCommandLineTitle"; 
//                    }
//                    else
//                        Console.WriteLine(string1);
//                }
//                foreach (ServiceBase serviceBase in services)
//                {
//                    serviceBase.Dispose();
//                    if (!flag && serviceBase.EventLog.Source.Length != 0)
//                       WriteEventLogEntry("StartFailed", new object[1]
//            {
//              (object) str
//            }, EventLogEntryType.Error);
//                }
//            }
//        }

//        private void WriteEventLogEntry(string message)
//        {
//            try
//            {
//                if (!this.AutoLog)
//                    return;
//                this.EventLog.WriteEntry(message);
//            }
//            catch (StackOverflowException ex)
//            {
//                throw;
//            }
//            catch (OutOfMemoryException ex)
//            {
//                throw;
//            }
//            catch (ThreadAbortException ex)
//            {
//                throw;
//            }
//            catch
//            {
//            }
//        }
//
//        private void WriteEventLogEntry(string message, EventLogEntryType errorType)
//        {
//            try
//            {
//                if (!this.AutoLog)
//                    return;
//                this.EventLog.WriteEntry(message, errorType);
//            }
//            catch (StackOverflowException ex)
//            {
//                throw;
//            }
//            catch (OutOfMemoryException ex)
//            {
//                throw;
//            }
//            catch (ThreadAbortException ex)
//            {
//                throw;
//            }
//            catch
//            {
//            }
//        }
//        private int ServiceCommandCallbackEx(int command, int eventType, IntPtr eventData, IntPtr eventContext)
//        {
//            int num = 0;
//            switch (command)
//            {
//                case 13:
//                    new DeferredHandlerDelegateAdvanced(this.DeferredPowerEvent).BeginInvoke(eventType, eventData, (AsyncCallback)null, (object)null);
//                    break;
//                case 14:
//                    DeferredHandlerDelegateAdvancedSession delegateAdvancedSession = new DeferredHandlerDelegateAdvancedSession(this.DeferredSessionChange);
//                    NativeMethods.WTSSESSION_NOTIFICATION wtssessionNotification = new NativeMethods.WTSSESSION_NOTIFICATION();
//                    Marshal.PtrToStructure(eventData, (object)wtssessionNotification);
//                    delegateAdvancedSession.BeginInvoke(eventType, wtssessionNotification.sessionId, (AsyncCallback)null, (object)null);
//                    break;
//                default:
//                    this.ServiceCommandCallback(command);
//                    break;
//            }
//            return num;
//        }
//
// 
//
//        private void OnSessionChange(SessionChangeDescription sessionChangeDescription)
//        {
//            throw new NotImplementedException();
//        }

//        private void DeferredPowerEvent(int eventtype, IntPtr eventdata)
//        {
//            throw new NotImplementedException();
//        }
//
//        private unsafe void ServiceMainCallback()
//        {
//            
//        }
//        private unsafe void ServiceCommandCallback(int command)
//        {
//            fixed (NativeMethods.SERVICE_STATUS* status = &this.status)
//            {
//                if (command == 4)
//                    NativeMethods.SetServiceStatus(this.statusHandle, status);
//                else if (this.status.currentState != 5 && this.status.currentState != 2 && (this.status.currentState != 3 && this.status.currentState != 6))
//                {
//                    switch (command)
//                    {
//                        case 1:
//                            int num = this.status.currentState;
//                            if (this.status.currentState == 7 || this.status.currentState == 4)
//                            {
//                                this.status.currentState = 3;
//                                NativeMethods.SetServiceStatus(this.statusHandle, status);
//                                this.status.currentState = num;
//                                new DeferredHandlerDelegate(this.DeferredStop).BeginInvoke((AsyncCallback)null, (object)null);
//                                break;
//                            }
//                            break;
//                        case 2:
//                            if (this.status.currentState == 4)
//                            {
//                                this.status.currentState = 6;
//                                NativeMethods.SetServiceStatus(this.statusHandle, status);
//                                new DeferredHandlerDelegate(DeferredPause).BeginInvoke((AsyncCallback)null, (object)null);
//                                break;
//                            }
//                            break;
//                        case 3:
//                            if (this.status.currentState == 7)
//                            {
//                                this.status.currentState = 5;
//                                NativeMethods.SetServiceStatus(this.statusHandle, status);
//                                new DeferredHandlerDelegate(this.DeferredContinue).BeginInvoke((AsyncCallback)null, (object)null);
//                                break;
//                            }
//                            break;
//                        case 5:
//                            new DeferredHandlerDelegate(this.DeferredShutdown).BeginInvoke((AsyncCallback)null, (object)null);
//                            break;
//                        default:
//                            new DeferredHandlerDelegateCommand(this.DeferredCustomCommand).BeginInvoke(command, (AsyncCallback)null, (object)null);
//                            break;
//                    }
//                }
//            }
//        }

//        private void DeferredCustomCommand(int command)
//        {
//            throw new NotImplementedException();
//        }
//
//        private void DeferredShutdown()
//        {
//            throw new NotImplementedException();
//        }
//
//        private void DeferredContinue()
//        {
//            throw new NotImplementedException();
//        }
//
//        private void DeferredPause()
//        {
//            throw new NotImplementedException();
//        }
//
//        private void DeferredStop()
//        {
//            throw new NotImplementedException();
//        }
//
//        private static void WriteEventLogEntry(string startfailed, object[] objects, EventLogEntryType error)
//        {
//            throw new NotImplementedException();
//        }
//
//
//        private delegate void DeferredHandlerDelegate();
//
//        private delegate void DeferredHandlerDelegateCommand(int command);
//
//        private delegate void DeferredHandlerDelegateAdvanced(int eventType, IntPtr eventData);
//
//        private delegate void DeferredHandlerDelegateAdvancedSession(int eventType, int sessionId);
      
    }
}