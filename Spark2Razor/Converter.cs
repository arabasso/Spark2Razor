using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spark2Razor
{
    public class Converter
    {
        private List<ConverterRule>
            _rules = new List<ConverterRule>();

        public void AddRule(params ConverterRule[] rules)
        {
            _rules.AddRange(rules);

            _rules = _rules.OrderBy(ob => ob.RuleOrder().Index).ToList();
        }

        public void AddRulesFromNamespace(string name)
        {
            var types = new List<Type>();

            types.AddRange(Assembly.GetAssembly(typeof(ConverterRule)).GetConverterRuleTypes(name));

            var rules = types
                .Select(Activator.CreateInstance)
                .Cast<ConverterRule>()
                .ToArray();

            AddRule(rules);
        }

        public string Convert(string input)
        {
            return _rules.Aggregate(input, (current, rule) => rule.Convert(current));
        }
    }
}
