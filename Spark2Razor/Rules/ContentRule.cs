using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(2)]
    public class ContentRule :
        ConverterRule
    {
        public static readonly Regex
            ContentRegex = new Regex(@"\$\{((?>[^\{\}]+|\{(?<Depth>)|\}(?<-Depth>))*(?(Depth)(?!)))\}");

        public string Convert(string text, int position, Match match)
        {
            var value = match.Groups[1].Value.Trim();

            var index = position + match.Index + match.Length;

            var suffix = string.Empty;

            if (index < text.Length)
            {
                suffix = text[index].ToString();
            }

            return text.Replace(match.Value,
                IsComplex(value, suffix) ? $"@({value})" : $"@{value}",
                position + match.Index, match.Length);
        }

        private bool IsComplex(string input, string suffix)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(suffix, @"^[^<""\s]$")
                || System.Text.RegularExpressions.Regex.IsMatch(input, @".+?\?.+?\:.+?")
                || System.Text.RegularExpressions.Regex.IsMatch(input, @"^\s*\(.+?\)\s*.+?");
        }

        public override string Convert(string input)
        {
            return Convert(ContentRegex, input, Convert);
        }
    }
}
