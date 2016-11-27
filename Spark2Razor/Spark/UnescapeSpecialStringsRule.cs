using System.Collections.Generic;
using System.Linq;

namespace Spark2Razor.Spark
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
                    GreaterThanEscaped,
                    GreaterThanUnescaped
                }
            };

        public override string Convert(string input)
        {
            return SpecialStrings.Aggregate(input, (current, specialChar) => current.Replace(specialChar.Key, specialChar.Value));
        }
    }
}
