using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class LineRule :
        ConverterRule
    {
        private readonly string _attribute;
        private readonly Regex _regex;

        protected LineRule(string tag)
        {
            _regex = new Regex($@"(<({tag})([^>]*))(?>/)>");
        }

        protected LineRule(string tag, string attribute) :
            this(tag)
        {
            _attribute = attribute;
        }

        public override string Convert(string input)
        {
            return Convert(_regex, input, Convert);
        }

        public string Convert(string text,
            int position,
            Match match)
        {
            var node = new Node(match.Groups[2].Value, match.Groups[3].Value.Trim(), "", true);

            if (!string.IsNullOrEmpty(_attribute) && !node.Attributes.AllKeys.Contains(_attribute)) return text;

            return Convert(text, node, position, match);
        }

        public abstract string Convert(string text,
            Node node,
            int position,
            Match match);
    }
}
