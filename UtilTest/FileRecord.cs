using System;
using Utilities;

namespace UtilTest
{
    public class FileRecord:FileRecordBase
    {
        public string description { get; set; }
        public int id { get; set; }
        public DateTime createDate { get; set; }

        public string getRecord()
        {
            return string.Format("{0}, {1}, {2}", "                                ", id.ToString().PadRight(4, '0'), createDate.ToString("yyyy-M-d dddd"));
        }

        public FileRecord()
        {
            Delimiter = "\r\n";
            RecordSize = 57;
        }
        public override void Parse(string line)
        {
            var arr = line.Split(',');
            description = arr[0];
            id = int.Parse (arr[1]);
            createDate = DateTime.Parse(arr[2]);
        }
    }
}