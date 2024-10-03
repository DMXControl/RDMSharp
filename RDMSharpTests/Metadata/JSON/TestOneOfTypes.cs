using RDMSharp.Metadata.JSON.OneOfTypes;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestOneOfTypes
    {
        [Test]
        public void TestMany()
        {
            var oneOfTypes = new OneOfTypes();
            Assert.That(oneOfTypes.IsEmpty, Is.True);
            Assert.That(oneOfTypes.ToString(), Is.Null);
            Assert.DoesNotThrow(() =>
            {
                Assert.That(oneOfTypes.GetDataLength().Value, Is.EqualTo(0));
            });
        }
    }
}