using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestLabeledBooleanType
    {
        [Test]
        public void TestMany()
        {
            var labeledBooleanType = new LabeledBooleanType("NAME", "DISPLAY_NAME", "NOTES", null, true);
            Assert.That(labeledBooleanType.Value, Is.True);
            Assert.Throws(typeof(NotSupportedException), () => labeledBooleanType.GetDataLength());
            byte[] bytes = new byte[0];
            Assert.Throws(typeof(NotSupportedException), () => labeledBooleanType.ParseDataToPayload(ref bytes));
            Assert.Throws(typeof(NotSupportedException), () => labeledBooleanType.ParsePayloadToData(new DataTree()));
        }
    }
}