using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor
{
    public abstract class ConverterRule
    {
        public const string BackSlashDoubleQuotesUnescaped = "\\\"";
        public const string BackSlashDoubleQuotesEscaped = "&&backslashquot;;";

        public const string QuotesUnescaped = "'";

        public const string DoubleQuotesUnescaped = "\"";
        public const string DoubleQuotesEscaped = "&&quot;;";

        public const string EqualUnescaped = "=";
        public const string EqualEscaped = "&&eq;;";

        public const string LessThanUnescaped = "<";
        public const string LessThanEscaped = "&&lt;;";

        public const string GreaterThanUnescaped = ">";
        public const string GreaterThanEscaped = "&&gt;;";

        public const string AndAlsoUnescaped = "&&";
        public const string AndAlsoEscaped = "&&andalsoamp;;";

        public static readonly Regex
            BalancedParenthesisRegex = new Regex(@"\(((?>[^\(\)]+|\((?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!)))\)");

        public static readonly Regex
            BalancedBracketsRegex = new Regex(@"\[((?>[^\[\]]+|\[(?<Depth>)|\](?<-Depth>))*(?(Depth)(?!)))\]");

        public static readonly Regex
            SingleQuotesRegex = new Regex($@"{QuotesUnescaped}(.{{2,}}?){QuotesUnescaped}");

        public static readonly Dictionary<Regex, string> 
            Regex = new Dictionary<Regex, string>
            {
                { new Regex($@"(({EqualEscaped}{EqualEscaped}|\!{EqualEscaped}|{LessThanEscaped}|{LessThanEscaped}{EqualEscaped}|{GreaterThanEscaped}|{GreaterThanEscaped}{EqualEscaped})\s*)\'(.*?([^\']+))\'"), $"$1{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}" },
                { new Regex(@"^((\s*)\'([^\']+)\'(\s*))$"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
                { new Regex(@"^((\s*)\'([^\']+)\'(\s*))$"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
                { new Regex(@"((\(\s*)\'([^\']+)\'(\s*\)))"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
                { new Regex(@"((\[\s*)\'([^\']+)\'(\s*\]))"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
                { new Regex(@"((\(\s*)\'([^\']+)\'(\s*,))"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
                { new Regex(@"((\,\s*)\'([^\']+)\'(\s*\)))"), $"$2{DoubleQuotesEscaped}$3{DoubleQuotesEscaped}$4" },
            };

        public abstract string Convert(string input);

        public static string ConvertToString(string input)
        {
            var text = Regex.Aggregate(input, (current, pair) => pair.Key.Replace(current, pair.Value));

            text = System.Text.RegularExpressions.Regex.Replace(text, $"^{QuotesUnescaped}{QuotesUnescaped}$", DoubleQuotesEscaped + DoubleQuotesEscaped);

            return text;
        }

        protected string Convert(Regex regex, string text, Func<string, int, Match, string> func)
        {
            var match = regex.Match(text);

            var position = 0;

            while (match.Success)
            {
                var length = text.Length;

                text = func(text, position, match);

                position += match.Index + match.Length + (text.Length - length);

                match = regex.Match(text.Substring(position));
            }

            return text;
        }
    }
}
