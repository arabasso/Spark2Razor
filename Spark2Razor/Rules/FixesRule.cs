using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(50)]
    public class FixesRule :
        ConverterRule
    {
        private static readonly Dictionary<Regex, string> 
            FixRegex = new Dictionary<Regex, string>
            {
                { new Regex(@"(TempData\.Contains\s*\()"), "TempData.ContainsKey("},
                { new Regex(@"(string\.IsNullOrEmpty\s*\(\s*)((View|Temp)Data)"), "$1(string)$2"}
            };

        public override string Convert(string input)
        {
            return FixRegex.Aggregate(input, (current, r) => r.Key.Replace(current, r.Value));
        }
    }
}
