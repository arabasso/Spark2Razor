using NUnit.Framework;
using Spark2Razor;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class ExtensionsTest
    {
        [TestCase("BAAA", "A", "B", 0, 1,
            ExpectedResult = "BAAA")]
        [TestCase("AAAA", "A", "B", 1, 1,
            ExpectedResult = "ABAA")]
        [TestCase("AAAB", "A", "B", 3, 1,
            ExpectedResult = "AAAB")]
        public string String_range_replace(string input,
            string value,
            string replace,
            int start,
            int end)
        {
            return input.Replace(value, replace, start, end);
        }
    }
}
