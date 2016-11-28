using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(2)]
    public class HasAttributeRule :
        AttributeRule
    {
        public static readonly Regex
            HasContentRegex = new Regex(@"([^\s.]+?)\?\{((?>[^\{\}]+|\{(?<Depth>)|\}(?<-Depth>))*(?(Depth)(?!)))\}");

        public override string ConvertAttribute(string text, int position, Match match)
        {
            return HasContentRegex.Replace(text, $"@($2 ? {DoubleQuotesEscaped}$1{DoubleQuotesEscaped} : {DoubleQuotesEscaped}{DoubleQuotesEscaped})");
        }
    }
}
