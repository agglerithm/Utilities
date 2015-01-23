using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Rebex.Net;

namespace Utilities.IO
{
    public class FilePush
    {

        private Sftp sftpClient = null;
        public SFTPConfig sftpConfig = null;

        private const string c_appName = "PMFileDeliveryService"; 

#if (false)
		public FilePush() : this("localhost", "UserCertFile", "KeyPassword", "UserName", "UserPass", "sourceFile.txt", "destFile.txt", null, null ){}
		public FilePush(string site, string username, string password,string source, string dest, string rename, string flagfile)
			: this(site, null, null, username, password, source, dest, rename, flagfile) {}
		public FilePush(string site, FileInfo userPublicKey, string keyPassword, string username, string source, string dest, string rename, string flagfile)
			: this(site, userPublicKey.FullName, keyPassword, username, null, source, dest, rename, flagfile) {}
#endif

        public FilePush(string site, string keyPath, string keyPass, string user, string pass, string sourcefile, string destfile, string rename, string flagfile, int delay)
        { 

            sftpConfig = new SFTPConfig(site, keyPath, keyPass, user, pass, sourcefile, destfile, rename, flagfile, delay);
            sftpClient = new Sftp();
            sftpClient.CommandSent += new SftpCommandSentEventHandler(client_CommandSent);
            sftpClient.ResponseRead += new SftpResponseReadEventHandler(client_ResponseRead);

            sftpConfig.port = Sftp.DefaultPort;
            if (sftpConfig.destination.Contains(":"))
            {
                sftpConfig.port = Convert.ToInt16(sftpConfig.destination.Substring(sftpConfig.destination.IndexOf(":") + 1));
                sftpConfig.destination = sftpConfig.destination.Substring(0, sftpConfig.destination.IndexOf(":"));
            }
        }

        /// <summary>
        /// Initialize FilePush with sftpConfig data - currently unused
        /// </summary>
        /// <param name="sftpconfig"></param>
        public FilePush(SFTPConfig sftpconfig, string applicationName, string logConfigFilePath)
        {
            sftpConfig = sftpconfig;

            sftpClient = new Sftp();
            sftpClient.CommandSent += new SftpCommandSentEventHandler(client_CommandSent);
            sftpClient.ResponseRead += new SftpResponseReadEventHandler(client_ResponseRead);
             
            sftpConfig.port = Sftp.DefaultPort;
            if (sftpConfig.destination.Contains(":"))
            {
                sftpConfig.port = Convert.ToInt16(sftpConfig.destination.Substring(sftpConfig.destination.IndexOf(":") + 1));
                sftpConfig.destination = sftpConfig.destination.Substring(0, sftpConfig.destination.IndexOf(":"));
            }

        }



#if (false)
    /// <summary>
    /// Debug the settings being used for SFTP connections
    /// *debug for initial setup
    /// </summary>
    /// <returns></returns>
		public string debugFilePushSettings() { return sftpConfig.debugSFTPConfigSettings(); }
#endif

        /// <summary>
        /// Check that we have enough information to merit an connection attempt, that the file exists and we have enough information to try the connection
        /// </summary>
        /// <returns>true if it merits a connection attempt</returns>
        public Boolean isReady()
        {
            Boolean foundEverything = false;
            int maxAttempts = 10;  // times to try file for locking

            try
            {
                // make sure we can find the sample files for the unit test
                FileInfo fi = new FileInfo(sftpConfig.sourceFile);

                if (fi.Exists)
                {
                    if (!string.IsNullOrWhiteSpace(sftpConfig.privateKeyPath))
                    {
                        fi = new FileInfo(sftpConfig.privateKeyPath);
                        if (fi.Exists)
                        {
                            foundEverything = true;
                        }
                        else
                        { 
                        }
                    }
                    else
                    {
                        // no private key, should just be username and password
                        if (!string.IsNullOrWhiteSpace(sftpConfig.passWord))
                        {
                            foundEverything = true;
                        }
                        else
                        { 
                        }
                    }
                }
                else
                { 
                }
            }
            catch (ApplicationException)
            { 
            }
            catch (Exception e)
            { 
            }
            return foundEverything;
        }

        /// <summary>
        /// IsFileLocked - checks if a file is locked, called by RetryHelper that tries 10, times doubling the back off of 100ms each time.
        /// sometimes the garbage collection doesn't run and delivery tries to deliver it before the connections are flushed. 
        /// </summary>
        /// <param name="file">file to be checked for exclusive access</param>
        /// <returns>false if file is not locked, throws if it is locked used by RetryHelper to try and wait a little bit of time</returns>
        protected bool IsFileLocked(FileInfo file)
        {
#if (false)
			FileStream stream = null;

            try {
				stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException ioException) {
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				mLogger.Log(DeliveryLogEvents.leRetryingBackingOff, ioException.Message);
				throw;
			}
			finally {
				if (stream != null)
					stream.Close();
			}
#endif
            // file is not locked
            return false;
        }

        /// <summary>
        /// Send the file right now
        /// </summary>
        /// <returns>true on success</returns>
        public Boolean SendFile()
        {
            bool status = false;
            sftpClient.Connect(sftpConfig.destination, sftpConfig.port);
            var fileProc = new MultiThreadFileProc<AdHocRecord>(new AdHocRecordBlockSpecs());
            // figure out how we are supposed to connect
            // username/password combination
            if (!string.IsNullOrWhiteSpace(sftpConfig.passWord))
            {
                sftpClient.Login(sftpConfig.userName, sftpConfig.passWord);
            }
            else
            {
                // username / public key combination
                if (!string.IsNullOrWhiteSpace(sftpConfig.privateKeyPath))
                {
                    try
                    {
                        SshPrivateKey privateKey = new SshPrivateKey(sftpConfig.privateKeyPath, sftpConfig.privateKeyPass); 
                        sftpClient.Login(sftpConfig.userName, privateKey); 
                    }
                    catch (Exception e)
                    { 
                    }
                }
            }

            FileInfo fi = new FileInfo(sftpConfig.sourceFile);
            Stream sourceStream = null;
            long bytesTransfered = -1;
            try
            {


                fileProc.Copy(readStream, writeStream,fi.Length);
                bytesTransfered = sftpClient.PutFile(sourceStream, sftpConfig.destFile);
            }
            finally
            {
                if (sourceStream != null)
                {
                    try { sourceStream.Close(); }
                    catch (Exception) { }
                    try { sourceStream.Dispose(); }
                    catch (Exception) { }
                }
            }
            if (bytesTransfered != fi.Length)
            { 
            }
            else
            {
                // transfer successful 
                status = true;
                // should we rename the sent file?
                if (!string.IsNullOrWhiteSpace(sftpConfig.renameExt))
                {
                    Delay(sftpConfig.delay);
                    if (!sftpClient.FileExists(sftpConfig.renameExt))
                    {
                        Delay(sftpConfig.delay);
                        sftpClient.Rename(sftpConfig.destFile, sftpConfig.renameExt);
                        Delay(sftpConfig.delay);
                        if (!sftpClient.FileExists(sftpConfig.renameExt))
                        { 
                            status = false;
                        }
                    }
                    else
                    { 
                        status = false;
                    }
                }
                // should we create a flag file?
                if (!string.IsNullOrWhiteSpace(sftpConfig.flagFile))
                {
                    Delay(sftpConfig.delay);
                    sftpClient.PutFile(new MemoryStream(), sftpConfig.flagFile);  // write a zero byte file
                    Delay(sftpConfig.delay);
                    if (!sftpClient.FileExists(sftpConfig.flagFile))
                    { 
                        status = false;
                    }
                }

            }
            return status;
        }

        private void writeStream(Stream readStream, int offSet, int len)
        {
            sftpClient.PutFile(readStream,sftpConfig.destFile,offSet,len);
        }

        private Stream readStream()
        {
            try
            {
                return File.Open(sftpConfig.sourceFile, FileMode.Open, FileAccess.Read,
                    FileShare.Read | FileShare.Inheritable | FileShare.ReadWrite);
            }
            catch (IOException ioException)
            {
                // The file is unavailable because it is: still being written to or being processed by another thread or does not exist (has already been processed) 
                throw ioException;
            }
        }

        private void client_CommandSent(object sender, SftpCommandSentEventArgs e)
        {
            //Console.WriteLine("Command: {0}", e.Command); 
        }

        private void client_ResponseRead(object sender, SftpResponseReadEventArgs e)
        {
            //Console.WriteLine("Response: {0}", e.Response); 
        }

        private void Delay(int milliseconds)
        {
            using (System.Threading.EventWaitHandle tmpEvent = new System.Threading.ManualResetEvent(false))
            {
                tmpEvent.WaitOne(TimeSpan.FromMilliseconds(Math.Max(1, Math.Min(milliseconds, 60 * 10000))));
            }
        }

 
    }
}