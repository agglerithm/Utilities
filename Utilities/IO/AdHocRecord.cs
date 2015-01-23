using System;
using System.Net.Mime;

namespace Utilities.IO
{
    public class AdHocRecord : FileRecordBase
    {
        public string Text { get; private set; }
        public override void Parse(string line)
        {
            Text = line;
        }
    }
}