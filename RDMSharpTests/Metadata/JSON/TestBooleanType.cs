using RDMSharp.Metadata;
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
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", null, "NOTES11", null, false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, true);
            var booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType);
            Assert.That(booleanType.Labels, Has.Length.EqualTo(2));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = booleanType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(1));
            });

            DoParseDataTest(booleanType, false, new byte[] { 0x00 });
            DoParseDataTest(booleanType, true, new byte[] { 0x01 });

            booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", null);
            DoParseDataTest(booleanType, false, new byte[] { 0x00 });
            DoParseDataTest(booleanType, true, new byte[] { 0x01 });

            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "bolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[1];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[3];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, true);
            labeledBooleanType[2] = new LabeledBooleanType("NAME33", "DISPLAY_NAME33", "NOTES33", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));

            labeledBooleanType = new LabeledBooleanType[2];
            labeledBooleanType[0] = new LabeledBooleanType("NAME11", "DISPLAY_NAME11", "NOTES11", null, false);
            labeledBooleanType[1] = new LabeledBooleanType("NAME22", "DISPLAY_NAME22", "NOTES22", null, false);
            Assert.Throws(typeof(ArgumentException), () => booleanType = new BooleanType("NAME", "DISPLAY_NAME", "NOTES", null, "boolean", labeledBooleanType));
        }
        private void DoParseDataTest(BooleanType booleanType, bool value, byte[] expectedData, string message = null)
        {
            var dataTree = new DataTree(booleanType.Name, 0, value);
            var data = new byte[0];
            Assert.DoesNotThrow(() => data = booleanType.ParsePayloadToData(dataTree), message);
            Assert.That(data, Is.EqualTo(expectedData), message);

            byte[] clonaData = new byte[data.Length];
            Array.Copy(data, clonaData, clonaData.Length);
            var parsedDataTree = booleanType.ParseDataToPayload(ref clonaData);
            Assert.That(clonaData, Has.Length.EqualTo(0), message);

            Assert.That(parsedDataTree, Is.EqualTo(dataTree), message);

            //Test for short Data & PDL Issue
            clonaData = new byte[data.Length - 1];
            Array.Copy(data, clonaData, clonaData.Length);
            Assert.DoesNotThrow(() => parsedDataTree = booleanType.ParseDataToPayload(ref clonaData));
            Assert.That(parsedDataTree.Issues, Is.Not.Null);
            Assert.That(parsedDataTree.Value, Is.Not.Null);

            Assert.Throws(typeof(ArithmeticException), () => data = booleanType.ParsePayloadToData(new DataTree("Different Name", dataTree.Index, dataTree.Value)), message);
            Assert.Throws(typeof(ArithmeticException), () => data = booleanType.ParsePayloadToData(new DataTree(dataTree.Name, dataTree.Index, 234)), message);
        }
    }
}