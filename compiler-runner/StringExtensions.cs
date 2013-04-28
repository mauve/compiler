using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompilerRunner
{
    public static class StringExtensions
    {
        public static String FirstLine(this String str)
        {
            return str.Substring(0, str.IndexOf(Environment.NewLine));
        }

        public static bool RegexMatchesAny(this String str, IEnumerable<Regex> regexes)
        {
            foreach (var regex in regexes)
            {
                Match match = regex.Match(str);
                if (match.Success)
                    return true;
            }

            return false;
        }

        public static bool RegexMatchesAny(this String str, IEnumerable<String> regexes)
        {
            foreach (var regex in regexes)
            {
                var match = Regex.Match(str, regex);
                if (match.Success)
                    return true;
            }

            return false;
        }
    }
}
