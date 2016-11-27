using NUnit.Framework;
using Spark2Razor.Spark;

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
    }
}
