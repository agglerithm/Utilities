namespace Utilities.IO
{
    public abstract class FileRecordBase
    {  

        public int RecordSize { get;  set; } 
        public string Delimiter { get;  set; }
        public int TotalSize { get { return RecordSize + Delimiter.Length; } }
        public abstract void Parse(string line);
    }
}