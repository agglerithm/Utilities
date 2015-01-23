using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.IO
{
    public class StructuredRecordBlock<T> : RecordBlock<T> where T : FileRecordBase, new()
    {
        private readonly string _startPattern;
        private readonly string _endPattern;
        private readonly List<AdHocRecord> _recList = new List<AdHocRecord>();
        private int _offset;

        public StructuredRecordBlock(string startPattern, string endPattern, long offset):base(null,0)
        {
            _startPattern = startPattern;
            _endPattern = endPattern;
            _offset = (int) offset;
        }

        public void Add(string rec)
        {
            if (!rec.Contains(_startPattern))
            {
                LeadingIncompleteText = rec;
                return;
            }
            if (!rec.Contains(_endPattern))
            {
                TrailingIncompleteText = rec;
                return;
            }
            var record = new AdHocRecord();
            record.Parse(rec);
            _recList.Add(record);
        }

 
        public override int Offset { get { return _offset; } }
        public string LeadingIncompleteText { get; private set; }
        public string TrailingIncompleteText { get; private set; }

        public IEnumerable<AdHocRecord> Records
        {
            get{return  _recList ;}
        }
    }

 
}