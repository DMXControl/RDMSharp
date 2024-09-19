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
        }
#endif
    }
}