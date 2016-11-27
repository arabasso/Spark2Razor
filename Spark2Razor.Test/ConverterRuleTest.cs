using System.Text.RegularExpressions;
using NUnit.Framework;
using Spark2Razor.Rules;

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
            ExpectedResult = "<a if=\"TempData[&&quot;;Value&&quot;;] &&eq;;&&eq;; null\" href=\"#\">Link</a>")]
        [TestCase("<a if=\"(value == \"Value\")\" href=\"#\">Link</a>",
            ExpectedResult = "<a if=\"(value &&eq;;&&eq;; &&quot;;Value&&quot;;)\" href=\"#\">Link</a>")]
        [TestCase("${value > 10 ? \"10\" : \"\"}",
            ExpectedResult = "${value &&gt;; 10 ? &&quot;;10&&quot;; : &&quot;;&&quot;;}")]
        [TestCase("${value < 10 ? \"10\" : \"\"}",
            ExpectedResult = "${value &&lt;; 10 ? &&quot;;10&&quot;; : &&quot;;&&quot;;}")]
        [TestCase("<viewdata model=\"Sino.Workflow.Models.DocumentoModel\" />\r\n<use master=\"Site\" />\r\n<set Descricao=\"'Documentos'\" />\r\n\r\n<var usuario=\"ViewBag.Usuario\" />\r\n<var tramitacoes=\"ViewBag.Tramitacoes\" type=\"IEnumerable<Sino.Siscam.Dados.Models.FluxoModel>\" />\r\n<var documentoAutores=\"ViewBag.Documento.Autores\" type=\"IEnumerable<Sino.Siscam.Dados.Models.DocumentoAutorModel>\" />\r\n",
            ExpectedResult = "<viewdata model=\"Sino.Workflow.Models.DocumentoModel\" />\r\n<use master=\"Site\" />\r\n<set Descricao=\"'Documentos'\" />\r\n\r\n<var usuario=\"ViewBag.Usuario\" />\r\n<var tramitacoes=\"ViewBag.Tramitacoes\" type=\"IEnumerable&&lt;;Sino.Siscam.Dados.Models.FluxoModel&&gt;;\" />\r\n<var documentoAutores=\"ViewBag.Documento.Autores\" type=\"IEnumerable&&lt;;Sino.Siscam.Dados.Models.DocumentoAutorModel&&gt;;\" />\r\n")]
        public string Escape_expression_special_chars(string input)
        {
            var output = Convert<EscapeSpecialStringsRule>(input);

            output = Convert<EscapeExpressionSpecialStringsRule>(output);

            return output;
        }

        private class IterationRule :
            RegexRule
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
