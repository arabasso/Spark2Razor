using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class ElseRule :
        BlockRule
    {
        public ElseRule() :
            base("else")
        {
        }

        public override string Convert(string text,
            Node node,
            int position,
            Match match)
        {
            var inner = Convert(node.Inner);

            var value = $"\r\n@else\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}