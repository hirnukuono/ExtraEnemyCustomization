using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EECustom.Utils
{
    public static class RegexUtil
    {
        public static readonly Regex VectorRegex = new Regex("[0-9.]+"); 

        public static bool TryParseVectorString(string input, out float[] vectorArray)
        {
            try
            {
                var matches = VectorRegex.Matches(input);
                var count = matches.Count;
                if (count < 1)
                    throw new Exception();

                vectorArray = new float[count];

                for (int i = 0; i<count; i++)
                {
                    var match = matches[i];
                    vectorArray[i] = float.Parse(match.Value);
                }

                return true;
            }
            catch
            {
                vectorArray = null;
                return false;
            }
        }
    }
}
