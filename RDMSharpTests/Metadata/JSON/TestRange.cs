using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;

namespace RDMSharpTests.Metadata.JSON
{
    public class TestRange
    {
        [Test]
        public void TestByte()
        {
            Range<byte> range = new Range<byte>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 01 - 07"));
        }
        [Test]
        public void TestSByte()
        {
            Range<sbyte> range = new Range<sbyte>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(-3), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 01 - 07"));
        }
        [Test]
        public void TestShort()
        {
            Range<short> range = new Range<short>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 0001 - 0007"));
        }
        [Test]
        public void TestUShort()
        {
            Range<ushort> range = new Range<ushort>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 0001 - 0007"));
        }
        [Test]
        public void TestInt()
        {
            Range<int> range = new Range<int>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 00000001 - 00000007"));
        }
        [Test]
        public void TestUInt()
        {
            Range<uint> range = new Range<uint>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 00000001 - 00000007"));
        }
        [Test]
        public void TestLong()
        {
            Range<long> range = new Range<long>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 0000000000000001 - 0000000000000007"));
        }
        [Test]
        public void TestULong()
        {
            Range<ulong> range = new Range<ulong>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 0000000000000001 - 0000000000000007"));
        }
#if NET7_0_OR_GREATER
        [Test]
        public void TestInt128()
        {
            Range<Int128> range = new Range<Int128>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 00000000000000000000000000000001 - 00000000000000000000000000000007"));
        }
        [Test]
        public void TestUInt128()
        {
            Range<UInt128> range = new Range<UInt128>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 00000000000000000000000000000001 - 00000000000000000000000000000007"));
        }
#endif

        [Test]
        public void TestStringInvalid()
        {
            Range<string> range = new Range<string>("1", "7");
            Assert.That(range.IsInRange("2"), Is.False);
            Assert.That(range.IsInRange("0"), Is.False);
            Assert.That(range.ToString(), Is.EqualTo("Range: 1 - 7"));
        }
    }
}