﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Spark
{
    [ConverterRuleOrder(1)]
    public class EscapeExpressionSpecialStringsRule :
        RegexIterationRule
    {
        protected static Dictionary<string, string>
            SpecialStrings = new Dictionary<string, string>
            {
                {
                    DoubleQuotesUnescaped,
                    DoubleQuotesEscaped
                },
                {
                    GreaterThanUnescaped,
                    GreaterThanEscaped
                }
            };

        public EscapeExpressionSpecialStringsRule() :
            base(ContentRule.ContentRegex,
                BalancedParenthesisRegex,
                BalancedBracketsRegex)
        {
        }

        public override string Convert(string text, int position, Match match)
        {
            return SpecialStrings.Aggregate(text, (current, specialString) => current.Replace(specialString.Key, specialString.Value, position + match.Index, match.Length));
        }
    }
}
