using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(1)]
    public class EscapeExpressionSpecialStringsRule :
        RegexRule
    {
        public static readonly Regex
            //BalancedDoubleQuotes = new Regex(@"""((?>""\b(?<DEPTH>)|\b""(?<-DEPTH>)|[^""]*)*(?(DEPTH)(?!)))""");
            BalancedDoubleQuotes = new Regex("\"(.*?)\"");

        protected static Dictionary<string, string>
            SpecialStrings = new Dictionary<string, string>
            {
                {
                    DoubleQuotesUnescaped,
                    DoubleQuotesEscaped
                },
                {
                    EqualUnescaped,
                    EqualEscaped
                },
                {
                    LessThanUnescaped,
                    LessThanEscaped
                },
                {
                    GreaterThanUnescaped,
                    GreaterThanEscaped
                }
            };

        public EscapeExpressionSpecialStringsRule() :
            base(ContentRule.ContentRegex,
                BalancedParenthesisRegex,
                BalancedBracketsRegex,
                BalancedDoubleQuotes)
        {
        }

        public override string Convert(string text,
            int position,
            Match match)
        {
            if (string.IsNullOrEmpty(match.Groups[1].Value)) return text;

            if (match.Groups[1].Value.Contains("IEnumerable"))
            {
                
            }

            var value = SpecialStrings
                .Aggregate(match.Groups[1].Value,
                (current, specialString) => current.Replace(specialString.Key, specialString.Value));

            return text.Replace(match.Groups[1].Value, value, position + match.Index, match.Length);
        }
    }
}
