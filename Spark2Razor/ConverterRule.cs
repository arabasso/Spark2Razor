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

        public abstract string Convert(string input);

        public static string ConvertToString(string input)
        {
            var text = SingleQuotesRegex.Replace(input, $"{DoubleQuotesEscaped}$1{DoubleQuotesEscaped}");

            text = Regex.Replace(text, $"^{QuotesUnescaped}{QuotesUnescaped}$", DoubleQuotesEscaped + DoubleQuotesEscaped);

            return text;
        }
    }
}
