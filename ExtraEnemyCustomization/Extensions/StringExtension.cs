using System;

namespace EEC
{
    public static class StringExtension
    {
        public static bool InvariantEquals(this string str, string strToCompare, bool ignoreCase = false)
        {
            return str.Equals(strToCompare, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }

        public static bool InvariantContains(this string str, string strToCompare, bool ignoreCase = false)
        {
            return str.Contains(strToCompare, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }

        public static bool InvariantStartsWith(this string str, string strToCompare, bool ignoreCase = false)
        {
            return str.StartsWith(strToCompare, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }

        public static bool InvariantEndsWith(this string str, string strToCompare, bool ignoreCase = false)
        {
            return str.EndsWith(strToCompare, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }

        public static bool EqualsAnyIgnoreCase(this string input, params string[] args)
        {
            return EqualsAny(input, true, args);
        }

        public static bool EqualsAny(this string input, bool ignoreCase, params string[] args)
        {
            var comparisonMode = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var arg in args)
            {
                if (input.Equals(arg, comparisonMode))
                    return true;
            }
            return false;
        }

        public static bool ContainsAnyIgnoreCase(this string input, params string[] args)
        {
            return ContainsAny(input, true, args);
        }

        public static bool ContainsAny(this string input, bool ignoreCase, params string[] args)
        {
            var comparisonMode = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var arg in args)
            {
                if (input.Contains(arg, comparisonMode))
                    return true;
            }
            return false;
        }
    }
}