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
            var expression = ConvertToString(node.Attributes["each"].Trim());

            var eachExpression = new EachExpression(expression);

            var arguments = eachExpression.ExtractArguments(match.Value);

            node.Attributes.Remove("each");

            if (node.IsBlock)
            {
                node.Inner = Convert(node.Inner).Replace("\r\n", "\r\n\t");
            }

            var value = $"{arguments.Initialization}\r\n@foreach ({expression})\r\n{{{arguments.Declaration}{arguments.Increment}\r\n\t<text>\r\n\t\t{node.Inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
