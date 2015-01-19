using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace $rootnamespace$.Extensions
{
    public static class ProcessEx  
    {
        public static Process GetParent(this Process child)
        { 
            int parentId = 0;
            if (child == null)
                throw new ArgumentNullException("child");

            var procs = new ProcessCollection();

            var currentProc = procs.First();

            if (currentProc == null) return null;
                do
                {
                    if (child.Id == currentProc.Id)
                        parentId = (int) currentProc.Id;
                    currentProc = procs.Next();
                }
                while (parentId == 0);

                return parentId > 0 ? Process.GetProcessById(parentId) : null;
        }
         
        public static bool IsRunningAsAService(this Process proc)
        { 
                try
                {
                    Process process = GetParent(proc);
                    if (process != null && process.ProcessName == "services")
                    { 
                        return true;
                    }
                }
                catch (InvalidOperationException)
                { 
                }
                return false; 
        }

        public static IEnumerable<string> Arguments(this Process proc)
        {
            return Environment.GetCommandLineArgs(); 
        }

    }
}
