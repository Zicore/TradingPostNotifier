using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SessionKeyReader
{
    public static class StringExtensions
    {

        public static string After(this String str, string find, int startIndex)
        {
            int index = str.IndexOf(find, startIndex);
            return (index < 0) ? null : str.Substring(index + find.Length);

        }

        public static string After(this String str, string find)
        {
            return After(str, find, 0);
        }

        public static string Before(this String str, string find, int startIndex)
        {
            int index = str.IndexOf(find, startIndex);
            return (index < 0) ? null : str.Substring(startIndex, index);

        }

        public static string Before(this String str, string find)
        {
            return Before(str, find, 0);
        }

        public static string Between(this String str, string before, string after, int startIndex)
        {
            string afterPart = str.After(before, startIndex);
            return (afterPart == null) ? null : afterPart.Before(after, startIndex);
        }

        public static string Between(this String str, string before, string after)
        {
            return Between(str, before, after, 0);
        }
    }
}
