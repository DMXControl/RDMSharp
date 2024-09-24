using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestIntegerType
    {
        [Test]
        public void TestManyInt8()
        {
            var integerType = new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.Int8));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(1));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<sbyte>(integerType, -71, new byte[] { 185 });
                DoParseDataTest<sbyte>(integerType, sbyte.MinValue, new byte[] { 128 }, nameof(sbyte.MinValue));
                DoParseDataTest<sbyte>(integerType, sbyte.MaxValue, new byte[] { 127 }, nameof(sbyte.MaxValue));
                DoParseDataTest<sbyte>(integerType, 0, new byte[] { 0 }, "Zero");
                DoParseDataTest<sbyte>(integerType, 1, new byte[] { 1 }, "One");
            });
        }
        [Test]
        public void TestManyUInt8()
        {
            var integerType = new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.UInt8));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(1));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<byte>(integerType, 71, new byte[] { 71 });
                DoParseDataTest<byte>(integerType, byte.MinValue, new byte[] { 0 }, nameof(byte.MinValue));
                DoParseDataTest<byte>(integerType, byte.MaxValue, new byte[] { 255 }, nameof(byte.MaxValue));
                DoParseDataTest<byte>(integerType, 0, new byte[] { 0 }, "Zero");
                DoParseDataTest<byte>(integerType, 1, new byte[] { 1 }, "One");
            });
        }
        [Test]
        public void TestManyInt16()
        {
            var integerType = new IntegerType<short>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int16, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.Int16));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(2));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<short>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt16, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<short>(integerType, -13245, new byte[] { 204, 67 });
                DoParseDataTest<short>(integerType, short.MinValue, new byte[] { 128, 0 }, nameof(short.MinValue));
                DoParseDataTest<short>(integerType, short.MaxValue, new byte[] { 127,255 }, nameof(short.MaxValue));
                DoParseDataTest<short>(integerType, 0, new byte[] { 0, 0 }, "Zero");
                DoParseDataTest<short>(integerType, 1, new byte[] { 0, 1 }, "One");
            });
        }
        [Test]
        public void TestManyUInt16()
        {
            var integerType = new IntegerType<ushort>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt16, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.UInt16));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(2));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<ushort>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int16, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<ushort>(integerType, 65432, new byte[] { 255, 152 });
                DoParseDataTest<ushort>(integerType, ushort.MinValue, new byte[] { 0, 0 }, nameof(ushort.MinValue));
                DoParseDataTest<ushort>(integerType, ushort.MaxValue, new byte[] { 255, 255 }, nameof(ushort.MaxValue));
                DoParseDataTest<ushort>(integerType, 0, new byte[] { 0, 0 }, "Zero");
                DoParseDataTest<ushort>(integerType, 1, new byte[] { 0, 1 }, "One");
            });
        }
        [Test]
        public void TestManyInt32()
        {
            var integerType = new IntegerType<int>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int32, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.Int32));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(4));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<int>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt32, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<int>(integerType, -13243567, new byte[] { 255, 53, 235, 81 });
                DoParseDataTest<int>(integerType, int.MinValue, new byte[] { 128, 0, 0, 0 }, nameof(int.MinValue));
                DoParseDataTest<int>(integerType, int.MaxValue, new byte[] { 127, 255, 255, 255 }, nameof(int.MaxValue));
                DoParseDataTest<int>(integerType, 0, new byte[] { 0, 0, 0, 0 }, "Zero");
                DoParseDataTest<int>(integerType, 1, new byte[] { 0, 0, 0, 1 }, "One");
            });
        }
        [Test]
        public void TestManyUInt32()
        {
            var integerType = new IntegerType<uint>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt32, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.UInt32));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(4));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<uint>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int32, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<uint>(integerType, 75643244, new byte[] { 4, 130, 57, 108 });
                DoParseDataTest<uint>(integerType, uint.MinValue, new byte[] { 0, 0, 0, 0 }, nameof(uint.MinValue));
                DoParseDataTest<uint>(integerType, uint.MaxValue, new byte[] { 255, 255, 255, 255 }, nameof(uint.MaxValue));
                DoParseDataTest<uint>(integerType, 0, new byte[] { 0, 0, 0, 0 }, "Zero");
                DoParseDataTest<uint>(integerType, 1, new byte[] { 0, 0, 0, 1 }, "One");
            });
        }
        [Test]
        public void TestManyInt64()
        {
            var integerType = new IntegerType<long>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int64, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.Int64));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(8));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<long>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt64, null, null, null, null, null, null));
            
            Assert.Multiple(() =>
            {
                DoParseDataTest<long>(integerType, -2136456776545, new byte[] { 255, 255, 254, 14, 145, 64, 180, 159 });
                DoParseDataTest<long>(integerType, long.MinValue, new byte[] { 128, 0, 0, 0, 0, 0, 0, 0 }, nameof(long.MinValue));
                DoParseDataTest<long>(integerType, long.MaxValue, new byte[] { 127, 255, 255, 255, 255, 255, 255, 255 }, nameof(long.MaxValue));
                DoParseDataTest<long>(integerType, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "Zero");
                DoParseDataTest<long>(integerType, 1, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, "One");
            });
        }
        [Test]
        public void TestManyUInt64()
        {
            var integerType = new IntegerType<ulong>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt64, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.UInt64));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(8));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<ulong>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int64, null, null, null, null, null, null));

            Assert.Multiple(() =>
            {
                DoParseDataTest<ulong>(integerType, 345676543456543, new byte[] { 0, 1, 58, 100, 23, 148, 117, 31 });
                DoParseDataTest<ulong>(integerType, ulong.MinValue, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, nameof(ulong.MinValue));
                DoParseDataTest<ulong>(integerType, ulong.MaxValue, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, nameof(ulong.MaxValue));
                DoParseDataTest<ulong>(integerType, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "Zero");
                DoParseDataTest<ulong>(integerType, 1, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, "One");
            });
        }
#if NET7_0_OR_GREATER
        [Test]
        public void TestManyInt128()
        {
            var integerType = new IntegerType<Int128>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int128, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.Int128));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(16));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<Int128>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt128, null, null, null, null, null, null));
            Assert.Multiple(() =>
            {
                DoParseDataTest<Int128>(integerType, new Int128(2725151552362626646, 5278987657876567), new byte[] { 37, 209, 174, 229, 253, 173, 190, 86, 0, 18, 193, 54, 24, 31, 20, 87 });
                DoParseDataTest<Int128>(integerType, Int128.MinValue, new byte[] { 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, nameof(Int128.MinValue));
                DoParseDataTest<Int128>(integerType, Int128.MaxValue, new byte[] { 127, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, nameof(Int128.MaxValue));
                DoParseDataTest<Int128>(integerType, Int128.Zero, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, nameof(Int128.Zero));
                DoParseDataTest<Int128>(integerType, Int128.One, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, nameof(Int128.One));
            });
        }
        [Test]
        public void TestManyUInt128()
        {
            var integerType = new IntegerType<UInt128>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt128, null, null, null, null, null, null);
            Assert.That(integerType.Type, Is.EqualTo(EIntegerType.UInt128));

            Assert.DoesNotThrow(() =>
            {
                PDL pdl = integerType.GetDataLength();
                Assert.That(pdl.Value, Is.EqualTo(16));
            });

            Assert.Throws(typeof(ArgumentException), () => integerType = new IntegerType<UInt128>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int128, null, null, null, null, null, null));

            Assert.Multiple(() =>
            {
                DoParseDataTest<UInt128>(integerType, new UInt128(2725751552362626686, 5278987657876567), new byte[] { 37, 211, 208, 152, 96, 139, 62, 126, 0, 18, 193, 54, 24, 31, 20, 87 });
                DoParseDataTest<UInt128>(integerType, UInt128.MinValue, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, nameof(UInt128.MinValue));
                DoParseDataTest<UInt128>(integerType, UInt128.MaxValue, new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, nameof(UInt128.MaxValue));
                DoParseDataTest<UInt128>(integerType, UInt128.Zero, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, nameof(UInt128.Zero));
                DoParseDataTest<UInt128>(integerType, UInt128.One, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, nameof(UInt128.One));
            });
        }
#endif

        [Test]
        public void TestPrefix1024()
        {
            CommonPropertiesForNamed[] types = new CommonPropertiesForNamed[]
            {
                 new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, 10, 2),
                 new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, 10, 2),
                 new IntegerType<short>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int16, null, null, null, null, 10, 2),
                 new IntegerType<ushort>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt16, null, null, null, null, 10, 2),
                 new IntegerType<int>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int32, null, null, null, null, 10, 2),
                 new IntegerType<uint>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt32, null, null, null, null, 10, 2),
                 new IntegerType<long>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int64, null, null, null, null, 10, 2),
                 new IntegerType<ulong>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt64, null, null, null, null, 10, 2),
            };
            foreach (CommonPropertiesForNamed integerType in types)
            {
                Assert.That(((IIntegerType)integerType).PrefixBase, Is.EqualTo(2));
                Assert.That(((IIntegerType)integerType).PrefixPower, Is.EqualTo(10));
                Assert.That(((IIntegerType)integerType).PrefixMultiplyer, Is.EqualTo(1024));

                uint pdl = integerType.GetDataLength().Value.Value;
                Assert.Multiple(() =>
                {
                    var data = new byte[pdl];
                    var dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(0));
                    var parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    Assert.That(parsedData, Is.EqualTo(data));

                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(1024));
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    Assert.That(parsedData, Is.EqualTo(data));

                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(102400));
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    Assert.That(parsedData, Is.EqualTo(data));
                });
            }
        }
        [Test]
        public void TestPrefix1024Negativ()
        {
            CommonPropertiesForNamed[] types = new CommonPropertiesForNamed[]
            {
                 new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, 1, -1024),
                 new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, 1, -1024),
                 new IntegerType<short>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int16, null, null, null, null, 1, -1024),
                 new IntegerType<ushort>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt16, null, null, null, null, 1, -1024),
                 new IntegerType<int>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int32, null, null, null, null, 1, -1024),
                 new IntegerType<uint>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt32, null, null, null, null, 1, -1024),
                 new IntegerType<long>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int64, null, null, null, null, 1, -1024),
                 new IntegerType<ulong>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt64, null, null, null, null, 1, -1024),
            };
            foreach (CommonPropertiesForNamed integerType in types)
            {
                Assert.That(((IIntegerType)integerType).PrefixBase, Is.EqualTo(-1024));
                Assert.That(((IIntegerType)integerType).PrefixPower, Is.EqualTo(1));
                Assert.That(((IIntegerType)integerType).PrefixMultiplyer, Is.EqualTo(-1024));

                uint pdl = integerType.GetDataLength().Value.Value;
                Assert.Multiple(() =>
                {
                    string message= ((IIntegerType)integerType).Type.ToString();
                    var data = new byte[pdl];
                    var dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(0), message);
                    var parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    Assert.That(parsedData, Is.EqualTo(data), message);

                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(-1024), message);
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    Assert.That(parsedData, Is.EqualTo(data), message);

                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(-102400), message);
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    Assert.That(parsedData, Is.EqualTo(data), message);
                });
            }
        }
        [Test]
        public void TestPrefix4Decimals()
        {
            CommonPropertiesForNamed[] types = new CommonPropertiesForNamed[]
            {
                 new IntegerType<sbyte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int8, null, null, null, null, -4, 10),
                 new IntegerType<byte>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt8, null, null, null, null, -4, 10),
                 new IntegerType<short>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int16, null, null, null, null, -4, 10),
                 new IntegerType<ushort>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt16, null, null, null, null, -4, 10),
                 new IntegerType<int>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int32, null, null, null, null, -4, 10),
                 new IntegerType<uint>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt32, null, null, null, null, -4, 10),
                 new IntegerType<long>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.Int64, null, null, null, null, -4, 10),
                 new IntegerType<ulong>("NAME", "DISPLAY_NAME", "NOTES", null, EIntegerType.UInt64, null, null, null, null, -4, 10),
            };
            foreach (CommonPropertiesForNamed integerType in types)
            {
                Assert.That(((IIntegerType)integerType).PrefixBase, Is.EqualTo(10));
                Assert.That(((IIntegerType)integerType).PrefixPower, Is.EqualTo(-4));
                Assert.That(((IIntegerType)integerType).PrefixMultiplyer, Is.EqualTo(0.0001));

                uint pdl = integerType.GetDataLength().Value.Value;
                Assert.Multiple(() =>
                {
                    var data = new byte[pdl];
                    var dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(0));
                    var parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    Assert.That(parsedData, Is.EqualTo(data));

                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(0.0001));
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 1;
                    Assert.That(parsedData, Is.EqualTo(data));

                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    dataTree = integerType.ParseDataToPayload(ref data);
                    Assert.That(dataTree.Value, Is.EqualTo(0.01));
                    parsedData = integerType.ParsePayloadToData(dataTree);
                    data = new byte[pdl];
                    data[data.Length - 1] = 100;
                    Assert.That(parsedData, Is.EqualTo(data));
                });
            }
        }

        private void DoParseDataTest<T>(IntegerType<T> integerType, T value, byte[] expectedData, string message = null)
        {
            var dataTree = new DataTree(integerType.Name, 0, value);
            var data = new byte[0];
            Assert.DoesNotThrow(() => data = integerType.ParsePayloadToData(dataTree), message);
            Assert.That(data, Is.EqualTo(expectedData), message);

            byte[] clonaData = new byte[data.Length];
            Array.Copy(data, clonaData, clonaData.Length);
            var parsedDataTree = integerType.ParseDataToPayload(ref clonaData);
            Assert.That(clonaData, Has.Length.EqualTo(0), message);

            Assert.That(parsedDataTree, Is.EqualTo(dataTree), message);

            //Test for short Data & PDL Issue
            clonaData = new byte[data.Length - 1];
            Array.Copy(data, clonaData, clonaData.Length);
            Assert.DoesNotThrow(() => parsedDataTree = integerType.ParseDataToPayload(ref clonaData));
            Assert.That(parsedDataTree.Issues, Is.Not.Null);
            Assert.That(parsedDataTree.Value, Is.Not.Null);

            Assert.Throws(typeof(ArithmeticException), () => data = integerType.ParsePayloadToData(new DataTree("Different Name", dataTree.Index, dataTree.Value)), message);
        }
    }
}