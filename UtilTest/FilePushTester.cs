using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.IO;

namespace UtilTest
{
    [TestClass]
    public class FilePushTester
    {
        private string basePath = @"C:\Users\jreese\My Documents\GitHub\Utilities\UtilTest\";
        private string ukFolder;
        private string tdFolder;
        // sftp destination
        private string CerberusHost = "localhost";
        // folder of private user keys
        private string UserKeysFolder = "../../../UtilTest/testdata/userKeys/";
        // folder of test data for transfers
        private string TestDataFolder = "../../../UtilTest/testdata/";
        // installation root of cerberus where accounts have home directories
        private string CerberusRoot = @"C:\Users\jreese\My Documents\GitHub\Utilities\UtilTest\testdata\cerberusRoot\";
        // directory on account in whichto create files
        private string CerberusPath = "/cerberusRoot/";

        [TestInitialize]
        public void setup()
        {
            ukFolder = basePath + @"testdata\userKeys\";
            tdFolder = basePath + @"testdata\";
        }
        [TestMethod()]
        public void FilePushAndCheck()
        {
            var userPrivateKey = String.Format("{0}{1}", UserKeysFolder, "testUserSFTPPutty.ppk");
            string userPrivateKeyPassword = "password";
            string destFileName = "blah.mp4";
            string username = "UserPublicKey";
            string source = String.Format("{0}{1}", TestDataFolder, "jumpers.mp4");
            string remoteDest = String.Format("{0}{1}", CerberusPath, destFileName);
            string transferedfile = String.Format("{0}{1}", CerberusRoot, destFileName);
             

            FilePush target = new FilePush(CerberusHost, userPrivateKey, userPrivateKeyPassword, username, "", source,
                remoteDest, null, null, 2000);
            Assert.IsTrue(target.SendFile());
        }
    }
}