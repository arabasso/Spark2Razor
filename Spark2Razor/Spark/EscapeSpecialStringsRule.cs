using System.Collections.Generic;
using System.Linq;

namespace Spark2Razor.Spark
{
    [ConverterRuleOrder(0)]
    public class EscapeSpecialStringsRule :
        ConverterRule
    {
        protected static Dictionary<string, string>
            SpecialStrings = new Dictionary<string, string>
            {
                {
                    BackSlashDoubleQuotesUnescaped,
                    BackSlashDoubleQuotesEscaped
                }
            };

        public override string Convert(string input)
        {
            return SpecialStrings.Aggregate(input, (current, specialChar) => current.Replace(specialChar.Key, specialChar.Value));
        }
    }
}
