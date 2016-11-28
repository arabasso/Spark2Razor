using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(20)]
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
                node = new Node(node.Name, node.Attributes, Convert(node.Inner), node.IsBlock);
            }

            var inner = node.Text;

            var value = $"\r\n@if ({expression})\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
