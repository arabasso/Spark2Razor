using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class IfRule :
        BlockRule
    {
        public IfRule() :
            base("if")
        {
        }

        public override string Convert(string text,
            Node node,
            int position,
            Match match)
        {
            var expression = node.Attributes["condition"];

            var inner = Convert(node.Inner);

            var value = $"\r\n@if ({expression})\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
