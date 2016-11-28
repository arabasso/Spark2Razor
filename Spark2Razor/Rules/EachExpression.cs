using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class EachExpression
    {
        public string Type { get; set; }
        public string Variable { get; set; }
        public string Enumerable { get; set; }

        public EachExpression(string input)
        {
            var match = Regex.Match(input, @"(.+?)\s*(\w+)\s*in\s*(.+)");

            if (match.Success)
            {
                Type = match.Groups[1].Value.Trim();
                Variable = match.Groups[2].Value.Trim();
                Enumerable = match.Groups[3].Value.Trim();
            }
        }

        public EachArguments ExtractArguments(string text)
        {
            return new EachArguments(this, text);
        }
    }
}
