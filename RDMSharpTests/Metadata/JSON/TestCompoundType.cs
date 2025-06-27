using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestCompoundType
    {
        [Test]
        public void TestMany()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes(new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);
            Assert.That(compoundType.Subtypes, Has.Length.EqualTo(2));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = compoundType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(2));
            });

            Assert.Throws(typeof(ArgumentException), () => compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "kompound", oneOf));
            Assert.Throws(typeof(ArgumentException), () => compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", null));
            Assert.Throws(typeof(ArgumentException), () => compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", new OneOfTypes[0]));

            Assert.Throws(typeof(ArithmeticException), () => compoundType.validateDataLength(1));
            Assert.Throws(typeof(ArithmeticException), () => compoundType.validateDataLength(3));
            Assert.DoesNotThrow(() => compoundType.validateDataLength(2));
        }

        [Test]
        public void TestParseData1()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME1", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes(new IntegerType<sbyte>("NAME2", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);

            DataTree dataTree = new DataTree(compoundType.Name, 0, new DataTree[] { new DataTree(oneOf[0].ObjectType.Name, 0, (byte)33), new DataTree(oneOf[1].ObjectType.Name, 1, (sbyte)-22) });
            byte[] data = compoundType.ParsePayloadToData(dataTree).SelectMany(en => en).ToArray();
            Assert.That(data, Is.EqualTo(new byte[] { 33, 234 }));

            DataTree dataTreeResult = compoundType.ParseDataToPayload(ref data);
            Assert.That(dataTreeResult, Is.EqualTo(dataTree));
        }

        [Test]
        public void TestParseDataInvalidName()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME1", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes(new IntegerType<sbyte>("NAME2", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);

            DataTree dataTree = new DataTree(compoundType.Name+"INVALID", 0, new DataTree[] { new DataTree(oneOf[0].ObjectType.Name, 0, (byte)33), new DataTree(oneOf[1].ObjectType.Name, 1, (sbyte)-22) });
            Assert.Throws(typeof(ArithmeticException), () => compoundType.ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseDataInvalidNameOnChildren()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME1", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes(new IntegerType<sbyte>("NAME2", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);

            DataTree dataTree = new DataTree(compoundType.Name, 0, new DataTree[] { new DataTree(oneOf[0].ObjectType.Name+ "INVALID", 0, (byte)33), new DataTree(oneOf[1].ObjectType.Name, 1, (sbyte)-22) });
            Assert.Throws(typeof(ArithmeticException), () => compoundType.ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseDataMissingChildrenDataTree()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME1", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes(new IntegerType<sbyte>("NAME2", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);

            DataTree dataTree = new DataTree(compoundType.Name, 0, new DataTree[] { new DataTree(oneOf[0].ObjectType.Name, 0, (byte)33) });
            Assert.Throws(typeof(ArithmeticException), () => compoundType.ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseSubItemIsEmpty()
        {
            OneOfTypes[] oneOf = new OneOfTypes[2];
            oneOf[0] = new OneOfTypes(new IntegerType<byte>("NAME1", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            oneOf[1] = new OneOfTypes();

            var compoundType = new CompoundType("NAME", "DISPLAY_NAME", "NOTES", null, "compound", oneOf);

            DataTree dataTree = new DataTree(compoundType.Name, 0, new DataTree[] { new DataTree(oneOf[0].ObjectType.Name, 0, (byte)33), new DataTree(null, 1, (sbyte)-22) });
            Assert.Throws(typeof(ArithmeticException), () => compoundType.ParsePayloadToData(dataTree));
        }
    }
}