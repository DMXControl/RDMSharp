using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestReferenceType
    {
        [Test]
        public void TestMany()
        {
            var referenceType = new ReferenceType("#/get_request/0");
            Assert.That(referenceType.Command, Is.EqualTo(Command.ECommandDublicte.GetRequest));
            Assert.That(referenceType.Pointer, Is.EqualTo(0));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = referenceType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(0));
                Assert.That(pdl.MinLength, Is.Null);
                Assert.That(pdl.MaxLength, Is.Null);
            });

            Assert.Throws(typeof(ArgumentException), () => referenceType = new ReferenceType("%/get_request/0"));
        }
    }
}