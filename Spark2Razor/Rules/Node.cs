using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class Node
    {
        public static readonly Regex
            AttributesRegex = new Regex(@"((?<name>[\w-]+)\s*=\s*""(?<value>.*?)"")|((?<name>[\w-]+)=(?<value>\w+))");

        public Node(string name,
            NameValueCollection attributes,
            string inner,
            bool isBlock)
        {
            Name = name;
            Inner = inner;
            IsBlock = isBlock;
            Attributes = attributes;
        }

        public Node(string name,
            string attributes,
            string inner,
            bool isBlock)
        {
            Name = name;
            Inner = inner;
            IsBlock = isBlock;

            foreach (Match match in AttributesRegex.Matches(attributes))
            {
                Attributes.Add(match.Groups["name"].Value, match.Groups["value"].Value);
            }
        }

        public string Name { get; }
        public NameValueCollection Attributes { get; } = new NameValueCollection();
        public string Inner { get; set; }
        public bool IsBlock { get; set; }

        public string Text
        {
            get
            {
                var attributes = Attributes.AllKeys.
                    Select(s => $"{s}=\"{Attributes[s]}\"");

                var attr = string.Join(" ", attributes);

                if (!string.IsNullOrEmpty(attr))
                {
                    attr = " " + attr;
                }

                return IsBlock
                    ? $"<{Name}{attr}>{Inner}</{Name}>"
                    : $"<{Name}{attr} />";
            }
        }
    }
}