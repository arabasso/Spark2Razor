using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Spark
{
    [ConverterRuleOrder(1)]
    public class EscapeExpressionSpecialStringsRule :
        ConverterRule
    {
        protected static List<Regex>
            Regex = new List<Regex>
            {
                new Regex(@"\$\{(?>[^\{\}]+|\{(?<Depth>)|\} (?<-Depth>))*(?(Depth)(?!))\}"),
                new Regex(@"\[(?>[^\[\]]+|\[(?<Depth>)|\](?<-Depth>))*(?(Depth)(?!))\]"),
                new Regex(@"\((?>[^\(\)]+|\((?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\)")
            };

        protected static Dictionary<string, string>
            SpecialStrings = new Dictionary<string, string>
            {
                {
                    DoubleQuotesUnescaped,
                    DoubleQuotesEscaped
                },
                {
                    GreaterThanUnescaped,
                    GreaterThanEscaped
                }
            };

        public override string Convert(string input)
        {
            var text = input;

            foreach (var regex in Regex)
            {
                var match = regex.Match(text);

                var position = 0;

                while (match.Success)
                {
                    text = SpecialStrings.Aggregate(text, (current, specialString) => current.Replace(specialString.Key, specialString.Value, position + match.Index, match.Length));

                    position += match.Index + match.Length;

                    match = regex.Match(text.Substring(position));
                }
            }

            return text;
        }
    }
}
