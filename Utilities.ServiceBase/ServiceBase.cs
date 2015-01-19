using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Utilities.WinMockups
{
    public abstract class ServiceBase:IDisposable
    {     
        private NativeMethods.SERVICE_STATUS status = new NativeMethods.SERVICE_STATUS();
        public const int MaxNameLength = 80;
        private IntPtr statusHandle;
        private NativeMethods.ServiceControlCallback commandCallback;
        private NativeMethods.ServiceControlCallbackEx commandCallbackEx;
        private NativeMethods.ServiceMainCallback mainCallback;
        private IntPtr handleName;  
        private readonly EventLog eventLog = new EventLog();
        private bool nameFrozen; 
        private bool initialized;
        private bool disposed;


        protected abstract void OnStart(string[] args);

        protected abstract void OnStop();

 

        public string ServiceDescription { get; set; } 

        private void Initialize(bool multipleServices)
        {
            if (this.initialized)
                return;
            if (this.disposed)
                throw new ObjectDisposedException(this.GetType().Name);
            this.status.serviceType = multipleServices ? 32 : 16;
            this.status.currentState = 2;
            this.status.controlsAccepted = 0;
            this.status.win32ExitCode = 0;
            this.status.serviceSpecificExitCode = 0;
            this.status.checkPoint = 0;
            this.status.waitHint = 0;
            this.mainCallback = new NativeMethods.ServiceMainCallback(this.ServiceMainCallback);
            this.commandCallback = new NativeMethods.ServiceControlCallback(this.ServiceCommandCallback);
            this.commandCallbackEx = new NativeMethods.ServiceControlCallbackEx(this.ServiceCommandCallbackEx);
            this.handleName = Marshal.StringToHGlobalUni(this.ServiceName);
            this.initialized = true;
        }

        public string ServiceName { get; set; }

        private void ServiceMainCallback(int argcount, IntPtr argpointer)
        {
            throw new NotImplementedException();
        }

        private NativeMethods.SERVICE_TABLE_ENTRY GetEntry()
        {
            NativeMethods.SERVICE_TABLE_ENTRY serviceTableEntry = new NativeMethods.SERVICE_TABLE_ENTRY();
            this.nameFrozen = true;
            serviceTableEntry.callback = (Delegate)this.mainCallback;
            serviceTableEntry.name = this.handleName;
            return serviceTableEntry;
        }
        protected void Run()
        {

            IntPtr entry = Marshal.AllocHGlobal((IntPtr)(2 * Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY))));
            NativeMethods.SERVICE_TABLE_ENTRY serviceTableEntry1 = GetEntry(); 
            IntPtr num = (IntPtr)0; 
            Initialize(false); 
            IntPtr ptr = (IntPtr)((long)entry + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY))));
            Marshal.StructureToPtr((object)serviceTableEntry1, ptr, true); 
            NativeMethods.SERVICE_TABLE_ENTRY serviceTableEntry = new NativeMethods.SERVICE_TABLE_ENTRY();
            serviceTableEntry.callback = (Delegate)null;
            serviceTableEntry.name = (IntPtr)0;
            IntPtr ptr1 = (IntPtr)((long)entry + (long)(Marshal.SizeOf(typeof(NativeMethods.SERVICE_TABLE_ENTRY))));
            Marshal.StructureToPtr((object)serviceTableEntry, ptr1, true);
            bool flag = NativeMethods.StartServiceCtrlDispatcher(entry);
            string str = "";
            if (!flag)
            {
                str = new Win32Exception().Message;
                string string1 = "CantStartFromCommandLine";
                if (Environment.UserInteractive)
                {
                    string string2 = "CantStartFromCommandLineTitle";
                }
                else
                    Console.WriteLine(string1);
            } 
            if (!flag && EventLog.Source.Length != 0)
                WriteEventLogEntry("StartFailed", EventLogEntryType.Error);
            Dispose();
        }
 

        public EventLog EventLog { get { return eventLog; } }

        public bool AutoLog { get; private set; }

 
        private int ServiceCommandCallbackEx(int command, int eventType, IntPtr eventData, IntPtr eventContext)
        {
            int num = 0;
            switch (command)
            {
                case 13:
                    new DeferredHandlerDelegateAdvanced(this.DeferredPowerEvent).BeginInvoke(eventType, eventData, (AsyncCallback)null, (object)null);
                    break;
                case 14:
                    DeferredHandlerDelegateAdvancedSession delegateAdvancedSession = new DeferredHandlerDelegateAdvancedSession(this.DeferredSessionChange);
                    NativeMethods.WTSSESSION_NOTIFICATION wtssessionNotification = new NativeMethods.WTSSESSION_NOTIFICATION();
                    Marshal.PtrToStructure(eventData, (object)wtssessionNotification);
                    delegateAdvancedSession.BeginInvoke(eventType, wtssessionNotification.sessionId, (AsyncCallback)null, (object)null);
                    break;
                default:
                    this.ServiceCommandCallback(command);
                    break;
            }
            return num;
        }

        private void DeferredSessionChange(int eventtype, int sessionid)
        {
            try
            {
                this.OnSessionChange(new SessionChangeDescription((SessionChangeReason)eventtype, sessionid));
            }
            catch (Exception ex)
            {
                this.WriteEventLogEntry("SessionChangeFailed");
                throw;
            }
        }

        private void OnSessionChange(SessionChangeDescription sessionChangeDescription)
        {
            throw new NotImplementedException();
        }

        private void DeferredPowerEvent(int eventtype, IntPtr eventdata)
        {
            throw new NotImplementedException();
        }

        private unsafe void ServiceCommandCallback(int command)
        {
            fixed (NativeMethods.SERVICE_STATUS* status = &this.status)
            {
                if (command == 4)
                    NativeMethods.SetServiceStatus(this.statusHandle, status);
                else if (this.status.currentState != 5 && this.status.currentState != 2 && (this.status.currentState != 3 && this.status.currentState != 6))
                {
                    switch (command)
                    {
                        case 1:
                            int num = this.status.currentState;
                            if (this.status.currentState == 7 || this.status.currentState == 4)
                            {
                                this.status.currentState = 3;
                                NativeMethods.SetServiceStatus(this.statusHandle, status);
                                this.status.currentState = num;
                                new DeferredHandlerDelegate(this.DeferredStop).BeginInvoke((AsyncCallback)null, (object)null);
                                break;
                            }
                            break;
                        case 2:
                            if (this.status.currentState == 4)
                            {
                                this.status.currentState = 6;
                                NativeMethods.SetServiceStatus(this.statusHandle, status);
                                new DeferredHandlerDelegate(DeferredPause).BeginInvoke((AsyncCallback)null, (object)null);
                                break;
                            }
                            break;
                        case 3:
                            if (this.status.currentState == 7)
                            {
                                this.status.currentState = 5;
                                NativeMethods.SetServiceStatus(this.statusHandle, status);
                                new DeferredHandlerDelegate(this.DeferredContinue).BeginInvoke((AsyncCallback)null, (object)null);
                                break;
                            }
                            break;
                        case 5:
                            new DeferredHandlerDelegate(this.DeferredShutdown).BeginInvoke((AsyncCallback)null, (object)null);
                            break;
                        default:
                            new DeferredHandlerDelegateCommand(this.DeferredCustomCommand).BeginInvoke(command, (AsyncCallback)null, (object)null);
                            break;
                    }
                }
            }
        }

        private void DeferredCustomCommand(int command)
        {
            throw new NotImplementedException();
        }

        private void DeferredShutdown()
        {
            throw new NotImplementedException();
        }

        private void DeferredContinue()
        {
            throw new NotImplementedException();
        }

        private void DeferredPause()
        {
            throw new NotImplementedException();
        }

        private void DeferredStop()
        {
            throw new NotImplementedException();
        }

 
        private void WriteEventLogEntry(string message)
        {
            try
            {
                if (!this.AutoLog)
                    return;
                this.EventLog.WriteEntry(message);
            }
            catch (StackOverflowException ex)
            {
                throw;
            }
            catch (OutOfMemoryException ex)
            {
                throw;
            }
            catch (ThreadAbortException ex)
            {
                throw;
            }
            catch
            {
            }
        }

        private void WriteEventLogEntry(string message, EventLogEntryType errorType)
        {
            try
            {
                if (!this.AutoLog)
                    return;
                this.EventLog.WriteEntry(message, errorType);
            }
            catch (StackOverflowException ex)
            {
                throw;
            }
            catch (OutOfMemoryException ex)
            {
                throw;
            }
            catch (ThreadAbortException ex)
            {
                throw;
            }
            catch
            {
            }
        }

        private delegate void DeferredHandlerDelegate();

        private delegate void DeferredHandlerDelegateCommand(int command);

        private delegate void DeferredHandlerDelegateAdvanced(int eventType, IntPtr eventData);

        private delegate void DeferredHandlerDelegateAdvancedSession(int eventType, int sessionId);

        public void Dispose()
        {
            disposed = true;
        }
    }

    internal enum SessionChangeReason
    {
    }

    internal class SessionChangeDescription
    {
        public SessionChangeDescription(SessionChangeReason eventtype, int sessionid)
        {
            throw new NotImplementedException();
        }
    }
}