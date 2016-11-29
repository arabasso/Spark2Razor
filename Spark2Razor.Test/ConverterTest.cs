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
            EachArguments.InTest = true;

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
        [TestCase("<img src=\"~/${ext}.gif\" />",
            ExpectedResult = "<img src=\"~/@(ext).gif\" />")]
        [TestCase("${ String.Format(\"{0:dd/MM/yyyy}\",a.Data)}",
            ExpectedResult = "@String.Format(\"{0:dd/MM/yyyy}\",a.Data)")]
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
            ExpectedResult = "\r\n@if (variable == null)\r\n{\r\n\t<text>\r\n\t\tText\r\n\t</text>\r\n}\r\n")]
        [TestCase("<if condition=\"variable == null\"><if condition=\"1 < 0\">Text</if></if>",
            ExpectedResult = "\r\n@if (variable == null)\r\n{\r\n\t<text>\r\n\t\t\r\n\t@if (1 < 0)\r\n\t{\r\n\t\t<text>\r\n\t\t\tText\r\n\t\t</text>\r\n\t}\r\n\t\r\n\t</text>\r\n}\r\n")]
        public string If_conversion(string input)
        {
            var output = Convert<IfRule>(input);

            return output;
        }

        [TestCase("<for each=\"var i in ViewBag.List\">${i}</for>",
            ExpectedResult = "\r\n@foreach (var i in ViewBag.List)\r\n{\r\n\t<text>\r\n\t\t${i}\r\n\t</text>\r\n}\r\n")]
        [TestCase("<for each=\"var i in ViewBag.ListI\"><for each=\"var j in ViewBag.ListJ\">${i} - ${j}</for></for>",
            ExpectedResult = "\r\n@foreach (var i in ViewBag.ListI)\r\n{\r\n\t<text>\r\n\t\t\r\n\t@foreach (var j in ViewBag.ListJ)\r\n\t{\r\n\t\t<text>\r\n\t\t\t${i} - ${j}\r\n\t\t</text>\r\n\t}\r\n\t\r\n\t</text>\r\n}\r\n")]
        [TestCase("<for each=\"var i in ViewBag.ListI\"><for each=\"var j in ViewBag.ListJ\">${iIndex} - ${j}</for></for>",
            ExpectedResult = "\r\n@{ var __i0 = 0; }\r\n@foreach (var i in ViewBag.ListI)\r\n{\r\n\tvar iIndex = __i0;\r\n\t__i0++;\r\n\t<text>\r\n\t\t\r\n\t@foreach (var j in ViewBag.ListJ)\r\n\t{\r\n\t\t<text>\r\n\t\t\t${iIndex} - ${j}\r\n\t\t</text>\r\n\t}\r\n\t\r\n\t</text>\r\n}\r\n")]
        public string For_conversion(string input)
        {
            return Convert<ForRule>(input);
        }

        [TestCase("<else>Text</else>",
            ExpectedResult = "\r\nelse\r\n{\r\n\t<text>\r\n\t\tText\r\n\t</text>\r\n}\r\n")]
        [TestCase("<else><if condition=\"1 > 1\">Text</if><else>Text</else></else>",
            ExpectedResult = "\r\nelse\r\n{\r\n\t<text>\r\n\t\t<if condition=\"1 > 1\">Text</if>\r\n\telse\r\n\t{\r\n\t\t<text>\r\n\t\t\tText\r\n\t\t</text>\r\n\t}\r\n\t\r\n\t</text>\r\n}\r\n")]
        public string Else_conversion(string input)
        {
            return Convert<ElseRule>(input);
        }

        [TestCase("<use master=\"Site\" />",
            ExpectedResult = "@{ Layout = \"../Shared/Site.cshtml\"; }")]
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
            ExpectedResult = "\r\n@if (!true)\r\n{\r\n\t<text>\r\n\t\t<div>Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div class=\"clear-both\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\">Text</div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n\t\t<div class=\"clear-both\">Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div if=\"!string.IsNullOrEmpty(tramite.Complemento)\" class=\"clear-both\">Text</div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n\t\t<div class=\"clear-both\">Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<img src=\"~/Dot.gif\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\" />",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n\t\t<img src=\"~/Dot.gif\" />\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div class=\"clear-both\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\"><img src=\"~/Dot.gif\" if=\"!string.IsNullOrEmpty(tramite.Complemento)\" /></div>",
            ExpectedResult = "\r\n@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n{\r\n\t<text>\r\n\t\t<div class=\"clear-both\">\r\n\t@if (!string.IsNullOrEmpty(tramite.Complemento))\r\n\t{\r\n\t\t<text>\r\n\t\t\t<img src=\"~/Dot.gif\" />\r\n\t\t</text>\r\n\t}\r\n\t</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div if=\"true\"><span><img src=\"~/Dot.gif\" if=\"false\" /></span></div>",
            ExpectedResult = "\r\n@if (true)\r\n{\r\n\t<text>\r\n\t\t<div><span>\r\n\t@if (false)\r\n\t{\r\n\t\t<text>\r\n\t\t\t<img src=\"~/Dot.gif\" />\r\n\t\t</text>\r\n\t}\r\n\t</span></div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div if=\"System.IO.Path.GetExtension(ni.Imagem) == \'.mp3\' || System.IO.Path.GetExtension(ni.Imagem) == \'.wma\' || System.IO.Path.GetExtension(ni.Imagem) == \'.wav\' \">Text</div>",
            ExpectedResult = "\r\n@if (System.IO.Path.GetExtension(ni.Imagem) == \".mp3\" || System.IO.Path.GetExtension(ni.Imagem) == \".wma\" || System.IO.Path.GetExtension(ni.Imagem) == \".wav\")\r\n{\r\n\t<text>\r\n\t\t<div>Text</div>\r\n\t</text>\r\n}\r\n")]
        public string Attribute_if_conversion(string input)
        {
            return Convert<AttributeIfRule>(input);
        }

        [TestCase("<div each=\"var t in tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado)\" class=\"text-center\">Text</div>",
            ExpectedResult = "\r\n@foreach (var t in tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado))\r\n{\r\n\t<text>\r\n\t\t<div class=\"text-center\">Text</div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<option each=\"Sino.Siscam.Dados.Models.AutorModel autor in remetentes\" value=\"${autor.Id}\" data-tipo=\"${(int)autor.TipoAutor}\" data-tipo-descricao=\"${Sino.Siscam.Dados.Descricoes.TiposAutorLista.First(f => f.Tipo == autor.TipoAutor).Descricao}\">${autor.UsarApelido ? autor.Apelido : autor.Nome}</option>",
            ExpectedResult = "\r\n@foreach (Sino.Siscam.Dados.Models.AutorModel autor in remetentes)\r\n{\r\n\t<text>\r\n\t\t<option value=\"${autor.Id}\" data-tipo=\"${(int)autor.TipoAutor}\" data-tipo-descricao=\"${Sino.Siscam.Dados.Descricoes.TiposAutorLista.First(f => f.Tipo == autor.TipoAutor).Descricao}\">${autor.UsarApelido ? autor.Apelido : autor.Nome}</option>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<div each=\"var t in tramitacoes\" class=\"item item-impar?{tIsEven}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"false\" /></div>",
            ExpectedResult = "\r\n@{ var __i0 = 0; }\r\n@foreach (var t in tramitacoes)\r\n{\r\n\tvar tIsEven = (__i0 % 2 == 0);\r\n\t__i0++;\r\n\t<text>\r\n\t\t<div class=\"item item-impar?{tIsEven}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"false\" /></div>\r\n\t</text>\r\n}\r\n")]
        [TestCase("<input each=\"Siscam.Dados.Models.FluxoModel tramitacao in ViewBag.Tramitacao\" type=\"hidden\" name=\"Tramitacao[${tramitacaoIndex}]\" value=\"${tramitacao.Id}\" />",
            ExpectedResult = "\r\n@{ var __i0 = 0; }\r\n@foreach (Siscam.Dados.Models.FluxoModel tramitacao in ViewBag.Tramitacao)\r\n{\r\n\tvar tramitacaoIndex = __i0;\r\n\t__i0++;\r\n\t<text>\r\n\t\t<input type=\"hidden\" name=\"Tramitacao[${tramitacaoIndex}]\" value=\"${tramitacao.Id}\" />\r\n\t</text>\r\n}\r\n")]
        public string Attribute_each_conversion(string input)
        {
            var output = Convert<AttributeEachRule>(input);

            return output;
        }

        [TestCase("<tr each=\"var arquivo in arquivos\" class=\"item-impar?{arquivoIsEven}\"></tr>",
            ExpectedResult = "<tr each=\"var arquivo in arquivos\" class=\"@(arquivoIsEven ? \"item-impar\" : \"\")\"></tr>")]
        [TestCase("<tr each=\"var arquivo in arquivos\" class=\"item item-impar?{arquivoIsEven}\"></tr>",
            ExpectedResult = "<tr each=\"var arquivo in arquivos\" class=\"item @(arquivoIsEven ? \"item-impar\" : \"\")\"></tr>")]
        [TestCase("<tr each=\"var arquivo in arquivos\" class=\"item item-impar?{arquivoIsEven} item-par?{arquivoIsOdd} item-selected\"></tr>",
            ExpectedResult = "<tr each=\"var arquivo in arquivos\" class=\"item @(arquivoIsEven ? \"item-impar\" : \"\") @(arquivoIsOdd ? \"item-par\" : \"\") item-selected\"></tr>")]
        [TestCase("<option each=\"var ta in ViewBag.TipoAutores\" value=\"${ta.Tipo}\" selected=\"selected?{ta.Tipo == ViewBag.TipoAutorPadrao}\">${ta.Descricao}</option>",
            ExpectedResult = "<option each=\"var ta in ViewBag.TipoAutores\" value=\"${ta.Tipo}\" selected=\"@(ta.Tipo == ViewBag.TipoAutorPadrao ? \"selected\" : \"\")\">${ta.Descricao}</option>")]
        [TestCase("<div each=\"var t in tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado)\" class=\"item item-impar?{tIsEven} item-destaque?{t.Status == StatusFluxo.Enviado} margin-bottom-10\" data-lote=\"${t.Status}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"true\" /></div>",
            ExpectedResult = "<div each=\"var t in tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado)\" class=\"item @(tIsEven ? \"item-impar\" : \"\") @(t.Status == StatusFluxo.Enviado ? \"item-destaque\" : \"\") margin-bottom-10\" data-lote=\"${t.Status}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"true\" /></div>")]
        [TestCase("<option value=\"0\" selected=\"\">Selecione...</option>",
            ExpectedResult = "<option value=\"0\" selected=\"\">Selecione...</option>")]
        public string Inline_has_attribute(string input)
        {
            return Convert<HasAttributeRule>(input);
        }

        [TestCase("${Html.Partial('Index')}",
            ExpectedResult = "${Html.Partial(\"Index\")}")]
        [TestCase("${Url.Action('Index','Home')}",
            ExpectedResult = "${Url.Action(\"Index\",\"Home\")}")]
        [TestCase("${date.ToString(\"dd 'de' MM 'de' yyyy\")}",
            ExpectedResult = "${date.ToString(\"dd 'de' MM 'de' yyyy\")}")]
        [TestCase("<li if=\"ViewData[\'Administrador\'] != null\"></li>",
            ExpectedResult = "<li if=\"ViewData[\"Administrador\"] != null\"></li>")]
        [TestCase("${Url.Action(\"Editar\",\"Usuario\", new {id=ViewData[\'IdUsuario\']})}",
            ExpectedResult = "${Url.Action(\"Editar\",\"Usuario\", new {id=ViewData[\"IdUsuario\"]})}")]
        [TestCase("<tr each=\"var arquivo in arquivos\" class=\"item-impar?{ext == '.gif'}\"></tr>",
            ExpectedResult = "<tr each=\"var arquivo in arquivos\" class=\"item-impar?{ext == \".gif\"}\"></tr>")]
        [TestCase("<div if=\"ViewBag.Informacao == 'presencial'\"></div>",
            ExpectedResult = "<div if=\"ViewBag.Informacao == 'presencial'\"></div>")]
        [TestCase("<p if=\"string.IsNullOrEmpty(Model.ConfirmarSenha)\">${Html.TextBoxFor(e=>e.Cnpj, new{@Class='CampoCPF width50', onblur=\"validarCPF(this.value)\", title=\"Informe o CPF\"})}</p>",
            ExpectedResult = "<p if=\"string.IsNullOrEmpty(Model.ConfirmarSenha)\">${Html.TextBoxFor(e=>e.Cnpj, new{@Class=\"CampoCPF width50\", onblur=\"validarCPF(this.value)\", title=\"Informe o CPF\"})}</p>")]
        [TestCase("<a href=\"${Url.Action('Listar','Composicao', new{id=m.Composicao.Id})}\"> ${m.Composicao.Titulo} (${m.Cargo.Descricao})</a> <br/>",
            ExpectedResult = "<a href=\"${Url.Action(\"Listar\",\"Composicao\", new{id=m.Composicao.Id})}\"> ${m.Composicao.Titulo} (${m.Cargo.Descricao})</a> <br/>")]
        public string Quotes_to_double_quotes_if_string(string input)
        {
            return _converter.Convert(input);
        }

        [TestCase("<div if=\"TempData.Contains(\"Aviso\")\"><strong> ${TempData[\"Aviso\"]}</strong></div>",
            ExpectedResult = "<div if=\"TempData.ContainsKey(\"Aviso\")\"><strong> ${TempData[\"Aviso\"]}</strong></div>")]
        [TestCase("<h3 if=\"!string.IsNullOrEmpty(ViewData[\"Funcionamento\"])\">${ViewData[\"Funcionamento\"]}</h3>",
            ExpectedResult = "<h3 if=\"!string.IsNullOrEmpty((string)ViewData[\"Funcionamento\"])\">${ViewData[\"Funcionamento\"]}</h3>")]
        [TestCase("<h3 if=\"!string.IsNullOrEmpty(TempData[\"Funcionamento\"])\">${ViewData[\"Funcionamento\"]}</h3>",
            ExpectedResult = "<h3 if=\"!string.IsNullOrEmpty((string)TempData[\"Funcionamento\"])\">${ViewData[\"Funcionamento\"]}</h3>")]
        public string Fixes_conversion(string input)
        {
            return Convert<FixesRule>(input);
        }
    }
}
