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
        }
    }
}