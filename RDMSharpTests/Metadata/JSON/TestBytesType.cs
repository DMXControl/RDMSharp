using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestBytesType
    {
        [Test]
        public void TestMany()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, null, null);
            Assert.That(bytesType.MinLength, Is.Null);
            Assert.That(bytesType.MaxLength, Is.Null);

            bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 1, null);
            Assert.That(bytesType.MinLength, Is.EqualTo(1));
            Assert.That(bytesType.MaxLength, Is.Null);

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = bytesType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 1, 3);
            Assert.That(bytesType.MinLength, Is.EqualTo(1));
            Assert.That(bytesType.MaxLength, Is.EqualTo(3));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = bytesType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(3));
            });

            Assert.Throws(typeof(ArgumentException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bites", null, 1, 5));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 6, 5));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 4294567890, null));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 2, 4294567890));
        }
    }
}