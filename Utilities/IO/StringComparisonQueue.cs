using System.Collections.Generic;
using System.Linq;

namespace Utilities.IO
{
    public class StringComparisonQueue
    {
        private readonly Queue<char> _charQueue;
        private readonly string _compareValue;
        private int _len;
        private int _lastNdx;

        public StringComparisonQueue(string compareValue)
        {
            _compareValue = compareValue;
            _len = _compareValue.Length;
            _lastNdx = _len - 1;
            _charQueue = new Queue<char>(_len);
        }

        public int Index { get; private set; }

        public bool FeedAndCompare(char nextChar)
        {
            _charQueue.Enqueue(nextChar);
            truncateQueue();
            if (doComparison())
                return true;
            Index++;
            return false;
        }

        private bool doComparison()
        {
            if (_charQueue.Count != _len) return false;
            if (_charQueue.Peek() != _compareValue[_lastNdx]) return false;
            var chars = _charQueue.ToArray(); 
            return !_compareValue.Where((t, i) => t != chars[i]).Any();
        }

        private void truncateQueue()
        {
            while (_charQueue.Count > _len)
                _charQueue.Dequeue();

        }
    }
}