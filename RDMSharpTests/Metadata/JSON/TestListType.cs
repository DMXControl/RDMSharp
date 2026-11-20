using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestListType
    {
        [Test]
        public void TestMany()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), null, null);
            Assert.That(listType.MinItems, Is.Null);
            Assert.That(listType.MaxItems, Is.Null);

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = listType.GetDataLength();
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            Assert.Throws(typeof(ArgumentException), () => listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "lost", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), null, null));
            Assert.Throws(typeof(ArgumentException), () => listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(), null, null));

            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, 3, 10, null);
            listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(stringType), 1, 3);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = listType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(3));
                Assert.That(pdl.MaxLength, Is.EqualTo(30));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null,32, 32, null);
            listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(stringType), 1, 3);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = listType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(32));
                Assert.That(pdl.MaxLength, Is.EqualTo(96));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, 32, 32, null);
            listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(stringType), null, 3);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = listType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.Zero);
                Assert.That(pdl.MaxLength, Is.EqualTo(96));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, 32, 32, null);
            listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(stringType), 1, null);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = listType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(32));
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            Assert.Throws(typeof(ArithmeticException), () => listType.validateDataLength(0));
            Assert.Throws(typeof(ArithmeticException), () => listType.validateDataLength(3));
            Assert.DoesNotThrow(() => listType.validateDataLength(32));
            Assert.DoesNotThrow(() => listType.validateDataLength(58905));
        }

        [Test]
        public void TestParseData1()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 1, 5);
            var dataTree = new DataTree(listType.Name, 0, children: new DataTree[] { new DataTree("NAME", 0, (byte)11), new DataTree("NAME", 1, (byte)22), new DataTree("NAME", 2, (byte)33) });
            var data = listType.ParsePayloadToData(dataTree).SelectMany(en => en).ToArray();
            Assert.That(data, Is.EqualTo(new byte[] { 11, 22, 33 }));

            var dataTreeResult = listType.ParseDataToPayload(ref data);
            Assert.That(dataTreeResult, Is.EqualTo(dataTree));

            listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 3, 3);
            data = listType.ParsePayloadToData(dataTree).SelectMany(en => en).ToArray();
            Assert.That(data, Is.EqualTo(new byte[] { 11, 22, 33 }));

            dataTreeResult = listType.ParseDataToPayload(ref data);
            Assert.That(dataTreeResult, Is.EqualTo(dataTree));
        }
        [Test]
        public void TestParseDataShortMin()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 4, 5);
            var dataTree = new DataTree(listType.Name, 0, children: new DataTree[] { new DataTree("NAME", 0, (byte)11), new DataTree("NAME", 1, (byte)22), new DataTree("NAME", 2, (byte)33) });
            
            var data = new byte[] { 11, 22, 33 };
            var dataTreeResult = listType.ParseDataToPayload(ref data);
            Assert.That(dataTreeResult.Issues, Has.Length.EqualTo(2));
        }
        [Test]
        public void TestParseDataExceedMax()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 1, 2);
            var dataTree = new DataTree(listType.Name, 0, children: new DataTree[] { new DataTree("NAME", 0, (byte)11), new DataTree("NAME", 1, (byte)22), new DataTree("NAME", 2, (byte)33) });

            var data = new byte[] { 11, 22, 33 };
            var dataTreeResult = listType.ParseDataToPayload(ref data);
            Assert.That(dataTreeResult.Issues, Has.Length.EqualTo(2));
        }
        /// <summary>
        /// A real device answered LANGUAGE_CAPABILITIES with 0x0000 (no valid ISO 639-1 code).
        /// Every item of a list of fixed size Strings (minLength == maxLength) has to consume its bytes,
        /// otherwise the list never ends.
        /// </summary>
        [Test]
        public void TestParseDataFixedSizeStringItemsWithNul()
        {
            var listType = new ListType("language_codes", "DISPLAY_NAME", "NOTES", null, "list",
                new OneOfTypes(new StringType("language_code", "DISPLAY_NAME", "NOTES", null, "string", null, "^[a-zA-Z]{2}$", 2, 2, null, null, null)), null, null);

            DataTree parse(byte[] data)
            {
                DataTree result = default;
                var task = Task.Run(() => { result = listType.ParseDataToPayload(ref data); });
                Assert.That(task.Wait(TimeSpan.FromSeconds(5)), Is.True, "parsing must not hang");
                Assert.That(data, Is.Empty, "all bytes have to be consumed");
                return result;
            }

            var dataTree = parse(new byte[] { 0x00, 0x00 });
            Assert.That(dataTree.Children, Has.Length.EqualTo(1));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(dataTree.Children![0].Value, Is.EqualTo(string.Empty));
                Assert.That(dataTree.Children[0].Issues, Is.Not.Empty, "0x0000 is no valid language code");
            }

            dataTree = parse(new byte[] { 0x65, 0x6e, 0x00, 0x00, 0x64, 0x65 });
            Assert.That(dataTree.Children!.Select(c => c.Value), Is.EqualTo(new object[] { "en", string.Empty, "de" }));
        }

        [Test]
        public void TestParseDataNameInvalid()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 1, 2);
            var dataTree = new DataTree(listType.Name+"INVALID", 0, children: new DataTree[] { new DataTree("NAME", 0, (byte)11), new DataTree("NAME", 1, (byte)22), new DataTree("NAME", 2, (byte)33) });
            
            Assert.Throws(typeof(ArithmeticException), () => listType.ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseDataChildNameInvalid()
        {
            var listType = new ListType("NAME", "DISPLAY_NAME", "NOTES", null, "list", new OneOfTypes(new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null)), 1, 2);
            var dataTree = new DataTree(listType.Name, 0, children: new DataTree[] { new DataTree("NAME" + "INVALID", 0, (byte)11), new DataTree("NAME", 1, (byte)22), new DataTree("NAME", 2, (byte)33) });

            Assert.Throws(typeof(ArithmeticException), () => listType.ParsePayloadToData(dataTree));
        }
    }
}