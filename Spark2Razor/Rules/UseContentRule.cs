using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class UseContentRule :
        LineRule
    {
        public UseContentRule() :
            base("use", "content")
        {
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var content = node.Attributes["content"];

            if (content != "view") return text;

            var value = $"\r\n@RenderBody()\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}