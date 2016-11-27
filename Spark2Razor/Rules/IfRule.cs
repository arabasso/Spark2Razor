namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(10)]
    public class IfRule :
        BlockRule
    {
        public IfRule() :
            base("if", "if ({0})", "condition")
        {
        }
    }
}
