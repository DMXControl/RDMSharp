using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestPD_EnvelopeType
    {
        [Test]
        public void TestMany()
        {
            var pdEnvelopeType = new PD_EnvelopeType("NAME", "DISPLAY_NAME", "NOTES", null, "pdEnvelope", 2);
            Assert.That(pdEnvelopeType.Length, Is.EqualTo(2));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = pdEnvelopeType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(2));
            });

            Assert.Throws(typeof(ArgumentException), () => pdEnvelopeType = new PD_EnvelopeType("NAME", "DISPLAY_NAME", "NOTES", null, "pdEnvelop", 2));

        }
    }
}