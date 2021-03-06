﻿using System.Collections.Generic;
using System.Linq;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(1000)]
    public class UnescapeSpecialStringsRule :
        ConverterRule
    {
        protected static Dictionary<string, string>
            SpecialStrings = new Dictionary<string, string>
            {
                {
                    BackSlashDoubleQuotesEscaped,
                    BackSlashDoubleQuotesUnescaped
                },
                {
                    DoubleQuotesEscaped,
                    DoubleQuotesUnescaped
                },
                {
                    LessThanEscaped,
                    LessThanUnescaped
                },
                {
                    GreaterThanEscaped,
                    GreaterThanUnescaped
                },
                {
                    EqualEscaped,
                    EqualUnescaped
                }
            };

        public override string Convert(string input)
        {
            return SpecialStrings.Aggregate(input, (current, specialChar) => current.Replace(specialChar.Key, specialChar.Value));
        }
    }
}
