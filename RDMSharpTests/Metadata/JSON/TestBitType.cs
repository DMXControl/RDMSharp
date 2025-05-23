using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestBitType
    {
        [Test]
        public void TestMany()
        {
            var bitType = new BitType("NAME", "DISPLAY_NAME", "NOTES", null, "bit", 1, true, false);
            Assert.That(bitType.Index, Is.EqualTo(1));
            Assert.Throws(typeof(NotSupportedException), () => bitType.GetDataLength());
            byte[] bytes = new byte[0];
            Assert.Throws(typeof(NotSupportedException), () => bitType.ParseDataToPayload(ref bytes));
            Assert.Throws(typeof(NotSupportedException), () => bitType.ParsePayloadToData(new DataTree()));
        }
    }
}