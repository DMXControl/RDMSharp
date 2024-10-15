using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestDefineNotFoundException
    {
        [Test]
        public void TestMany()
        {
            Assert.Multiple(() =>
            {
                var exception = new DefineNotFoundException();
                Assert.That(exception.Message, Is.Not.Null);
                exception = new DefineNotFoundException("Test");
                Assert.That(exception.Message, Is.EqualTo("Test"));
            });
        }
    }
}