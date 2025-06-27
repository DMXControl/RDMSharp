using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestReferenceType
    {
        [Test]
        public void TestMany()
        {
            var referenceType = new ReferenceType("#/get_request/0");
            Assert.That(referenceType.Command, Is.EqualTo(Command.ECommandDublicate.GetRequest));
            Assert.That(referenceType.Pointer, Is.EqualTo(0));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = referenceType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(0));
                Assert.That(pdl.MinLength, Is.Null);
                Assert.That(pdl.MaxLength, Is.Null);
            });

            Assert.Throws(typeof(ArgumentException), () => referenceType = new ReferenceType("%/get_request/0"));
        }
        [Test]
        public void TestParse()
        {
            var referenceType = new ReferenceType("#/get_request/0", new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "uid", null, null));
            Assert.That(referenceType.GetDataLength().Value, Is.EqualTo(6));
            Assert.That(referenceType.ReferencedObject, Is.Not.Null);
            var uid = new UID(0x4646, 0x12345678);
            var data = referenceType.ParsePayloadToData(new DataTree(referenceType.ReferencedObject.Name, 0, uid)).SelectMany(en => en).ToArray();
            Assert.That(data, Is.EqualTo(new byte[] { 0x46, 0x46, 0x12, 0x34, 0x56, 0x78 }));
            var dataTree = referenceType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(uid));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "uid", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
    }
}