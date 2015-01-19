using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace $rootnamespace$.Extensions
{
    public static class CommonExtensions
    {
        public static string BuffToString(this byte[] buff)
        {
            return Encoding.UTF8.GetString(buff.SkipWhile(c => c == 0).ToArray());
        }

        public static string SafeTrim(this string str)
        {
            if (str == null) return null;
            return str.Trim();
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string[] Split(this string str, string delim)
        {
            return Regex.Split(str, delim);
        }

        public static int CastToInt(this string str)
        {
            return int.Parse(str);
        }

        public static decimal CastToDecimal(this string str)
        {
            return decimal.Parse(str);
        }

        public static DateTime CastToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }
    }
}