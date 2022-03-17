using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EEC.Utils
{
    public static class RegexUtil
    {
        private static readonly Regex _vectorRegex = new("-?[0-9.]+");

        public static bool TryParseVectorString(string input, out float[] vectorArray)
        {
            try
            {
                var matches = _vectorRegex.Matches(input);
                var count = matches.Count;
                if (count < 1)
                    throw new Exception();

                vectorArray = new float[count];

                for (int i = 0; i < count; i++)
                {
                    var match = matches[i];
                    vectorArray[i] = float.Parse(match.Value, CultureInfo.InvariantCulture);
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