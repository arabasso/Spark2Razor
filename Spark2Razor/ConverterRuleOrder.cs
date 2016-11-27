using System;

namespace Spark2Razor
{
    public class ConverterRuleOrderAttribute :
        Attribute
    {
        public static readonly ConverterRuleOrderAttribute
            Default = new ConverterRuleOrderAttribute(int.MaxValue);

        public int Index { get; set; }

        public ConverterRuleOrderAttribute(int index)
        {
            Index = index;
        }
    }
}
