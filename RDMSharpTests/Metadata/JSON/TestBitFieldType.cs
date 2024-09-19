using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestBitFieldType
    {
        [Test]
        public void TestMany()
        {
            var bitTypes=new BitType[2];
            bitTypes[0] = new BitType("NAME11", "DISPLAY_NAME11", "NOTES11", null, null, 2, false, false);
            bitTypes[1] = new BitType("NAME22", "DISPLAY_NAME22", "NOTES22", null, null, 4, false, false);
            var bitFieldType = new BitFieldType("NAME", "DISPLAY_NAME", "NOTES", null, "bitField",16, false, bitTypes);
            Assert.That(bitFieldType.Size, Is.EqualTo(16));
            Assert.That(bitFieldType.Bits, Has.Length.EqualTo(2));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = bitFieldType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(2));
            });

            Assert.Throws(typeof(ArgumentException), () => bitFieldType = new BitFieldType("NAME", "DISPLAY_NAME", "NOTES", null, "botField", 16, false, bitTypes));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bitFieldType = new BitFieldType("NAME", "DISPLAY_NAME", "NOTES", null, "bitField", 7, false, bitTypes));
        }
    }
}