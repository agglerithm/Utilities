using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;

namespace Utilities.WinMockups
{
    public class InstallContext
    {
        private string logFilePath;
        private StringDictionary parameters;

        public StringDictionary Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public InstallContext()
            : this((string)null, (string[])null)
        {
        }

        public InstallContext(string logFilePath, string[] commandLine)
        {
            this.parameters = InstallContext.ParseCommandLine(commandLine);
            if (this.Parameters["logfile"] != null)
            {
                this.logFilePath = this.Parameters["logfile"];
            }
            else
            {
                if (logFilePath == null)
                    return;
                this.logFilePath = logFilePath;
                this.Parameters["logfile"] = logFilePath;
            }
        }

        public bool IsParameterTrue(string paramName)
        {
            string strA = this.Parameters[paramName.ToLower(CultureInfo.InvariantCulture)];
            if (strA == null)
                return false;
            if (string.Compare(strA, "true", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(strA, "yes", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(strA, "1", StringComparison.OrdinalIgnoreCase) != 0)
                return "".Equals(strA);
            return true;
        }

        public void LogMessage(string message)
        {
            this.logFilePath = this.Parameters["logfile"];
            if (this.logFilePath != null && !"".Equals(this.logFilePath))
            {
                StreamWriter streamWriter = (StreamWriter)null;
                try
                {
                    streamWriter = new StreamWriter(this.logFilePath, true, Encoding.UTF8);
                    streamWriter.WriteLine(message);
                }
                finally
                {
                    if (streamWriter != null)
                        streamWriter.Close();
                }
            }
            if (!this.IsParameterTrue("LogToConsole") && this.Parameters["logtoconsole"] != null)
                return;
            Console.WriteLine(message);
        }

        protected static StringDictionary ParseCommandLine(string[] args)
        {
            StringDictionary stringDictionary = new StringDictionary();
            if (args == null)
                return stringDictionary;
            for (int index = 0; index < args.Length; ++index)
            {
                if (args[index].StartsWith("/", StringComparison.Ordinal) || args[index].StartsWith("-", StringComparison.Ordinal))
                    args[index] = args[index].Substring(1);
                int length = args[index].IndexOf('=');
                if (length < 0)
                    stringDictionary[args[index].ToLower(CultureInfo.InvariantCulture)] = "";
                else
                    stringDictionary[args[index].Substring(0, length).ToLower(CultureInfo.InvariantCulture)] = args[index].Substring(length + 1);
            }
            return stringDictionary;
        }
    }
}