using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public abstract class RegexRule :
        ConverterRule
    {
        private readonly List<Regex> _regex;

        protected RegexRule(params Regex [] regex)
        {
            _regex = new List<Regex>(regex);
        }

        public override string Convert(string input)
        {
            var text = input;

            foreach (var regex in _regex)
            {
                var match = regex.Match(text);

                var position = 0;

                while (match.Success)
                {
                    text = Convert(text, position, match);

                    position += match.Index + match.Length;

                    if (position >= text.Length) break;

                    match = regex.Match(text.Substring(position));
                }
            }

            return text;
        }

        public abstract string Convert(string text,
            int position,
            Match match);
    }
}
