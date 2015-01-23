namespace Utilities.IO
{
    public class SFTPConfig
    {
        public string destination { get; set; }              // Location where to deliver the files
        public string sourceFile { get; set; }               // source file to be delivered
        public string destFile { get; set; }                 // destination file path and name on remote server
        public string userName { get; set; }                 // Log in username at destination
        public string passWord { get; set; }                 // Password for log in at destination
        public string privateKeyPath { get; set; }           // Location of private key UserPrivateKey.pem
        public string privateKeyPass { get; set; }           // Private key password 
        public string renameExt { get; set; }                // do we need to rename the file after pushing?     // null = no
        public string flagFile { get; set; }                 // do we need to create a flag file after pushing?  // null = no
        public int port { get; set; }                        // port number defaults to 22 per SSH standard
        public int delay { get; set; }                       // milliseconds to delay between commands to remote server

        public SFTPConfig(string site, string keyPath, string keyPass, string user, string pass,
            string sourcefile, string destfile, string rename, string flagfile, int delay)
        {
            this.destination = site;             // destination - where we are connecting too
            this.sourceFile = sourcefile;        // file that we are sending
            this.destFile = destfile;            // destination file at remote site including path if necessary
            this.userName = user;                // username for authentication at remote site
            this.passWord = pass;                // passowrd for the username at remote site
            this.privateKeyPath = keyPath;       // user public key - log in mechanism at remote site
            this.privateKeyPass = keyPass;       // private key password - log in mechanism at remote site
            this.renameExt = rename;             // filename to rename after push
            this.flagFile = flagfile;            // create a flag file after push
            this.delay = delay;                  // milliseconds to delay between commands to remote server
        }

#if (false)
		public SFTPConfig() {}

		public string debugSFTPConfigSettings()
        {
			//*debug for initial setup
			StringBuilder sb = new StringBuilder();
			sb.Append("\n\n********** REMOVE ME ********\n");
			sb.AppendFormat("destination   : {0}\n", destination);
			sb.AppendFormat("privateKeyPath   : {0}\n", privateKeyPath);
			sb.AppendFormat("privateKeyPass   : {0}\n", (string.IsNullOrWhiteSpace(privateKeyPass) == false ? privateKeyPass.Substring(0, 1) : "emptystring"));
			sb.AppendFormat("userName   : {0}\n", userName);
			sb.AppendFormat("passWord   : {0}\n", (string.IsNullOrWhiteSpace(passWord) == false ? passWord.Substring(0, 1) : "emptystring"));
			sb.AppendFormat("sourceFile   : {0}\n", sourceFile);
			sb.AppendFormat("destFile   : {0}\n", destFile);
			sb.AppendFormat("renameExt   : {0}\n", renameExt);
			sb.AppendFormat("flagFile   : {0}\n", flagFile);
			sb.AppendFormat("port   : {0}\n", port);
			return (sb.ToString());
		}
#endif
    }
}