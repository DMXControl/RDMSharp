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
        }
        [Test]
        public void TestSByte()
        {
            Range<sbyte> range = new Range<sbyte>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(-3), Is.False);
        }
        [Test]
        public void TestShort()
        {
            Range<short> range = new Range<short>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestUShort()
        {
            Range<ushort> range = new Range<ushort>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestInt()
        {
            Range<int> range = new Range<int>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestUInt()
        {
            Range<uint> range = new Range<uint>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestLong()
        {
            Range<long> range = new Range<long>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestULong()
        {
            Range<ulong> range = new Range<ulong>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
#if NET7_0_OR_GREATER
        [Test]
        public void TestInt128()
        {
            Range<Int128> range = new Range<Int128>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
        [Test]
        public void TestUInt128()
        {
            Range<UInt128> range = new Range<UInt128>(1, 7);
            Assert.That(range.IsInRange(2), Is.True);
            Assert.That(range.IsInRange(0), Is.False);
        }
#endif
    }
}