using RDMSharp.Metadata.JSON;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestCommand
    {
        [Test]
        public void TestMany()
        {
            var command = new Command( Command.ECommandDublicte.GetResponse);
            Assert.That(command.GetIsEmpty(), Is.False);
            Assert.Throws(typeof(NotSupportedException), () => command.GetDataLength());
        }
    }
}