using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class Node
    {
        public static readonly Regex
            AttributesRegex = new Regex(@"([\w-]+)\s*=\s*""(.*?)""");

        public Node(string name,
            string attributes,
            string inner)
        {
            Name = name;
            Inner = inner;

            foreach (Match match in AttributesRegex.Matches(attributes))
            {
                Attributes.Add(match.Groups[1].Value, match.Groups[2].Value);
            }
        }

        public string Name { get; }
        public NameValueCollection Attributes { get; } = new NameValueCollection();
        public string Inner { get; }
    }
}