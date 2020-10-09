using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Web.Utility
{
    public static class StringExtension
    {
        public static int[] SplitToIntArray(this string s, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            var result = s.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => Convert.ToInt32(x))
                .ToArray();
            return result;
        }
    }
}
