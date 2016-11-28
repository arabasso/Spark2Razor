using NUnit.Framework;
using Spark2Razor.Rules;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class ExpressionTest
    {
        [SetUp]
        public void SetUp()
        {
            EachArguments.InTest = true;
        }

        [TestCase("var t in tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado)",
            "t", "tramitacoes.Where(w => w.Status != StatusFluxo.Finalizado)")]
        [TestCase("Sino.Siscam.Dados.Models.AutorModel autor in ViewBag.Remetentes",
            "autor", "ViewBag.Remetentes")]
        [TestCase("Sino.Siscam.Dados.Models.AutorModel autor in ViewBag.Remetentes as IEnumerable<AutorModel>",
            "autor", "ViewBag.Remetentes as IEnumerable<AutorModel>")]
        [TestCase("Sino.Siscam.Dados.Models.AutorModel autor in (IEnumerable<AutorModel>)ViewBag.Remetentes",
            "autor", "(IEnumerable<AutorModel>)ViewBag.Remetentes")]
        public void ExtractInExpression(string input, string variable, string enumerable)
        {
            var expression = new EachExpression(input);

            Assert.That(expression.Variable, Is.EqualTo(variable));
            Assert.That(expression.Enumerable, Is.EqualTo(enumerable));
        }

        [TestCase("<for each=\"var t in tramitacoes\"><div class=\"item item-impar?{tIsEven}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"false\" /></div></for>",
            "var t in tramitacoes", true, false, false, false)]
        [TestCase("<for each=\"var t in tramitacoes\"><div class=\"item item-impar?{tIsOdd}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"false\" /></div></for>",
            "var t in tramitacoes", false, true, false, false)]
        [TestCase("<for each=\"var t in tramitacoes\"><div class=\"item item-impar?{tIsFirst}\"><use file=\"FluxoItem\" fluxo=\"t\" entrada=\"false\" /></div></for>",
            "var t in tramitacoes", false, false, true, false)]
        [TestCase("<input each=\"Siscam.Dados.Models.FluxoModel tramitacao in ViewBag.Tramitacao\" type=\"hidden\" name=\"Tramitacao[${tramitacaoIndex}]\" value=\"${tramitacao.Id}\" />",
            "Siscam.Dados.Models.FluxoModel tramitacao in ViewBag.Tramitacao", false, false, false, true)]
        public void ExtractEachExpressionArguments(string text, string input, bool isEven, bool isOdd, bool isFirst, bool hasIndex)
        {
            var expression = new EachExpression(input);

            var arguments = expression.ExtractArguments(text);

            Assert.That(arguments.IsEven, Is.EqualTo(isEven));
            Assert.That(arguments.IsOdd, Is.EqualTo(isOdd));
            Assert.That(arguments.IsFirst, Is.EqualTo(isFirst));
            Assert.That(arguments.HasIndex, Is.EqualTo(hasIndex));
        }
    }
}
