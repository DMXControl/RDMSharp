using RDMSharp.Metadata;
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


            bitTypes = new BitType[3];
            bitTypes[0] = new BitType("NAME11", 2);
            bitTypes[1] = new BitType("NAME22", 4);
            bitTypes[2] = new BitType("NAME33", 15);
            bitFieldType = new BitFieldType("NAME_BIT_FIELD", 16, bitTypes);
            var dataTree= new DataTree(bitFieldType.Name,0,new DataTree[] { new DataTree(bitTypes[0].Name, 0, true), new DataTree(bitTypes[1].Name, 1, true), new DataTree(bitTypes[2].Name, 2, true), });

            DoParseDataTest(bitFieldType, dataTree, new byte[] { 0b00010100, 0b10000000 });

            bitFieldType = new BitFieldType("NAME_BIT_FIELD", 16, bitTypes, true);
            dataTree = new DataTree(bitFieldType.Name, 0, new DataTree[] { new DataTree(bitTypes[0].Name, 0, false), new DataTree(bitTypes[1].Name, 1, false), new DataTree(bitTypes[2].Name, 2, false), });
            Assert.That(bitFieldType.ValueForUnspecified, Is.True);
            DoParseDataTest(bitFieldType, dataTree, new byte[] { 0b11101011, 0b01111111 });
        }
        private void DoParseDataTest(BitFieldType bitFieldType, DataTree dataTree, byte[] expectedData, string message = null)
        {
            Assert.Multiple(() =>
            {
                Assert.That(dataTree.Value, Is.Null);
                Assert.That(dataTree.Children, Is.Not.Null);
                var data = new byte[0];
                Assert.DoesNotThrow(() => data = bitFieldType.ParsePayloadToData(dataTree), message);
                Assert.That(data, Is.EqualTo(expectedData), message);

                byte[] clonaData = new byte[data.Length];
                Array.Copy(data, clonaData, clonaData.Length);
                var parsedDataTree = bitFieldType.ParseDataToPayload(ref clonaData);
                Assert.That(clonaData, Has.Length.EqualTo(0), message);

                Assert.That(parsedDataTree, Is.EqualTo(dataTree), message);
                Assert.That(parsedDataTree.Value, Is.Null);

                //Test for short Data & PDL Issue
                clonaData = new byte[data.Length - 1];
                Array.Copy(data, clonaData, clonaData.Length);
                Assert.DoesNotThrow(() => parsedDataTree = bitFieldType.ParseDataToPayload(ref clonaData));
                Assert.That(parsedDataTree.Issues, Is.Not.Null);
                Assert.That(parsedDataTree.Value, Is.Null);
                Assert.That(parsedDataTree.Children, Is.Not.Null);

                Assert.Throws(typeof(ArithmeticException), () => data = bitFieldType.ParsePayloadToData(new DataTree("Different Name", dataTree.Index, dataTree.Value)), message);
            });
        }
    }
}