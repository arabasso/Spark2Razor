using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(2)]
    public class ContentRule :
        RegexRule
    {
        public static readonly Regex
            ContentRegex = new Regex(@"\$\{((?>[^\{\}]+|\{(?<Depth>)|\}(?<-Depth>))*(?(Depth)(?!)))\}");

        private static readonly Regex
            TernaryOperatorRegex = new Regex(@"");

        public override string Convert(string text,
            int position,
            Match match)
        {
            var value = match.Groups[1].Value;

            return text.Replace(match.Value,
                IsComplex(value) ? $"@({value})" : $"@{value}",
                position + match.Index, match.Length);
        }

        private bool IsComplex(string input)
        {
            return Regex.IsMatch(input, @".+?\?.+?\:.+?")
                || Regex.IsMatch(input, @"\(.+?\)\s*.+?");
        }

        public ContentRule() :
            base(ContentRegex)
        {
        }
    }
}
