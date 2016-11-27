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

            var node = new Node("if", "condition=\"value == null\"", "Text");

            Assert.That(attributes, Is.EqualTo(node.Attributes));
        }
    }
}
