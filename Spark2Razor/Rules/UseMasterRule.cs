using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(10)]
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

            var value = $"@{{ Layout = \"~/Views/Shared/{master}.cshtml\"; }}";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
