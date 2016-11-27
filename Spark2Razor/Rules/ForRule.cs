using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class ForRule :
        BlockRule
    {
        public ForRule() :
            base("for")
        {
        }

        public override string Convert(string text,
            Node node,
            int position,
            Match match)
        {
            var expression = node.Attributes["each"];

            var inner = Convert(node.Inner);

            var value = $"\r\n@foreach ({expression})\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}