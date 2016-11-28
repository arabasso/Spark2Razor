using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class NodeRule :
        RegexRule
    {
        private readonly string _attribute;

        protected NodeRule(string tag) :
            base(new Regex($@"<({tag})\s*(.*?)\s*>(?<inner>(?>\s?<\1.*?>(?<LEVEL>)|</\1>(?<-LEVEL>)|\s?(?!<\1.*?>|</\1>).)*\s?(?(LEVEL)(?!)))</\1>"),
                new Regex($@"(<({tag})([^>]*))(?>/)>"))
        {
        }

        protected NodeRule(string name,
            string attribute) : 
            this(name)
        {
            _attribute = attribute;
        }


        public override string Convert(int index,
            string text,
            int position,
            Match match)
        {
            var node = index == 0
                ? new Node(match.Groups[1].Value,
                    match.Groups[2].Value,
                    match.Groups["inner"].Value,
                    true)
                : new Node(match.Groups[2].Value,
                    match.Groups[3].Value.Trim(),
                    null,
                    false);

            if (!string.IsNullOrEmpty(_attribute) && !node.Attributes.AllKeys.Contains(_attribute))
            {
                if (!node.IsBlock) return text;

                node.Inner = Convert(node.Inner);

                return text.Replace(match.Value, node.Text, position + match.Index, match.Length);
            }

            return Convert(index, text, node, position, match);
        }

        public abstract string Convert(int index,
            string text,
            Node node,
            int position,
            Match match);
    }
}
