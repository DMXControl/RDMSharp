using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestBytesType
    {
        [Test]
        public void TestMany()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, null, null);
            Assert.That(bytesType.MinLength, Is.Null);
            Assert.That(bytesType.MaxLength, Is.Null);

            bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 1, null);
            Assert.That(bytesType.MinLength, Is.EqualTo(1));
            Assert.That(bytesType.MaxLength, Is.Null);

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = bytesType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(PDL.MAX_LENGTH));
            });

            bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 1, 3);
            Assert.That(bytesType.MinLength, Is.EqualTo(1));
            Assert.That(bytesType.MaxLength, Is.EqualTo(3));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = bytesType.GetDataLength();
                Assert.That(pdl.Value, Is.Null);
                Assert.That(pdl.MinLength, Is.EqualTo(1));
                Assert.That(pdl.MaxLength, Is.EqualTo(3));
            });

            Assert.Throws(typeof(ArgumentException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bites", null, 1, 5));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 6, 5));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 4294567890, null));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, 2, 4294567890));
        }
        [Test]
        public void TestParseUID()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "uid", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(6));
            var uid = new UID(0x4646, 0x12345678);
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, uid));
            Assert.That(data, Is.EqualTo(new byte[] { 0x46, 0x46, 0x12, 0x34, 0x56, 0x78 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(uid));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "uid", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseUIDArrayEmpty()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "uid[]", null, null);
            Assert.That(bytesType.GetDataLength().MinLength, Is.EqualTo(0));
            var uidArray = new UID[0];
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, uidArray));
            Assert.That(data, Is.EqualTo(new byte[0]));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Null);
        }
        [Test]
        public void TestParseUIDArray()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "uid[]", null, null);
            Assert.That(bytesType.GetDataLength().MinLength, Is.EqualTo(0));
            var uidArray = new UID[] { new UID(0x4646, 0x12345678) , new UID(0x4646, 0x12345678) };
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, uidArray));
            Assert.That(data, Is.EqualTo(new byte[] { 0x46, 0x46, 0x12, 0x34, 0x56, 0x78, 0x46, 0x46, 0x12, 0x34, 0x56, 0x78 }));
            var corrupData = new List<byte>();
            corrupData.AddRange(data);
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.EqualTo(uidArray));

            corrupData.Add(1);
            corrupData.Add(2);
            data= corrupData.ToArray();
            dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(2));
            Assert.That(data, Is.EqualTo(new byte[] { 1, 2 }));
            Assert.That(dataTree.Value, Is.EqualTo(uidArray));
            Assert.That(dataTree.Issues, Has.Length.EqualTo(2));
        }

        [Test]
        public void TestParseIPv4()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "ipv4", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(4));
            var ipv4 = new IPv4Address(192,168,178,254);
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, ipv4));
            Assert.That(data, Is.EqualTo(new byte[] { 192, 168, 178, 254 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(ipv4));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "ipv4", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseIPv6()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "ipv6", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(16));
            var ipv6 = IPv6Address.LocalHost;
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, ipv6));
            Assert.That(data, Is.EqualTo(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(ipv6));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "ipv6", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseMAC()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "mac-address", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(6));
            var mac = new MACAddress(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC });
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, mac));
            Assert.That(data, Is.EqualTo(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(mac));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "mac-address", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseGUID()
        {
            string[] formates = new string[] { "uuid", "guid" };
            foreach (var formate in formates)
            {
                var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", formate, null, null);
                Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(16));
                var guid = new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
                var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, guid));
                Assert.That(data, Is.EqualTo(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
                var dataTree = bytesType.ParseDataToPayload(ref data);
                Assert.That(data, Has.Length.EqualTo(0));
                Assert.That(dataTree.Value, Is.Not.Null);
                Assert.That(dataTree.Value, Is.EqualTo(guid));

                Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", formate, null, null).ParsePayloadToData(dataTree));
                Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
            }
        }
        [Test]
        public void TestParseDouble()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "double", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(8));
            var _double = 0.252536d;
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, _double));
            Assert.That(data, Is.EqualTo(new byte[] { 142, 2, 68, 193, 140, 41, 208, 63 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(_double));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "double", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseFloat()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "float", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(4));
            var _float = 0.252536f;
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, _float));
            Assert.That(data, Is.EqualTo(new byte[] { 102, 76, 129, 62 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(_float));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "float", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParsePID()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "pid", null, null);
            Assert.That(bytesType.GetDataLength().Value, Is.EqualTo(2));
            var pid = ERDM_Parameter.CURVE;
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, pid));
            Assert.That(data, Is.EqualTo(new byte[] { 0x03, 0x043 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(pid));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "pid", null, null).ParsePayloadToData(dataTree));
            Assert.Throws(typeof(ArithmeticException), () => new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "xyz", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseASCII()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "ascii", null, null);
            var ascii = "This is ASCII";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, ascii));
            Assert.That(data, Is.EqualTo(new byte[] { 84, 104, 105, 115, 32, 105, 115, 32, 65, 83, 67, 73, 73 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(ascii));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "ascii", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseUTF8()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "utf8", null, null);
            var utf8 = "äöüß€!";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, utf8));
            Assert.That(data, Is.EqualTo(new byte[] { 195, 164, 195, 182, 195, 188, 195, 159, 226, 130, 172, 33 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(utf8));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "utf8", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseUTF32()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "utf32", null, null);
            var utf32 = "😊🚀🌍💡📚✈️";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, utf32));
            Assert.That(data, Is.EqualTo(new byte[] { 10, 246, 1, 0, 128, 246, 1, 0, 13, 243, 1, 0, 161, 244, 1, 0, 218, 244, 1, 0, 8, 39, 0, 0, 15, 254, 0, 0 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(utf32));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "utf32", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseUnicode()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "unicode", null, null);
            var unicode = "ÄÖÜß😊";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, unicode));
            Assert.That(data, Is.EqualTo(new byte[] { 196, 0, 214, 0, 220, 0, 223, 0, 61, 216, 10, 222 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(unicode));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "unicode", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseBigEndianUnicode()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "big_edian_unicode", null, null);
            var big_edian_unicode = "ÄÖÜß😊";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, big_edian_unicode));
            Assert.That(data, Is.EqualTo(new byte[] { 0, 196, 0, 214, 0, 220, 0, 223, 216, 61, 222, 10 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(big_edian_unicode));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "big_edian_unicode", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseLatin1()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "latin1", null, null);
            var latin1 = "Café"; ;
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, latin1));
            Assert.That(data, Is.EqualTo(new byte[] { 67, 97, 102, 233 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(latin1));

            Assert.Throws(typeof(ArithmeticException), () => new BytesType("Other Name", "DISPLAY_NAME", "NOTES", null, "bytes", "latin1", null, null).ParsePayloadToData(dataTree));
        }
        [Test]
        public void TestParseFallbackString()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, null, null);
            var utf8 = "äöüß€!";
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, utf8));
            Assert.That(data, Is.EqualTo(new byte[] { 195, 164, 195, 182, 195, 188, 195, 159, 226, 130, 172, 33 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
        }
        [Test]
        public void TestParseFallbackUTF8Array()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "utf8[]", null, null);
            var utf8 = "äöüß€!";
            var array = new string[] { utf8, utf8 };
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, array));
            Assert.That(data, Is.EqualTo(new byte[] { 195, 164, 195, 182, 195, 188, 195, 159, 226, 130, 172, 33, 0, 195, 164, 195, 182, 195, 188, 195, 159, 226, 130, 172, 33, 0 }));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
        }
        [Test]
        public void TestParseFallbackByteArray()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", null, null, null);
            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var data = bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, bytes));
            Assert.That(data, Is.EqualTo(bytes));
            var dataTree = bytesType.ParseDataToPayload(ref data);
            Assert.That(data, Has.Length.EqualTo(0));
            Assert.That(dataTree.Value, Is.Not.Null);
            Assert.That(dataTree.Value, Is.EqualTo(bytes));
        }
        [Test]
        public void TestParseFallbackUnknownType()
        {
            var bytesType = new BytesType("NAME", "DISPLAY_NAME", "NOTES", null, "bytes", "UNKNOWN", null, null);
            Assert.Throws(typeof(ArithmeticException), () => bytesType.ParsePayloadToData(new DataTree(bytesType.Name, 0, DateTime.Now)));
        }
    }
}