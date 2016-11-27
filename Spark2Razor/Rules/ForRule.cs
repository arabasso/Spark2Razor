using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(10)]
    public class ForRule :
        BlockRule
    {
        public ForRule() :
            base("for", "@foreach ({0})", "each")
        {
        }
    }
}