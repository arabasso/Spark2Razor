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
        [TestCase("${Html.DropDownListFor(m => m.SubTipo, new SelectList(ViewBag.SubTipos, \"Id\", \"Descricao\"), new { tabindex=\"2\", @class=\"float-left\" })}",
            ExpectedResult = "@Html.DropDownListFor(m => m.SubTipo, new SelectList(ViewBag.SubTipos, \"Id\", \"Descricao\"), new { tabindex=\"2\", @class=\"float-left\" })")]
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
            var output = Convert<IfRule>(input);

            return output;
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
            ExpectedResult = "\r\nelse\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n")]
        [TestCase("<else><if condition=\"1 > 1\">Text</if><else>Text</else></else>",
            ExpectedResult = "\r\nelse\r\n{\r\n\t<text>\r\n<if condition=\"1 > 1\">Text</if>\r\nelse\r\n{\r\n\t<text>\r\nText\r\n\t</text>\r\n}\r\n\r\n\t</text>\r\n}\r\n")]
        public string Else_conversion(string input)
        {
            return Convert<ElseRule>(input);
        }

        [TestCase("<use master=\"Site\" />",
            ExpectedResult = "@{ Layout = \"~/Views/Shared/Site.cshtml\"; }")]
        [TestCase("<use content=\"view\" />",
            ExpectedResult = "<use content=\"view\" />")]
        public string Use_master_conversion(string input)
        {
            return Convert<UseMasterRule>(input);
        }

        [TestCase("<use master=\"Site\" />",
            ExpectedResult = "<use master=\"Site\" />")]
        [TestCase("<use content=\"view\" />",
            ExpectedResult = "@RenderBody()")]
        public string Use_content_conversion(string input)
        {
            return Convert<UseContentRule>(input);
        }

        [TestCase("<use file=\"PesquisaDocumento\"/>",
            ExpectedResult = "\r\n@Html.Partial(\"PesquisaDocumento\")")]
        [TestCase("<use file=\"DocumentoItem\" documento=\"d\" />",
            ExpectedResult = "\r\n@{ ViewBag.PartialDocumento = d; }\r\n@Html.Partial(\"DocumentoItem\")")]
        [TestCase("<use file=\"DocumentoItem\" documento=\"d\" nome=\"true\" />",
            ExpectedResult = "\r\n@{ ViewBag.PartialDocumento = d; }\r\n@{ ViewBag.PartialNome = true; }\r\n@Html.Partial(\"DocumentoItem\")")]
        public string Use_file_conversion(string input)
        {
            return Convert<UseFileRule>(input);
        }

        [TestCase("<var tramitacoes=\"ViewBag.Tramitacoes\" type=\"IEnumerable<Siscam.Dados.Models.FluxoModel>\" />",
            ExpectedResult = "@{ var tramitacoes = ViewBag.Tramitacoes as IEnumerable<Siscam.Dados.Models.FluxoModel>; }")]
        [TestCase("<var tramitacoes=\"ViewBag.Tramitacoes\" />",
            ExpectedResult = "@{ var tramitacoes = ViewBag.Tramitacoes; }")]
        [TestCase("<var remetente=\"remetentes.Last()\" />",
            ExpectedResult = "@{ var remetente = remetentes.Last(); }")]
        [TestCase("<var usuario=\"ViewBag.Usuario\" />",
            ExpectedResult = "@{ var usuario = ViewBag.Usuario; }")]
        [TestCase("<var permiteInclusao=\"documentoAutores.Any(a => usuarioAutores.Any(ua => ua.Autor.Id == a.Autor.Id)) || permiteAlteracao\" />",
            ExpectedResult = "@{ var permiteInclusao = documentoAutores.Any(a => usuarioAutores.Any(ua => ua.Autor.Id == a.Autor.Id)) || permiteAlteracao; }")]
        [TestCase("<var permissoes =\"ViewBag.ObterPermissao(Sino.Siscam.Dados.TipoModulo.Workflow)\" type=\"Sino.Permissao\" />",
            ExpectedResult = "@{ var permissoes = ViewBag.ObterPermissao(Sino.Siscam.Dados.TipoModulo.Workflow) as Sino.Permissao; }")]
        public string Var_conversion(string input)
        {
            return Convert<VarRule>(input);
        }

        [TestCase("<set Descricao=\"'Caixa de Entrada'\" />",
            ExpectedResult = "@{ ViewBag.Descricao = \"Caixa de Entrada\"; }")]
        public string Set_conversion(string input)
        {
            return Convert<SetRule>(input);
        }

        [TestCase("<viewdata model=\"Sino.Workflow.Models.PesquisaDocumentoModel\"/>",
            ExpectedResult = "@model Sino.Workflow.Models.PesquisaDocumentoModel")]
        public string Model(string input)
        {
            return Convert<ModelRule>(input);
        }

        [TestCase("<global type=\"string\" Title=\"''\" />",
            ExpectedResult = "@{ var Title = (ViewBag.Title as string) ?? \"\"; }")]
        [TestCase("<global type=\"string\" Descricao=\"''\" />",
            ExpectedResult = "@{ var Descricao = (ViewBag.Descricao as string) ?? \"\"; }")]
        [TestCase("<global type=\"bool\" SemMenu=\"false\" />",
            ExpectedResult = "@{ var SemMenu = (ViewBag.SemMenu as bool) ?? false; }")]
        public string Global_decl_conversion(string input)
        {
            return Convert<GlobalDeclRule>(input);
        }

        [TestCase("<div if=\"!true\">Text</div>",
            ExpectedResult = "\r\n@if (!true)\r\n{\r\n\t<text>\r\n<div>Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div class=\"clear-both\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\">Text</div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n<div class=\"clear-both\">Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div if=\"!string.IsNullOrEmpty(tramite.Complemento)\" class=\"clear-both\">Text</div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n<div class=\"clear-both\">Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<img src=\"~/Dot.gif\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\" />",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n<img src=\"~/Dot.gif\" />\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div class=\"clear-both\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\"><img src=\"~/Dot.gif\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\" /></div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n<div class=\"clear-both\">\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n<img src=\"~/Dot.gif\" />\r\n\t</text>\r\n}\r\n</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div if=\"true\"><span><img src=\"~/Dot.gif\" if=\"false\" /></span></div>",
            ExpectedResult = "\r\n@if (true)\r\n{\r\n\t<text>\r\n<div><span>\r\n@if (false)\r\n{\r\n\t<text>\r\n<img src=\"~/Dot.gif\" />\r\n\t</text>\r\n}\r\n</span></div>\r\n\t</text>\r\n}\r\n")]
        public string Attribute_if(string input)
        {
            return Convert<AttributeIfRule>(input);
        }
    }
}
