using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class BlockRule :
        RegexRule
    {
        protected BlockRule(string name) :
            base(new Regex($@"<({name})\s*(.*?)\s*>(?<inner>(?><\1.*?>(?<LEVEL>)|</\1>(?<-LEVEL>)|(?!<\1.*?>|</\1>).)*(?(LEVEL)(?!)))</\1>"))
        {
        }

        public override string Convert(string text,
            int position,
            Match match)
        {
            var node = new Node(match);

            return Convert(text, node, position, match);
        }

        public abstract string Convert(string text, 
            Node node,
            int position,
            Match match);
    }
}
