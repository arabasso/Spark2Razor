using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(3)]
    public class ModelRule :
        LineRule
    {
        public ModelRule() :
            base("viewdata", "model")
        {
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var model = node.Attributes["model"];

            var value = $"\r\n@model {model}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}