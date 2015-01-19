using System;
using Utilities.IO;

namespace UtilTest
{
    public class FileRecord:FileRecordBase
    {
        public string description { get; set; }
        public int id { get; set; }
        public string createDate { get; set; }
        private static int cnt;
        public string getRecord()
        {
            id = cnt++;
            return string.Format("{0}, {1}, {2}", "                                ", id.ToString().PadLeft(7, '0'), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"));
        }

        public FileRecord()
        {
            Delimiter = "\r\n";
            RecordSize = 66; 
        }

        public override void Parse(string line)
        {
            var arr = line.Split(',');
            if (arr.Length < 3) 
                return;
            try
            {
                description = arr[0];
                id = int.Parse (arr[1]);
                createDate = arr[2]; 
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}