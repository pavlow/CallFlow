using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class Utilities
    {
        public static string Expand(string pattern, string name, string value)
        {
            return Expand(pattern, name, value, 0);
        }

        private static string Expand(string pattern, string name, string value, int count)
        {
            if (!pattern.Contains("{") || !pattern.Contains("}")) return pattern;
            var start = pattern.ToLowerInvariant().IndexOf("{" + name.ToLowerInvariant());
            if (start == -1) return pattern;
            var length = name.Length + "{}".Length;
            if (start + length > pattern.Length) return pattern;
            var result = "";

            if (pattern.Substring(start + name.Length + 1, 1) == "}")
            {
                result = pattern.Substring(0, start) + value + pattern.Substring(start + length, pattern.Length - (start + length));
            } else 
            {
                return pattern;
            }
            if (result != pattern && count < 40) 
            {
                result = Expand(result, name, value, count + 1);
            }
            return result;
        }
    }
}