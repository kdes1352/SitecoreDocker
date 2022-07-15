using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AllinaHealth.Framework.Extensions
{
    public static class StringExtensions
    {
        private static string _attributeRegex = "({0}=\".+?\")";

        public static string StripHtmlAttributes(this string s, string attributeName, string replaceWith = "")
        {
            var r = new Regex(string.Format(_attributeRegex, attributeName), RegexOptions.IgnoreCase);
            return r.Replace(s, replaceWith);
        }

        public static string StripHtmlClassAndStyleAttributes(this string s)
        {
            var value = s.StripHtmlAttributes("class");
            return value.StripHtmlAttributes("style");
        }

        public static bool ContainsAny(this string s, IEnumerable<string> values)
        {
            return values.Any(value => s.ToLower().Contains(value.ToLower()));
        }
    }
}