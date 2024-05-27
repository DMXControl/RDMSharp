//using RDMSharp.ParameterWrapper;

//namespace RDMSharpTests.RDM
//{
//    public class RDMUID_Test
//    {
//        [Test]
//        public void RDMUID_ToString()
//        {
//            Assert.Multiple(() =>
//            {
//                Assert.That(new UID(0xFFFF, 0xABCDEF98).ToString(), Is.EqualTo("FFFF:ABCDEF98"));
//                Assert.That(new UID(0xA053, 0x00335612).ToString(), Is.EqualTo("A053:00335612"));
//                Assert.That(new UID(0x0001, 0x12345678).ToString(), Is.EqualTo("0001:12345678"));
//                Assert.That(new UID(1, 2).ToString(), Is.EqualTo("0001:00000002"));
//                Assert.That(UID.Empty.ToString(), Is.EqualTo("0000:00000000"));
//            });
//        }

//        [Test]
//        public void RDMUID_FromString()
//        {
//            var uid = new UID(0x0123456789ab);
//            Assert.Multiple(() =>
//            {
//                Assert.That(new UID("0123:456789ab"), Is.EqualTo(uid));
//                Assert.That(new UID("0123.456789ab"), Is.EqualTo(uid));
//                Assert.That(new UID("0123-456789ab"), Is.EqualTo(uid));
//                Assert.That(new UID("0123456789ab"), Is.EqualTo(uid));
//            });

//            Assert.Multiple(() =>
//            {
//                Assert.Throws(typeof(FormatException), () => new UID("0123+456789ab"));
//                Assert.Throws(typeof(FormatException), () => new UID("0123456789a"));
//                Assert.Throws(typeof(FormatException), () => new UID("0123456789ag"));
//            });
//        }

//        [Test]
//        public void RDMUID_FromULong()
//        {
//            Assert.Multiple(() =>
//            {
//                Assert.That(new UID(0xFFFF, 0xABCDEF98), Is.EqualTo(new UID(0xFFFFABCDEF98)));
//                Assert.That(new UID(0xFF1F, 0xABCD0F98), Is.EqualTo(new UID(0xFF1FABCD0F98)));

//                Assert.That(new UID(EManufacturer.ESTA, 1), Is.EqualTo(new UID(0x0001)));
//            });
//        }

//        [Test]
//        public void RDMUID_CastToULong()
//        {
//            Assert.Multiple(() =>
//            {
//                Assert.That((ulong)new UID(0xFFFF, 0xABCDEF98), Is.EqualTo(0xFFFFABCDEF98u));
//                Assert.That((ulong)new UID(0xFF1F, 0xABCD0F98), Is.EqualTo(0xFF1FABCD0F98u));

//                Assert.That((ulong)new UID(EManufacturer.ESTA, 1), Is.EqualTo(0x0001u));
//            });
//        }

//        [Test]
//        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2043:Use ComparisonConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
//        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
//        public void RDMUID_Equals()
//        {
//            Assert.Multiple(() =>
//            {
//                Assert.That(new UID(), Is.EqualTo(UID.Empty));
//                Assert.That(new UID(EManufacturer.ESTA, 0), Is.EqualTo(UID.Empty));
//                Assert.That(new UID(EManufacturer.ESTA, 0), Is.Not.EqualTo(null));
//                Assert.That(new UID(EManufacturer.ESTA, 0), Is.Not.EqualTo(new UID(0xFFFF, 0xABCDEF98)));
//                Assert.That(new UID(ushort.MaxValue, uint.MaxValue), Is.EqualTo(UID.Broadcast));
//            });

//            var uid = new UID(0xFFFF, 0xABCDEF98);
//            Assert.Multiple(() =>
//            {
//                Assert.That((object)uid, Is.EqualTo((object)new UID(0xFFFF, 0xABCDEF98)));
//                Assert.That(uid, Is.EqualTo((object)new UID(0xFFFF, 0xABCDEF98)));
//                Assert.That(uid, Is.Not.EqualTo((object)new UID(0xFFEE, 0xABCDEF98)));
//                Assert.That((object)uid, Is.Not.EqualTo((object)new UID(0xFFEE, 0xABCDEF98)));
//                Assert.That(object.Equals(uid, new UID(0xFFFF, 0xABCDEF98)), Is.True);
//                Assert.That(object.Equals(uid, new UID(0xFFFF, 0xABCDEF48)), Is.False);
//                Assert.That(object.Equals(uid, null), Is.False);
//                Assert.That(object.Equals(null, uid), Is.False);
//                Assert.That(object.Equals(uid, 0xABCDEF98), Is.False);
//                Assert.That((object)uid, Is.Not.EqualTo(new UID(0xF4FF, 0xABCD3F98)));
//                Assert.That(uid, Is.Not.EqualTo((object)new UID(0xF4FF, 0xABCD3F98)));
//                Assert.That((object)uid, Is.Not.EqualTo(null));
//                Assert.That(uid, Is.Not.EqualTo(null));
//            });

//            Assert.Multiple(() =>
//            {
//                Assert.That(uid == new UID(0xFFFF, 0xABCDEF98), Is.True);
//                Assert.That(uid != new UID(0xFFFF, 0x4BCD3F98), Is.True);
//                Assert.That(new UID(0xFFFF, 0x4BCD3F98) < new UID(0xFFFF, 0xABCD3F98), Is.True);
//                Assert.That(new UID(0xFFFF, 0x4BCD3F98) > new UID(0xFFFF, 0xABCD3F98), Is.False);
//                Assert.That(new UID(0xFFFF, 0x4BCD3F98) <= new UID(0xFFFF, 0xABCD3F98), Is.True);
//                Assert.That(new UID(0xFFFF, 0x4BCD3F98) >= new UID(0xFFFF, 0xABCD3F98), Is.False);
//                Assert.That(new UID(0xFFFF, 0xFBCD3F98) > new UID(0xFFFF, 0xABCD3F98), Is.True);
//                Assert.That(new UID(0xFFFF, 0xFBCD3F98) < new UID(0xFFFF, 0xABCD3F98), Is.False);
//                Assert.That(new UID(0xFFFF, 0xFBCD3F98) >= new UID(0xFFFF, 0xABCD3F98), Is.True);
//                Assert.That(new UID(0xFFFF, 0xFBCD3F98) <= new UID(0xFFFF, 0xABCD3F98), Is.False);
//            });
//            Assert.Multiple(() =>
//            {
//                Assert.That(new UID(0x1FFF, 0x4BCD3F98) < new UID(0x2FFF, 0x4BCD3F98), Is.True);
//                Assert.That(new UID(0x1FFF, 0x4BCD3F98) > new UID(0x2FFF, 0x4BCD3F98), Is.False);
//                Assert.That(new UID(0x1FFF, 0x4BCD3F98) <= new UID(0x2FFF, 0x4BCD3F98), Is.True);
//                Assert.That(new UID(0x1FFF, 0x4BCD3F98) >= new UID(0x2FFF, 0x4BCD3F98), Is.False);
//                Assert.That(new UID(0x2FFF, 0x4BCD3F98) > new UID(0x1FFF, 0x4BCD3F98), Is.True);
//                Assert.That(new UID(0x2FFF, 0x4BCD3F98) < new UID(0x1FFF, 0x4BCD3F98), Is.False);
//                Assert.That(new UID(0x2FFF, 0x4BCD3F98) >= new UID(0x1FFF, 0x4BCD3F98), Is.True);
//                Assert.That(new UID(0x2FFF, 0x4BCD3F98) <= new UID(0x1FFF, 0x4BCD3F98), Is.False);
//            });
//        }
//        [Test]
//        public void RDMUID_Remaining()
//        {
//            Assert.Multiple(() =>
//            {
//                Assert.That(UID.Empty.Manufacturer, Is.EqualTo(EManufacturer.ESTA));
//                Assert.That(new UID(EManufacturer.DMXControlProjects_eV, 1234567).Manufacturer, Is.EqualTo(EManufacturer.DMXControlProjects_eV));


//                Assert.That(UID.Empty.IsValidDeviceUID, Is.False);
//                Assert.That(new UID(EManufacturer.ESTA, 122334).IsValidDeviceUID, Is.False);
//                Assert.That(UID.Broadcast.IsValidDeviceUID, Is.False);
//                Assert.That(new UID("0123:456789ab").IsValidDeviceUID, Is.True);

//            });

//            Assert.That((byte[])new UID(EManufacturer.ESTA, 0x12345678), Is.EqualTo(new byte[] { 0x00, 0x00, 0x12, 0x34, 0x56, 0x78 }));


//            List<UID> list = new List<UID>();
//            list.Add(new UID(0x1FFF, 0x00000031));
//            list.Add(new UID(0x2FFF, 0x00000022));
//            list.Add(new UID(0x3FFF, 0x00000013));
//            list.Add(new UID(0x4FFF, 0x00000001));
//            list.Add(new UID(0x4FFF, 0x00000002));
//            list.Add(new UID(0x4FFF, 0x00000003));
//            list.Add(new UID(0x0FFF, 0x00000001));
//            list.Add(new UID(0x0FFF, 0x00000002));
//            list.Add(new UID(0x0FFF, 0x00000003));

//            var orderd = list.OrderBy(uid => uid).ToArray();

//            Assert.Multiple(() =>
//            {
//                Assert.That(orderd[0], Is.EqualTo(new UID(0x0FFF, 0x00000001)));
//                Assert.That(orderd[1], Is.EqualTo(new UID(0x0FFF, 0x00000002)));
//                Assert.That(orderd[2], Is.EqualTo(new UID(0x0FFF, 0x00000003)));
//                Assert.That(orderd[3], Is.EqualTo(new UID(0x1FFF, 0x00000031)));
//                Assert.That(orderd[4], Is.EqualTo(new UID(0x2FFF, 0x00000022)));
//                Assert.That(orderd[5], Is.EqualTo(new UID(0x3FFF, 0x00000013)));
//                Assert.That(orderd[6], Is.EqualTo(new UID(0x4FFF, 0x00000001)));
//                Assert.That(orderd[7], Is.EqualTo(new UID(0x4FFF, 0x00000002)));
//                Assert.That(orderd[8], Is.EqualTo(new UID(0x4FFF, 0x00000003)));


//                Assert.That(new UID(10, 1) * 2, Is.EqualTo(new UID(20, 2)));
//                Assert.That(new UID(10, 1) * 3, Is.EqualTo(new UID(30, 3)));
//                Assert.That(new UID(10, 1) * 4, Is.EqualTo(new UID(40, 4)));
//            });
//        }
//    }
//}
