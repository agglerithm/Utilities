using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Extensions
{
    public static class CommonExtensions
    {
        public static string BuffToString(this byte[] buff)
        {
            return Encoding.UTF8.GetString(buff.SkipWhile(c => c == 0).ToArray());
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string[] Split(this string str, string delim)
        {
            return Regex.Split(str, delim);
        }
    }
}