using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestDefineNotFoundException
    {
        [Test]
        public void TestMany()
        {
            using (Assert.EnterMultipleScope())
            {
                var exception = new DefineNotFoundException();
                Assert.That(exception.Message, Is.Not.Null);
                exception = new DefineNotFoundException("Test");
                Assert.That(exception.Message, Is.EqualTo("Test"));
            }
        }
    }
}