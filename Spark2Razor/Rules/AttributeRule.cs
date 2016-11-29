using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class AttributeRule :
        ConverterRule
    {
        public static readonly Regex
            AttributeRegex = new Regex(@"=""((?>""\b(?<DEPTH>)|\b""(?<-DEPTH>)|[^""]*)*(?(DEPTH)(?!)))""");

        public override string Convert(string input)
        {
            return Convert(AttributeRegex, input, Convert);
        }

        public string Convert(string text, int position, Match match)
        {
            var value = match.Groups[1].Value;

            return string.IsNullOrEmpty(value)
                ? text :
                text.Replace(value, ConvertAttribute(value, position, match), position + match.Index, match.Length);
        }

        public abstract string ConvertAttribute(string text, int position, Match match);
    }
}
