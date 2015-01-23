using System.Collections.Generic;
using System.Text;

namespace Utilities.IO
{
    public class RawTextResults
    {
        private readonly List<string> _records = new List<string>();
        public string Text { get { return _textBuilder.ToString(); } }
        public IEnumerable<string> Records { get { return _records; } }
        public string LeadingGarbage { get; private set; }
        public string TrailingGarbage { get; private set; }
        private readonly StringBuilder _textBuilder = new StringBuilder();

        public void SetTrailing(string trailing)
        {
            if (string.IsNullOrEmpty(TrailingGarbage))
                TrailingGarbage = trailing;
        }
        public void SetLeading(string leading)
        {

            if (string.IsNullOrEmpty(LeadingGarbage))
                LeadingGarbage = leading;
        }

        public void Add(string rec)
        {
            _records.Add(rec);
            _textBuilder.Append(rec) ;
        }
    }
}