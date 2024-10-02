using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestStringType
    {
        [Test]
        public void TestMany()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, null, null, null);
            Assert.That(stringType.MinLength, Is.Null);
            Assert.That(stringType.MaxLength, Is.Null);


            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(0));
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 1, 32, null, null, null);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(32));
            });

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 1, 32, 0, 34, null);
            Assert.DoesNotThrow(() =>
            {
                PDL pdl = stringType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(0));
                Assert.That(pdl.MaxLength, Is.EqualTo(34));
            });

            Assert.Throws(typeof(ArgumentException), () => stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "sting", null, null, null, null, null, null, null));
        }
        [Test]
        public void TestParseFixedLengthASCII()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, null, 5, true);
            string str = "qwert";
            DataTree dataTree = new DataTree("NAME", 0, str);
            byte[] data = stringType.ParsePayloadToData(dataTree);
            Assert.That(data, Is.EqualTo(new byte[] { 113, 119, 101, 114, 116 }));
            DataTree reverseDataTree = stringType.ParseDataToPayload(ref data);
            Assert.Multiple(() =>
            {
                Assert.That(data, Has.Length.Zero);
                Assert.That(reverseDataTree, Is.EqualTo(dataTree));
            });
        }
        [Test]
        public void TestParseFixedLengthInBytesUTF8()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, minBytes: 8, maxBytes: 8, null);
            string str = "ÄÜÖß";
            DataTree dataTree = new DataTree("NAME", 0, str);
            byte[] data = stringType.ParsePayloadToData(dataTree);
            Assert.That(data, Is.EqualTo(new byte[] { 195, 132, 195, 156, 195, 150, 195, 159 }));
            DataTree reverseDataTree = stringType.ParseDataToPayload(ref data);
            Assert.Multiple(() =>
            {
                Assert.That(data, Has.Length.Zero);
                Assert.That(reverseDataTree, Is.EqualTo(dataTree));
            });
        }
        [Test]
        public void TestParseRangedLengthUTF8()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 4, 6, null, null, null);
            string str = "ÄÜÖß";
            DataTree dataTree = new DataTree("NAME", 0, str);
            byte[] data = stringType.ParsePayloadToData(dataTree);
            Assert.That(data, Is.EqualTo(new byte[] { 195, 132, 195, 156, 195, 150, 195, 159 }));
            DataTree reverseDataTree = stringType.ParseDataToPayload(ref data);
            Assert.Multiple(() =>
            {
                Assert.That(data, Has.Length.Zero);
                Assert.That(reverseDataTree, Is.EqualTo(dataTree));
            });
        }
        [Test]
        public void TestParseRangedLengthUTF8Mixed()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 4, 6, 4, 8, null);
            string str = "ÄUÖS";
            DataTree dataTree = new DataTree("NAME", 0, str);
            byte[] data = stringType.ParsePayloadToData(dataTree);
            Assert.That(data, Is.EqualTo(new byte[] { 195, 132, 85, 195, 150, 83 }));
            DataTree reverseDataTree = stringType.ParseDataToPayload(ref data);
            Assert.Multiple(() =>
            {
                Assert.That(data, Has.Length.Zero);
                Assert.That(reverseDataTree, Is.EqualTo(dataTree));
            });
        }

        [Test]
        public void TestParseExceptions()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 3, 6, 5, 8, null);
            string str = "12";
            DataTree dataTree = new DataTree("NAME FAIL", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
            str = "1234";
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
            str = "1234567";
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));

            stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, 5, 8, null);

            str = "ÄÖÜ4567";
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
            str = "ÄÖÜÜÖÄ";
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
            str = null;
            dataTree = new DataTree("NAME", 0, str);
            Assert.Throws(typeof(ArithmeticException), () => stringType.ParsePayloadToData(dataTree));
        }

        [Test]
        public void TestParseBadFormatedData1()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, null, null, 2, 8, null);
            byte[] data = new byte[] { 195, 132, 0, 0, 0, 0 };
            var dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("Ä"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));

            data = new byte[] { 195, 132, 0, 0, 119, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("Ä"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(2));

            data = new byte[] { 0, 0, 0, 0, 0, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo(string.Empty));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));

            data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo(string.Empty));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(2));

            data = new byte[] { };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo(string.Empty));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));

            data = new byte[] { 195, 132, 195, 132, 195, 132, 195, 132, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("ÄÄÄÄ"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));
            data = new byte[] { 195, 132, 195, 132, 195, 132, 195, 132, 119, 119, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("ÄÄÄÄ"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));
        }
        [Test]
        public void TestParseBadFormatedData2()
        {
            var stringType = new StringType("NAME", "DISPLAY_NAME", "NOTES", null, "string", null, null, 2, 4, null, null, null);
            byte[] data = new byte[] { 195, 132, 0, 0, 0, 0 };
            var dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("Ä"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(2));

            data = new byte[] { 195, 132, 119, 119, 119, 119, 0 };
            dataTree = stringType.ParseDataToPayload(ref data);
            Assert.That(dataTree.Value, Is.EqualTo("Äwwww"));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(1));
        }
    }
}