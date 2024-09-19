using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestStringType
    {
        [Test]
        public void TestMany()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null,null,null,null,null,null,null);
            Assert.That(stringType.MinLength, Is.Null);
            Assert.That(stringType.MaxLength, Is.Null);


            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(0));
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 1, 32, null, null, null);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(32));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 1, 32, 0, 34, null);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(0));
                Assert.That(pdl.MaxLength, Is.EqualTo(34));
            });

            Assert.Throws(typeof(ArgumentException), () => stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "sting", null, null, null, null, null, null, null));
        }
    }
}