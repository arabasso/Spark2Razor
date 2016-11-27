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

    public class UseMasterRule :
        LineRule
    {
        public UseMasterRule() :
            base("use", "master")
        {
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var master = node.Attributes["master"];

            var value = $"\r\n@{{ Layout = \"~/Views/Shared/{master}.cshtml\"; }}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
