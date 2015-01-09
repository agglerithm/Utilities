using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Utilities
{
    public interface  IProcessCollection
    {
        Process First(); 
        Process Next();
    }


        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        } ;

    public class ProcessCollection : IProcessCollection
    {
        public static uint TH32CS_SNAPPROCESS = 2;
        private IntPtr _handle;
        private PROCESSENTRY32 _piCursor;

        public ProcessCollection()
        { 
            _handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            _piCursor = new PROCESSENTRY32
            {
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
            };
            if(First() == null) throw new ProcessInfoException("No processes were found!");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);
         
        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);


        public Process First()
        {
            if (_handle == IntPtr.Zero)
                return null;

            
            return Process32First(_handle, ref _piCursor) == false ? null : Process.GetProcessById((int) _piCursor.th32ProcessID);
        }

        public Process Next()
        {
            return Process32Next(_handle, ref _piCursor) == false ? null : Process.GetProcessById((int)_piCursor.th32ProcessID);
        }

        public static IEnumerable<Process> GetProcessesByName(string procName)
        { 
            var procs = new ProcessCollection();
            Process nextProc = procs.First();
            if (NameMatches(nextProc,procName))
                yield return nextProc;
            do
            {
                nextProc = procs.Next();
                if (NameMatches(nextProc, procName)) 
                    yield return nextProc;

            } while (nextProc != null); 
        }
        public static PROCESSENTRY32 GetInfo(Process proc)
        { 
            var procs = new ProcessCollection();
            Process nextProc = null;
            if (InfoMatches(proc, procs._piCursor))
                return procs._piCursor;
            do
            {
                nextProc = procs.Next();

            } while (nextProc != null && !InfoMatches(proc, procs._piCursor));

            return procs._piCursor;
        }

        private static bool InfoMatches(Process proc, PROCESSENTRY32 processInfo)
        {
            return proc.Id == processInfo.th32ProcessID;
        }


        private static bool NameMatches(Process proc, string name)
        {
            if (proc == null) return false;
            return proc.ProcessName == name;
        }
    }

    public class ProcessInfoException : Exception
    {
        public ProcessInfoException(string msg):base(msg)
        { 
        }
    }
}