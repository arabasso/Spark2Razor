using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(3)]
    public class SetRule :
        LineRule
    {
        public SetRule() :
            base("set")
        {
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var attributeName = node.Attributes.AllKeys
                .First(w => w != "type");

            var attributeValue = ConvertToString(node.Attributes[attributeName]);

            var value = $"\r\n@{{ ViewBag.{attributeName} = {attributeValue}; }}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}