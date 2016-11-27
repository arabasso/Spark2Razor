using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class IfRule :
        BlockRule
    {
        public IfRule() :
            base("if", "if ({0})", "condition")
        {
        }
    }
}
