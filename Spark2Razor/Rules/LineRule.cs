using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class LineRule :
        RegexRule
    {
        private readonly string _attribute;

        protected LineRule(string tag) :
            base(new Regex($@"(<({tag})([^>]*))(?>/)>"))
        {
        }

        protected LineRule(string tag, string attribute) :
            this(tag)
        {
            _attribute = attribute;
        }

        public override string Convert(int index,
            string text,
            int position,
            Match match)
        {
            var node = new Node(match.Groups[2].Value,
                match.Groups[3].Value.Trim(),
                "",
                true);

            if (!string.IsNullOrEmpty(_attribute) && !node.Attributes.AllKeys.Contains(_attribute)) return text;

            return Convert(text, node, position, match);
        }

        public abstract string Convert(string text,
            Node node,
            int position,
            Match match);
    }
}
