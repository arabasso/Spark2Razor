using NUnit.Framework;
using Spark2Razor.Rules;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class ConverterTest
    {
        private Converter _converter;

        private string Convert<T>(string input)
            where T : ConverterRule, new()
        {
            _converter.AddRule(new T());

            return _converter.Convert(input);
        }

        [SetUp]
        public void SetUp()
        {
            _converter = new Converter();

            _converter.AddRule(new EscapeSpecialStringsRule());
            _converter.AddRule(new EscapeExpressionSpecialStringsRule());
            _converter.AddRule(new UnescapeSpecialStringsRule());
        }

        [TestCase("\"\\\"Text\\\"\"")]
        public void Escape_unescape_special_strings(string input)
        {
            Assert.That(input, Is.EqualTo(_converter.Convert(input)));
        }

        [TestCase("${Html.Partial(\"Index\")}",
            ExpectedResult = "@Html.Partial(\"Index\")")]
        [TestCase("<a href=\"${Html.Partial(\"Index\")}\" title=\"${TempData[\"Value\"]}\">Link</a>",
            ExpectedResult = "<a href=\"@Html.Partial(\"Index\")\" title=\"@TempData[\"Value\"]\">Link</a>")]
        [TestCase("${value > 10 ? \"10\" : \"\"}",
            ExpectedResult = "@(value > 10 ? \"10\" : \"\")")]
        [TestCase("${(int)value}",
            ExpectedResult = "@((int)value)")]
        public string Content_conversion(string input)
        {
            return Convert<ContentRule>(input);
        }

        [TestCase("<if condition=\"variable == null\">Text</if>",
            ExpectedResult = "\r\n@if (variable == null)\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n")]
        [TestCase("<if condition=\"variable == null\"><if condition=\"1 != 0\">Text</if></if>",
            ExpectedResult = "\r\n@if (variable == null)\r\n{\r\n\t<text>\r\n\r\n@if (1 != 0)\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n\r\n\t</text>\r\n}\r\n")]
        public string If_conversion(string input)
        {
            var output = Convert<IfRule>(input);

            return output;
        }
    }
}
