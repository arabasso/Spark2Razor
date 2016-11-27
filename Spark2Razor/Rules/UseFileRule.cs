using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spark2Razor.Rules
{
    public class UseFileRule :
        LineRule
    {
        private TextInfo _textInfo;

        public UseFileRule() :
            base("use", "file")
        {
            _textInfo = CultureInfo.CurrentCulture.TextInfo;
        }

        public override string Convert(string text, Node node, int position, Match match)
        {
            var file = node.Attributes["file"];

            var arguments = node.Attributes.AllKeys
                .Where(w => w != "file")
                .Select(attribute => $"\r\n@{{ViewBag.Partial{_textInfo.ToTitleCase(attribute)} = {node.Attributes[attribute]};}}")
                .ToList();

            var args = string.Join("", arguments);

            var value = $"{args}\r\n@Html.Partial(\"{file}\")\r\n";

            return text.Replace(match.Value, value, position + match.Index, match.Length);
        }
    }
}
