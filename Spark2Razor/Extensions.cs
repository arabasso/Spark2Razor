using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Spark2Razor
{
    public static class Extensions
    {
        public static string Replace(this string str,
            string oldValue,
            string newValue,
            int startIndex,
            int count)
        {
            var sb = new StringBuilder(str);

            sb.Replace(oldValue, newValue, startIndex, count);

            return sb.ToString();
        }

        public static ConverterRuleOrderAttribute RuleOrder(this ConverterRule rule)
        {
            return (ConverterRuleOrderAttribute)rule.GetType()
                .GetCustomAttributes(typeof(ConverterRuleOrderAttribute), true)
                .FirstOrDefault() ?? ConverterRuleOrderAttribute.Default;
        }

        public static IEnumerable<Type> GetConverterRuleTypes(this Assembly assembly, string @namespace)
        {
            return assembly.GetTypes()
                .Where(w => w.Namespace == @namespace)
                .Where(w => w.IsClass && !w.IsAbstract)
                .Where(w => w.IsSubclassOf(typeof(ConverterRule)));
        }
    }
}
