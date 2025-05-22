using RDMSharp.Metadata.JSON;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestCommand
    {
        [Test]
        public void TestMany()
        {
            var command = new Command( Command.ECommandDublicate.GetResponse);
            Assert.That(command.GetIsEmpty(), Is.False);
            Assert.Throws(typeof(NotSupportedException), () => command.GetDataLength());
        }
    }
}