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
        [TestCase("<if condition=\"i == 10\"></if>")]
        [TestCase("<if condition=\"i > 10\"></if>")]
        [TestCase("<if condition=\"i < 10\"></if>")]
        [TestCase("<if condition=\"i < 10 && i > 20\"></if>")]
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
        [TestCase("<if condition=\"variable == null\"><if condition=\"1 < 0\">Text</if></if>",
            ExpectedResult = "\r\n@if (variable == null)\r\n{\r\n\t<text>\r\n\r\n@if (1 < 0)\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n\r\n\t</text>\r\n}\r\n")]
        public string If_conversion(string input)
        {
            return Convert<IfRule>(input);
        }

        [TestCase("<for each=\"var i in ViewBag.List\">${i}</for>",
            ExpectedResult = "\r\n@foreach (var i in ViewBag.List)\r\n{\r\n\t<text>\r\n${i}\r\n\t</text>\r\n}\r\n")]
        [TestCase("<for each=\"var i in ViewBag.ListI\"><for each=\"var j in ViewBag.ListJ\">${i} - ${j}</for></for>",
            ExpectedResult = "\r\n@foreach (var i in ViewBag.ListI)\r\n{\r\n\t<text>\r\n\r\n@foreach (var j in ViewBag.ListJ)\r\n{\r\n\t<text>\r\n${i} - ${j}\r\n\t</text>\r\n}\r\n\r\n\t</text>\r\n}\r\n")]
        public string For_conversion(string input)
        {
            return Convert<ForRule>(input);
        }

        [TestCase("<else>Text</else>",
            ExpectedResult = "\r\n@else\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n")]
        [TestCase("<else><if condition=\"1 > 1\">Text</if><else>Text</else></else>",
            ExpectedResult = "\r\n@else\r\n{\r\n\t<text>\r\n<if condition=\"1 > 1\">Text</if>\r\n@else\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n\r\n\t</text>\r\n}\r\n")]
        public string Else_conversion(string input)
        {
            return Convert<ElseRule>(input);
        }

        [TestCase("<use master=\"Site\" />",
            ExpectedResult = "\r\n@{ Layout = \"~/Views/Shared/Site.cshtml\"; }\r\n")]
        [TestCase("<use content=\"view\" />",
            ExpectedResult = "<use content=\"view\" />")]
        public string Use_master_conversion(string input)
        {
            return Convert<UseMasterRule>(input);
        }

        [TestCase("<use master=\"Site\" />",
            ExpectedResult = "<use master=\"Site\" />")]
        [TestCase("<use content=\"view\" />",
            ExpectedResult = "\r\n@RenderBody()\r\n")]
        public string Use_content_conversion(string input)
        {
            return Convert<UseContentRule>(input);
        }

        [TestCase("<use file=\"PesquisaDocumento\"/>",
            ExpectedResult = "\r\n@Html.Partial(\"PesquisaDocumento\")\r\n")]
        [TestCase("<use file=\"DocumentoItem\" documento=\"d\" />",
            ExpectedResult = "\r\n@{ViewBag.PartialDocumento = d;}\r\n@Html.Partial(\"DocumentoItem\")\r\n")]
        [TestCase("<use file=\"DocumentoItem\" documento=\"d\" nome=\"true\" />",
            ExpectedResult = "\r\n@{ViewBag.PartialDocumento = d;}\r\n@{ViewBag.PartialNome = true;}\r\n@Html.Partial(\"DocumentoItem\")\r\n")]
        public string Use_file_conversion(string input)
        {
            return Convert<UseFileRule>(input);
        }
    }
}
