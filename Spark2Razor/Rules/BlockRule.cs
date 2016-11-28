using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class BlockRule :
        RegexRule
    {
        private readonly string _attribute;
        private readonly string _format;

        protected BlockRule(string name) :
            base(new Regex($@"<({name})\s*(.*?)\s*>(?<inner>(?><\1.*?>(?<LEVEL>)|</\1>(?<-LEVEL>)|\s?(?!<\1.*?>|</\1>).)*\s?(?(LEVEL)(?!)))</\1>"))
        {
        }

        protected BlockRule(string name,
            string format) :
            this(name)
        {
            _format = format;
        }

        protected BlockRule(string name,
            string format,
            string attribute) : 
            this(name, format)
        {
            _attribute = attribute;
        }

        public override string Convert(int index,
            string text,
            int position,
            Match match)
        {
            var node = new Node(match.Groups[1].Value,
                match.Groups[2].Value,
                match.Groups["inner"].Value,
                true);

            return Convert(text, node, position, match);
        }

        public virtual string Convert(string text,
            Node node,
            int position,
            Match match)
        {
            var expression = string.Empty;

            if (!string.IsNullOrEmpty(_attribute))
            {
                expression = node.Attributes[_attribute];
            }

            var inner = Convert(node.Inner);

            var name = string.Format(_format, expression);

            var value = $"\r\n{name}\r\n{{\r\n\t<text>\r\n{inner}\r\n\t</text>\r\n}}\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
