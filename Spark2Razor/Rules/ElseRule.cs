namespace Spark2Razor.Rules
{
    [ConverterRuleOrder(10)]
    public class ElseRule :
        BlockRule
    {
        public ElseRule() :
            base("else", "else")
        {
        }
    }
}