using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class AttributeRule :
        RegexRule
    {
        public static readonly Regex
            AttributeRegex = new Regex(@"=""((?>""\b(?<DEPTH>)|\b""(?<-DEPTH>)|[^""]*)*(?(DEPTH)(?!)))""");

        protected AttributeRule() :
            base(AttributeRegex)
        {
        }

        public override string Convert(int index, string text, int position, Match match)
        {
            var value = match.Groups[1].Value;

            if (string.IsNullOrEmpty(value)) return text;

            return text.Replace(value, ConvertAttribute(value, position, match), position + match.Index, match.Length);
        }

        public abstract string ConvertAttribute(string text, int position, Match match);
    }
}
