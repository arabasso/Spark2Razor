using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(10)]
    public class ForRule :
        BlockRule
    {
        public ForRule() :
            base("for", "@foreach ({0})", "each")
        {
        }

        public override string Convert(string text,
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
                node.Inner = Convert(node.Inner);
            }

            var value = $"{arguments.Initialization}\r\n@foreach ({expression})\r\n{{{arguments.Declaration}\r\n\t<text>\r\n{node.Inner}\r\n\t</text>{arguments.Increment}\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
