using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestBooleanType
    {
        [Test]
        public void TestMany()
        {
            var labeledBooleanType = new LabeledBooleanType[2];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, true);
            var booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType);
            Assert.That(booleanType.Labels, Has.Length.EqualTo(2));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = booleanType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(1));
            });

            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "bolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[1];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[3];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, true);
            labeledBooleanType[2] = new LabeledBooleanType("NAME33", "DISPLAY_NAME33", "NOTES33", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[2];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));
        }
    }
}