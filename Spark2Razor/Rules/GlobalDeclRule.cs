using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(3)]
    public class GlobalDeclRule :
        LineRule
    {
        public GlobalDeclRule() :
            base("global")
        {
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var attributeName = node.Attributes.AllKeys
                .First(w => w != "type");

            var attributeValue = ConvertToString(node.Attributes[attributeName]);

            var attributeType = node.Attributes.AllKeys
                .FirstOrDefault(w => w == "type");

            if (!string.IsNullOrEmpty(attributeType))
            {
                attributeType = " as " + node.Attributes[attributeType];
            }

            var value = $"@{{ var {attributeName} = (ViewBag.{attributeName}{attributeType}) ?? {attributeValue}; }}";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}