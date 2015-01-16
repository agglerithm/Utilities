using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Test
{
    public class TestTimer : IDisposable
    {
        private readonly string _finishMsg;
        private DateTime _startTime;

        private TestTimer(string finishMsg)
        {
            _finishMsg = finishMsg;
            _startTime = DateTime.Now;
        }

        public static TestTimer Start(string finishMsg)
        {
            return new TestTimer(finishMsg);
        }

        public void Dispose()
        {

            Console.WriteLine("{0} Took {1} seconds.", _finishMsg, (DateTime.Now - _startTime).Seconds);
        }
    }
}
