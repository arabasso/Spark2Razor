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

            var index = 0;

            foreach (var regex in _regex)
            {
                var i = index;

                text = Convert(regex, text, (t, p, m) => Convert(i, t, p, m));

                index++;
            }

            return text;
        }

        public abstract string Convert(int index,
            string text,
            int position,
            Match match);
    }
}
