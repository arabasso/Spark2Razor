using System.Collections.Specialized;
using NUnit.Framework;
using Spark2Razor.Rules;

namespace Spark2Razor.Test
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void Node_read_attributes()
        {
            var attributes = new NameValueCollection
            {
                {"condition", "value == null"}
            };

            var node = new Node("if", "condition=\"value == null\"", "Text", true);

            Assert.That(attributes, Is.EqualTo(node.Attributes));
        }

        [TestCase("span", "class=\"text-center\"", "Text", true,
            ExpectedResult = "<span class=\"text-center\">Text</span>")]
        [TestCase("span", "class=w", "Text", true,
            ExpectedResult = "<span class=\"w\">Text</span>")]
        [TestCase("img", "src=\"~/Img/Dot.gif\"", null, false,
            ExpectedResult = "<img src=\"~/Img/Dot.gif\" />")]
        public string Node_text(string tag, string attributes, string inner, bool isBlock)
        {
            var node = new Node(tag, attributes, inner, isBlock);

            return node.Text;
        }
    }
}
