using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class ElseRule :
        BlockRule
    {
        public ElseRule() :
            base("else", "else")
        {
        }
    }
}