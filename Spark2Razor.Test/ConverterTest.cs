using NUnit.Framework;
using Spark2Razor.Spark;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class ConverterTest
    {
        [TestCase("\"\\\"Text\\\"\"")]
        public void Escape_unescape_special_strings(string input)
        {
            var converter = new Converter();

            converter.AddRule(new EscapeSpecialStringsRule());
            converter.AddRule(new UnescapeSpecialStringsRule());

            Assert.AreEqual(input, converter.Convert(input));
        }
    }
}
