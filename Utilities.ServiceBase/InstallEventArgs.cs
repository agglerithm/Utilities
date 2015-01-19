using System;
using System.Collections;

namespace Utilities.WinMockups
{
    public class InstallEventArgs : EventArgs
    {
        private IDictionary savedState;

        public IDictionary SavedState
        {
            get
            {
                return this.savedState;
            }
        }

        public InstallEventArgs()
        {
        }

        public InstallEventArgs(IDictionary savedState)
        {
            this.savedState = savedState;
        }
    }
}