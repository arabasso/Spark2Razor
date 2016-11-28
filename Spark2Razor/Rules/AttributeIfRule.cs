using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(21)]
    public class AttributeIfRule :
        NodeRule
    {
        public AttributeIfRule() :
            base(@"\w+", "if")
        {
        }

        public override string Convert(int index,
            string text,
            Node node,
            int position,
            Match match)
        {
            var expression = node.Attributes["if"];

            node.Attributes.Remove("if");

            if (node.IsBlock)
            {
                node.Inner = Convert(node.Inner);
            }

            var inner = node.Text;

            var value = $"\r\n@if ({expression})\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
