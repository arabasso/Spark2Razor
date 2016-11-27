using System.Text.RegularExpressions;
using NUnit.Framework;
using Spark2Razor.Spark;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class ConverterRuleTest
    {
        private static string Convert<T>(string input)
            where T : ConverterRule, new()
        {
            return new T().Convert(input);
        }

        [TestCase("\"\\\"Text\\\"\"",
            ExpectedResult = "\"&&backslashquot;;Text&&backslashquot;;\"")]
        public string Escape_special_strings(string input)
        {
            return Convert<EscapeSpecialStringsRule>(input);
        }

        [TestCase("\"&&backslashquot;;Text&&backslashquot;;\"",
            ExpectedResult = "\"\\\"Text\\\"\"")]
        [TestCase("${Html.Partial(&&quot;;Index&&quot;;)}",
            ExpectedResult = "${Html.Partial(\"Index\")}")]
        [TestCase("${value &&gt;; 10 ? &&quot;;10&&quot;; : &&quot;;&&quot;;}",
            ExpectedResult = "${value > 10 ? \"10\" : \"\"}")]
        public string Unescape_special_strings(string input)
        {
            return Convert<UnescapeSpecialStringsRule>(input);
        }

        [TestCase("${Html.Partial(\"Index\")}",
            ExpectedResult = "${Html.Partial(&&quot;;Index&&quot;;)}")]
        [TestCase("<a href=\"${Html.Partial(\"Index\")}\" title=\"${TempData[\"Value\"]}\">Link</a>",
            ExpectedResult = "<a href=\"${Html.Partial(&&quot;;Index&&quot;;)}\" title=\"${TempData[&&quot;;Value&&quot;;]}\">Link</a>")]
        [TestCase("<a if=\"TempData[\"Value\"] == null\" href=\"#\">Link</a>",
            ExpectedResult = "<a if=\"TempData[&&quot;;Value&&quot;;] == null\" href=\"#\">Link</a>")]
        [TestCase("<a if=\"(value == \"Value\")\" href=\"#\">Link</a>",
            ExpectedResult = "<a if=\"(value == &&quot;;Value&&quot;;)\" href=\"#\">Link</a>")]
        [TestCase("${value > 10 ? \"10\" : \"\"}",
            ExpectedResult = "${value &&gt;; 10 ? &&quot;;10&&quot;; : &&quot;;&&quot;;}")]
        public string Escape_expression_special_chars(string input)
        {
            return Convert<EscapeExpressionSpecialStringsRule>(input);
        }

        private class IterationRule :
            RegexIterationRule
        {
            public int Count { get; private set; }

            public IterationRule() :
                base(ContentRule.ContentRegex)
            {
            }

            public override string Convert(string text, int position, Match match)
            {
                Count++;

                return text;
            }
        }

        [TestCase("${Html.LabelFor(m => m.Name, new { Id = 1 }, null)}",
            ExpectedResult = 1)]
        public int Regex_iteration(string input)
        {
            var rule = new IterationRule();

            rule.Convert(input);

            return rule.Count;
        }
    }
}
