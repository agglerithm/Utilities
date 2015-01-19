using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace Utilities.WinMockups
{
    public delegate void InstallEventHandler(object sender, InstallEventArgs e);

    [DefaultEvent("AfterInstall")]
    public class Installer : IDisposable
    {
        private const string wrappedExceptionSource = "WrappedExceptionSource";
        private InstallerCollection installers;
        private InstallContext context;
        internal Installer parent;
        private InstallEventHandler afterCommitHandler;
        private InstallEventHandler afterInstallHandler;
        private InstallEventHandler afterRollbackHandler;
        private InstallEventHandler afterUninstallHandler;
        private InstallEventHandler beforeCommitHandler;
        private InstallEventHandler beforeInstallHandler;
        private InstallEventHandler beforeRollbackHandler;
        private InstallEventHandler beforeUninstallHandler;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public InstallContext Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = value;
            }
        }

 

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InstallerCollection Installers
        {
            get
            {
                if (this.installers == null)
                    this.installers = new InstallerCollection(this);
                return this.installers;
            }
        }

        [TypeConverter(typeof(InstallerParentConverter))]
        [Browsable(true)] 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Installer Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (value == this)
                    throw new InvalidOperationException("InstallBadParent");
                if (value == this.parent)
                    return;
                if (value != null && this.InstallerTreeContains(value))
                    throw new InvalidOperationException("InstallRecursiveParent");
                if (this.parent != null)
                {
                    int index = this.parent.Installers.IndexOf(this);
                    if (index != -1)
                        this.parent.Installers.RemoveAt(index);
                }
                this.parent = value;
                if (this.parent == null || this.parent.Installers.Contains(this))
                    return;
                this.parent.Installers.Add(this);
            }
        }

        public event InstallEventHandler Committed
        {
            add
            {
                this.afterCommitHandler += value;
            }
            remove
            {
                this.afterCommitHandler -= value;
            }
        }

        public event InstallEventHandler AfterInstall
        {
            add
            {
                this.afterInstallHandler += value;
            }
            remove
            {
                this.afterInstallHandler -= value;
            }
        }

        public event InstallEventHandler AfterRollback
        {
            add
            {
                this.afterRollbackHandler += value;
            }
            remove
            {
                this.afterRollbackHandler -= value;
            }
        }

        public event InstallEventHandler AfterUninstall
        {
            add
            {
                this.afterUninstallHandler += value;
            }
            remove
            {
                this.afterUninstallHandler -= value;
            }
        }

        public event InstallEventHandler Committing
        {
            add
            {
                this.beforeCommitHandler += value;
            }
            remove
            {
                this.beforeCommitHandler -= value;
            }
        }

        public event InstallEventHandler BeforeInstall
        {
            add
            {
                this.beforeInstallHandler += value;
            }
            remove
            {
                this.beforeInstallHandler -= value;
            }
        }

        public event InstallEventHandler BeforeRollback
        {
            add
            {
                this.beforeRollbackHandler += value;
            }
            remove
            {
                this.beforeRollbackHandler -= value;
            }
        }

        public event InstallEventHandler BeforeUninstall
        {
            add
            {
                this.beforeUninstallHandler += value;
            }
            remove
            {
                this.beforeUninstallHandler -= value;
            }
        }

        internal bool InstallerTreeContains(Installer target)
        {
            if (this.Installers.Contains(target))
                return true;
            foreach (Installer installer in (CollectionBase)this.Installers)
            {
                if (installer.InstallerTreeContains(target))
                    return true;
            }
            return false;
        }

        public virtual void Commit(IDictionary savedState)
        {
            if (savedState == null)
                throw new ArgumentException("InstallNullParameter");
            if (savedState[(object)"_reserved_lastInstallerAttempted"] == null || savedState[(object)"_reserved_nestedSavedStates"] == null)
                throw new ArgumentException("InstallDictionaryMissingValues");
            Exception exception1 = (Exception)null;
            try
            {
                this.OnCommitting(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnCommitting", ex);
                this.Context.LogMessage("InstallCommitException");
                exception1 = ex;
            }
            int num = (int)savedState[(object)"_reserved_lastInstallerAttempted"];
            IDictionary[] dictionaryArray = (IDictionary[])savedState[(object)"_reserved_nestedSavedStates"];
            if (num + 1 != dictionaryArray.Length || num >= this.Installers.Count)
                throw new ArgumentException("InstallDictionaryCorrupted");
            for (int index = 0; index < this.Installers.Count; ++index)
                this.Installers[index].Context = this.Context;
            for (int index = 0; index <= num; ++index)
            {
                try
                {
                    this.Installers[index].Commit(dictionaryArray[index]);
                }
                catch (Exception ex)
                {
                    if (!this.IsWrappedException(ex))
                    {
                        this.Context.LogMessage("InstallLogCommitException");
                        Installer.LogException(ex, this.Context);
                        this.Context.LogMessage("InstallCommitException");
                    }
                    exception1 = ex;
                }
            }
            savedState[(object)"_reserved_nestedSavedStates"] = (object)dictionaryArray;
            savedState.Remove((object)"_reserved_lastInstallerAttempted");
            try
            {
                this.OnCommitted(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnCommitted", ex);
                this.Context.LogMessage("InstallCommitException");
                exception1 = ex;
            }
            if (exception1 != null)
            {
                Exception exception2 = exception1;
                if (!this.IsWrappedException(exception1))
                {
                    exception2 = (Exception)new InstallException("InstallCommitException", exception1);
                    exception2.Source = "WrappedExceptionSource";
                }
                throw exception2;
            }
        }

        public virtual void Install(IDictionary stateSaver)
        {
            if (stateSaver == null)
                throw new ArgumentException("InstallNullParameter");
            try
            {
                this.OnBeforeInstall(stateSaver);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityError", "OnBeforeInstall", ex);
                throw new InvalidOperationException("InstallEventException", ex);
            }
            int num = -1;
            ArrayList arrayList = new ArrayList();
            try
            {
                for (int index = 0; index < this.Installers.Count; ++index)
                    this.Installers[index].Context = this.Context;
                for (int index = 0; index < this.Installers.Count; ++index)
                {
                    Installer installer = this.Installers[index];
                    IDictionary stateSaver1 = (IDictionary)new Hashtable();
                    try
                    {
                        num = index;
                        installer.Install(stateSaver1);
                    }
                    finally
                    {
                        arrayList.Add((object)stateSaver1);
                    }
                }
            }
            finally
            {
                stateSaver.Add((object)"_reserved_lastInstallerAttempted", (object)num);
                stateSaver.Add((object)"_reserved_nestedSavedStates", (object)arrayList.ToArray(typeof(IDictionary)));
            }
            try
            {
                this.OnAfterInstall(stateSaver);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityError", "OnAfterInstall", ex);
                throw new InvalidOperationException("InstallEventException", ex);
            }
        }

        internal static void LogException(Exception e, InstallContext context)
        {
            bool flag = true;
            for (; e != null; e = e.InnerException)
            {
                if (flag)
                {
                    context.LogMessage(e.GetType().FullName + ": " + e.Message);
                    flag = false;
                }
                else
                    context.LogMessage("InstallLogInner");
                if (context.IsParameterTrue("showcallstack"))
                    context.LogMessage(e.StackTrace);
            }
        }

        private bool IsWrappedException(Exception e)
        {
            if (e is InstallException && e.Source == "WrappedExceptionSource")
                return e.TargetSite.ReflectedType == typeof(Installer);
            return false;
        }

        protected virtual void OnCommitted(IDictionary savedState)
        {
            if (this.afterCommitHandler == null)
                return;
            this.afterCommitHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnAfterInstall(IDictionary savedState)
        {
            if (this.afterInstallHandler == null)
                return;
            this.afterInstallHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnAfterRollback(IDictionary savedState)
        {
            if (this.afterRollbackHandler == null)
                return;
            this.afterRollbackHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnAfterUninstall(IDictionary savedState)
        {
            if (this.afterUninstallHandler == null)
                return;
            this.afterUninstallHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnCommitting(IDictionary savedState)
        {
            if (this.beforeCommitHandler == null)
                return;
            this.beforeCommitHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnBeforeInstall(IDictionary savedState)
        {
            if (this.beforeInstallHandler == null)
                return;
            this.beforeInstallHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnBeforeRollback(IDictionary savedState)
        {
            if (this.beforeRollbackHandler == null)
                return;
            this.beforeRollbackHandler((object)this, new InstallEventArgs(savedState));
        }

        protected virtual void OnBeforeUninstall(IDictionary savedState)
        {
            if (this.beforeUninstallHandler == null)
                return;
            this.beforeUninstallHandler((object)this, new InstallEventArgs(savedState));
        }

        public virtual void Rollback(IDictionary savedState)
        {
            if (savedState == null)
                throw new ArgumentException("InstallNullParameter");
            if (savedState[(object)"_reserved_lastInstallerAttempted"] == null || savedState[(object)"_reserved_nestedSavedStates"] == null)
                throw new ArgumentException("InstallDictionaryMissingValues");
            Exception exception1 = (Exception)null;
            try
            {
                this.OnBeforeRollback(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnBeforeRollback", ex);
                this.Context.LogMessage("InstallRollbackException");
                exception1 = ex;
            }
            int num = (int)savedState[(object)"_reserved_lastInstallerAttempted"];
            IDictionary[] dictionaryArray = (IDictionary[])savedState[(object)"_reserved_nestedSavedStates"];
            if (num + 1 != dictionaryArray.Length || num >= this.Installers.Count)
                throw new ArgumentException("InstallDictionaryCorrupted");
            for (int index = this.Installers.Count - 1; index >= 0; --index)
                this.Installers[index].Context = this.Context;
            for (int index = num; index >= 0; --index)
            {
                try
                {
                    this.Installers[index].Rollback(dictionaryArray[index]);
                }
                catch (Exception ex)
                {
                    if (!this.IsWrappedException(ex))
                    {
                        this.Context.LogMessage("InstallLogRollbackException");
                        Installer.LogException(ex, this.Context);
                        this.Context.LogMessage("InstallRollbackException");
                    }
                    exception1 = ex;
                }
            }
            try
            {
                this.OnAfterRollback(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnAfterRollback", ex);
                this.Context.LogMessage("InstallRollbackException");
                exception1 = ex;
            }
            if (exception1 != null)
            {
                Exception exception2 = exception1;
                if (!this.IsWrappedException(exception1))
                {
                    exception2 = (Exception)new InstallException("InstallRollbackException", exception1);
                    exception2.Source = "WrappedExceptionSource";
                }
                throw exception2;
            }
        }

        public virtual void Uninstall(IDictionary savedState)
        {
            Exception exception1 = (Exception)null;
            try
            {
                this.OnBeforeUninstall(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnBeforeUninstall", ex);
                this.Context.LogMessage("InstallUninstallException");
                exception1 = ex;
            }
            IDictionary[] dictionaryArray;
            if (savedState != null)
            {
                dictionaryArray = (IDictionary[])savedState[(object)"_reserved_nestedSavedStates"];
                if (dictionaryArray.Length != this.Installers.Count)
                    throw new ArgumentException("InstallDictionaryCorrupted");
            }
            else
                dictionaryArray = new IDictionary[this.Installers.Count];
            for (int index = this.Installers.Count - 1; index >= 0; --index)
                this.Installers[index].Context = this.Context;
            for (int index = this.Installers.Count - 1; index >= 0; --index)
            {
                try
                {
                    this.Installers[index].Uninstall(dictionaryArray[index]);
                }
                catch (Exception ex)
                {
                    if (!this.IsWrappedException(ex))
                    {
                        this.Context.LogMessage("InstallLogUninstallException");
                        Installer.LogException(ex, this.Context);
                        this.Context.LogMessage("InstallUninstallException");
                    }
                    exception1 = ex;
                }
            }
            try
            {
                this.OnAfterUninstall(savedState);
            }
            catch (Exception ex)
            {
                this.WriteEventHandlerError("InstallSeverityWarning", "OnAfterUninstall", ex);
                this.Context.LogMessage("InstallUninstallException");
                exception1 = ex;
            }
            if (exception1 != null)
            {
                Exception exception2 = exception1;
                if (!this.IsWrappedException(exception1))
                {
                    exception2 = (Exception)new InstallException("InstallUninstallException", exception1);
                    exception2.Source = "WrappedExceptionSource";
                }
                throw exception2;
            }
        }

        private void WriteEventHandlerError(string severity, string eventName, Exception e)
        {
            this.Context.LogMessage("InstallLogError");
            Installer.LogException(e, this.Context);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class InstallException : Exception
    {
        public InstallException(string msg, Exception ex):base(msg,ex)
        {
             
        }
    }
}