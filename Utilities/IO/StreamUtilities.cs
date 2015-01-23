using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Utilities.WinMockups;
using Utilities.Extensions;

namespace Utilities.IO
{
    public static class StreamUtilities
    { 

        public static long GetDemarcationPosition(Stream strm, string endPattern)
        { 
            var foundOrEof = false;
            int nextChar = 0;
            while (!foundOrEof )
            {
                foundOrEof = FindPattern(strm, endPattern, null, strm.Length);
            }
            if (nextChar == 0)
                return -1;
            return strm.Position;
        }

        public static RawTextResults GetBlockText(Stream strm, long limit, string startPattern, string endPattern)
        { 
 
            var retVal = new RawTextResults(); 

            RawTextResults str = null;
            do
            {
                str = GetRecordText(strm, limit, startPattern, endPattern);
                    if(str.Text != string.Empty)
                        retVal.Add(str.Text);
                    retVal.SetTrailing(str.TrailingGarbage); 
                    retVal.SetLeading(str.LeadingGarbage);
            } while (!(strm.EOF() || strm.ReachedLimit(limit)));
             
            return retVal;
        }

        public static RawTextResults GetRecordText(Stream strm, long limit, string startPattern, string endPattern)
        { 
            long startNdx = 0;
            long endNdx = 0; 
            var foundOrHalt = false;
            StringBuilder frontBuilder = new StringBuilder();
            var retVal = new RawTextResults();
            while (!foundOrHalt)
            {
                foundOrHalt = FindPattern(strm, startPattern, frontBuilder, limit);
            }
            var strippedVal = frontBuilder.ToString().Replace(startPattern, "");

            var prefix = frontBuilder.Replace(strippedVal, "").ToString();

            var leadingGarbage = !strippedVal.Contains(endPattern) && prefix == startPattern;

            var trailingGarbage = !strippedVal.Contains(endPattern) && prefix == string.Empty; 

            if (leadingGarbage)
            {
                retVal.SetLeading(strippedVal); 
            }
            if(trailingGarbage)
            { 
                retVal.SetTrailing(strippedVal);
                return retVal;
            }
                var backBuilder = new StringBuilder();

                foundOrHalt = false;

                while (!foundOrHalt)
                {
                    foundOrHalt = FindPattern(strm, endPattern, backBuilder, limit);
                }

                var val = backBuilder.ToString();
              
                trailingGarbage = !val.Contains(endPattern);
                
            var tempVal = startPattern + val;

           if (trailingGarbage) 
               retVal.SetTrailing(tempVal);

           else   retVal.Add(tempVal);
 
            return retVal;
        }

        public static bool FindPattern(Stream strm, string pattern, StringBuilder builder, long limit)
        {
            var lastNdx = pattern.Length - 1;
            var nextChar = strm.ReadByte();
            while (!strm.EOF() &&  !strm.ReachedLimit(limit))
            {
                builder.Append((char)nextChar);

                if ((char) nextChar == pattern[lastNdx]) break;

                nextChar = strm.ReadByte();

            }
            if (strm.EOF() || strm.ReachedLimit(limit))
            {
                builder.Append(nextChar);
                return true;
            }

            return builder.ToString().EndsWith(pattern);
        }
 
 
    }
}
