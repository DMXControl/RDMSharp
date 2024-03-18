using RDMSharp.ParameterWrapper;

namespace RDMSharpTests.RDM
{
    public class RDMUID_Test
    {
        [Test]
        public void RDMUID_ToString()
        {
            Assert.That(new RDMUID(0xFFFF, 0xABCDEF98).ToString(), Is.EqualTo("FFFF:ABCDEF98"));
            Assert.That(new RDMUID(0xA053, 0x00335612).ToString(), Is.EqualTo("A053:00335612"));
            Assert.That(new RDMUID(0x0001, 0x12345678).ToString(), Is.EqualTo("0001:12345678"));
            Assert.That(new RDMUID(1, 2).ToString(), Is.EqualTo("0001:00000002"));
            Assert.That(RDMUID.Empty.ToString(), Is.EqualTo("0000:00000000"));
        }

        [Test]
        public void RDMUID_FromString()
        {
            var uid = new RDMUID(0x0123456789ab);
            Assert.That(new RDMUID("0123:456789ab"), Is.EqualTo(uid));
            Assert.That(new RDMUID("0123.456789ab"), Is.EqualTo(uid));
            Assert.That(new RDMUID("0123-456789ab"), Is.EqualTo(uid));
            Assert.That(new RDMUID("0123456789ab"), Is.EqualTo(uid));
            Assert.Throws(typeof(FormatException), () => new RDMUID("0123+456789ab"));
            Assert.Throws(typeof(FormatException), () => new RDMUID("0123456789a"));
            Assert.Throws(typeof(FormatException), () => new RDMUID("0123456789ag"));
        }

        [Test]
        public void RDMUID_FromULong()
        {
            Assert.That(new RDMUID(0xFFFF, 0xABCDEF98), Is.EqualTo(new RDMUID(0xFFFFABCDEF98)));
            Assert.That(new RDMUID(0xFF1F, 0xABCD0F98), Is.EqualTo(new RDMUID(0xFF1FABCD0F98)));

            Assert.That(new RDMUID(EManufacturer.ESTA, 1), Is.EqualTo(new RDMUID(0x0001)));
        }

        [Test]
        public void RDMUID_CastToULong()
        {
            Assert.That((ulong)new RDMUID(0xFFFF, 0xABCDEF98), Is.EqualTo(0xFFFFABCDEF98u));
            Assert.That((ulong)new RDMUID(0xFF1F, 0xABCD0F98), Is.EqualTo(0xFF1FABCD0F98u));

            Assert.That((ulong)new RDMUID(EManufacturer.ESTA, 1), Is.EqualTo(0x0001u));
        }

        [Test]
        public void RDMUID_Equals()
        {
            Assert.That(new RDMUID(), Is.EqualTo(RDMUID.Empty));
            Assert.That(new RDMUID(EManufacturer.ESTA, 0), Is.EqualTo(RDMUID.Empty));
            Assert.That(new RDMUID(EManufacturer.ESTA, 0), Is.Not.EqualTo(null));
            Assert.That(new RDMUID(EManufacturer.ESTA, 0), Is.Not.EqualTo(new RDMUID(0xFFFF, 0xABCDEF98)));
            Assert.That(new RDMUID(ushort.MaxValue, uint.MaxValue), Is.EqualTo(RDMUID.Broadcast));

            var uid = new RDMUID(0xFFFF, 0xABCDEF98);
            Assert.That((object)uid, Is.EqualTo((object)new RDMUID(0xFFFF, 0xABCDEF98)));
            Assert.That(uid, Is.EqualTo((object)new RDMUID(0xFFFF, 0xABCDEF98)));
            Assert.That(uid, Is.Not.EqualTo((object)new RDMUID(0xFFEE, 0xABCDEF98)));
            Assert.That((object)uid, Is.Not.EqualTo((object)new RDMUID(0xFFEE, 0xABCDEF98)));
            Assert.That(object.Equals(uid, new RDMUID(0xFFFF, 0xABCDEF98)), Is.True);
            Assert.That(object.Equals(uid, new RDMUID(0xFFFF, 0xABCDEF48)), Is.False);
            Assert.That(object.Equals(uid, null), Is.False);
            Assert.That(object.Equals(null, uid), Is.False);
            Assert.That(object.Equals(uid, 0xABCDEF98), Is.False);
            Assert.That((object)uid, Is.Not.EqualTo(new RDMUID(0xF4FF, 0xABCD3F98)));
            Assert.That(uid, Is.Not.EqualTo((object)new RDMUID(0xF4FF, 0xABCD3F98)));
            Assert.That((object)uid, Is.Not.EqualTo(null));
            Assert.That(uid, Is.Not.EqualTo(null));

            Assert.That(uid == new RDMUID(0xFFFF, 0xABCDEF98), Is.True);
            Assert.That(uid != new RDMUID(0xFFFF, 0x4BCD3F98), Is.True);
            Assert.That(new RDMUID(0xFFFF, 0x4BCD3F98) < new RDMUID(0xFFFF, 0xABCD3F98), Is.True);
            Assert.That(new RDMUID(0xFFFF, 0x4BCD3F98) > new RDMUID(0xFFFF, 0xABCD3F98), Is.False);
            Assert.That(new RDMUID(0xFFFF, 0x4BCD3F98) <= new RDMUID(0xFFFF, 0xABCD3F98), Is.True);
            Assert.That(new RDMUID(0xFFFF, 0x4BCD3F98) >= new RDMUID(0xFFFF, 0xABCD3F98), Is.False);
            Assert.That(new RDMUID(0xFFFF, 0xFBCD3F98) > new RDMUID(0xFFFF, 0xABCD3F98), Is.True);
            Assert.That(new RDMUID(0xFFFF, 0xFBCD3F98) < new RDMUID(0xFFFF, 0xABCD3F98), Is.False);
            Assert.That(new RDMUID(0xFFFF, 0xFBCD3F98) >= new RDMUID(0xFFFF, 0xABCD3F98), Is.True);
            Assert.That(new RDMUID(0xFFFF, 0xFBCD3F98) <= new RDMUID(0xFFFF, 0xABCD3F98), Is.False);

            Assert.That(new RDMUID(0x1FFF, 0x4BCD3F98) < new RDMUID(0x2FFF, 0x4BCD3F98), Is.True);
            Assert.That(new RDMUID(0x1FFF, 0x4BCD3F98) > new RDMUID(0x2FFF, 0x4BCD3F98), Is.False);
            Assert.That(new RDMUID(0x1FFF, 0x4BCD3F98) <= new RDMUID(0x2FFF, 0x4BCD3F98), Is.True);
            Assert.That(new RDMUID(0x1FFF, 0x4BCD3F98) >= new RDMUID(0x2FFF, 0x4BCD3F98), Is.False);
            Assert.That(new RDMUID(0x2FFF, 0x4BCD3F98) > new RDMUID(0x1FFF, 0x4BCD3F98), Is.True);
            Assert.That(new RDMUID(0x2FFF, 0x4BCD3F98) < new RDMUID(0x1FFF, 0x4BCD3F98), Is.False);
            Assert.That(new RDMUID(0x2FFF, 0x4BCD3F98) >= new RDMUID(0x1FFF, 0x4BCD3F98), Is.True);
            Assert.That(new RDMUID(0x2FFF, 0x4BCD3F98) <= new RDMUID(0x1FFF, 0x4BCD3F98), Is.False);
        }
        [Test]
        public void RDMUID_Remaining()
        {
            Assert.That(RDMUID.Empty.Manufacturer, Is.EqualTo(EManufacturer.ESTA));
            Assert.That(new RDMUID(EManufacturer.DMXControlProjects_eV, 1234567).Manufacturer, Is.EqualTo(EManufacturer.DMXControlProjects_eV));


            Assert.That(RDMUID.Empty.IsValidDeviceUID, Is.False);
            Assert.That(new RDMUID(EManufacturer.ESTA, 122334).IsValidDeviceUID, Is.False);
            Assert.That(RDMUID.Broadcast.IsValidDeviceUID, Is.False);
            Assert.That(new RDMUID("0123:456789ab").IsValidDeviceUID, Is.True);


            Assert.That((byte[])new RDMUID(EManufacturer.ESTA, 0x12345678), Is.EqualTo(new byte[] { 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }));

            List<RDMUID>list= new List<RDMUID>();
            list.Add(new RDMUID(0x1FFF, 0x00000031));
            list.Add(new RDMUID(0x2FFF, 0x00000022));
            list.Add(new RDMUID(0x3FFF, 0x00000013));
            list.Add(new RDMUID(0x4FFF, 0x00000001));
            list.Add(new RDMUID(0x4FFF, 0x00000002));
            list.Add(new RDMUID(0x4FFF, 0x00000003));
            list.Add(new RDMUID(0x0FFF, 0x00000001));
            list.Add(new RDMUID(0x0FFF, 0x00000002));
            list.Add(new RDMUID(0x0FFF, 0x00000003));

            var orderd=list.OrderBy(uid => uid).ToArray();
            Assert.That(orderd[0], Is.EqualTo(new RDMUID(0x0FFF, 0x00000001)));
            Assert.That(orderd[1], Is.EqualTo(new RDMUID(0x0FFF, 0x00000002)));
            Assert.That(orderd[2], Is.EqualTo(new RDMUID(0x0FFF, 0x00000003)));
            Assert.That(orderd[3], Is.EqualTo(new RDMUID(0x1FFF, 0x00000031)));
            Assert.That(orderd[4], Is.EqualTo(new RDMUID(0x2FFF, 0x00000022)));
            Assert.That(orderd[5], Is.EqualTo(new RDMUID(0x3FFF, 0x00000013)));
            Assert.That(orderd[6], Is.EqualTo(new RDMUID(0x4FFF, 0x00000001)));
            Assert.That(orderd[7], Is.EqualTo(new RDMUID(0x4FFF, 0x00000002)));
            Assert.That(orderd[8], Is.EqualTo(new RDMUID(0x4FFF, 0x00000003)));
        }
    }
}
