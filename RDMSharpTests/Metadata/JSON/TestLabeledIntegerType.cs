using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestLabeledIntegerType
    {
        [Test]
        public void TestMany()
        {
            var labeledIntegerType = new LabeledIntegerType("NAME", "DISPLAY_NAME", "NOTES", null, 3);
            Assert.That(labeledIntegerType.Value, Is.EqualTo(3));
            Assert.Throws(typeof(NotSupportedException), () => labeledIntegerType.GetDataLength());
            byte[] bytes = new byte[0];
            Assert.Throws(typeof(NotSupportedException), () => labeledIntegerType.ParseDataToPayload(ref bytes));
            Assert.Throws(typeof(NotSupportedException), () => labeledIntegerType.ParsePayloadToData(new DataTree()));
        }
    }
}