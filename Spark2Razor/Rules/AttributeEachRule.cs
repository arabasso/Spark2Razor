using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(20)]
    public class AttributeEachRule :
        NodeRule
    {
        public AttributeEachRule() :
            base(@"\w+", "each")
        {
        }

        public override string Convert(int index,
            string text,
            Node node,
            int position,
            Match match)
        {
            var expression = node.Attributes["each"];

            var eachExpression = new EachExpression(expression);

            var arguments = eachExpression.ExtractArguments(match.Value);

            node.Attributes.Remove("each");

            if (node.IsBlock)
            {
                node = new Node(node.Name, node.Attributes, Convert(node.Inner), node.IsBlock);
            }

            var inner = node.Text;

            var value = $"{arguments.Initialization}\r\n@foreach ({expression})\r\n{{{arguments.Declaration}\r\n\t<text>\r\n{inner}\r\n\t</text>{arguments.Increment}\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
