using System.Collections.Concurrent;

namespace RDMSharpTests
{
    public class TestManyObjects
    {
        private static readonly Random rnd = new Random();
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void TestRDMDiscoveryStatus()
        {
            RDMDiscoveryStatus status1 = new RDMDiscoveryStatus(3, (ulong)(RDMUID.Broadcast - 1), "Test", new RDMUID(0x1234, 0x56789fab), 11);
            RDMDiscoveryStatus status2 = new RDMDiscoveryStatus(6, (ulong)(RDMUID.Broadcast - 1) / 2, "Test2", new RDMUID(0x1234, 0x56789fab), 22);
            Assert.Multiple(() =>
            {
                Assert.That(status1, Is.Not.EqualTo(status2));
                Assert.That((object)status1, Is.Not.EqualTo(status2));
                Assert.That((object)status1, Is.Not.EqualTo((object)status2));
                Assert.That(((object)status1).Equals((object)status2), Is.False);
                Assert.That(((object)status1).Equals(status2.LastFoundUid), Is.False);
                Assert.That(((object)status1).Equals(null), Is.False);
                Assert.That(status1 == status2, Is.False);
                Assert.That(status1 != status2, Is.True);
                Assert.That(status1.GetHashCode(), Is.Not.EqualTo(status2.GetHashCode()));
                var str1 = status1.ToString();
                var str2 = status2.ToString();
                Assert.That(str1, Is.Not.EqualTo(str2));
                Assert.That(str1, Has.Length.AtLeast(10));
                Assert.That(str2, Has.Length.AtLeast(10));
            });
        }
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2043:Use ComparisonConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void TestSubDevice()
        {
            Assert.Multiple(() =>
            {
                List<SubDevice> subdevices = new List<SubDevice>();
                subdevices.Add(SubDevice.Root);
                for (ushort i = 1; i <= 0x0200; i++)
                    subdevices.Add(new SubDevice(i));
                subdevices.Add(SubDevice.Broadcast);

                SubDevice? prev = null;
                HashSet<SubDevice> hashSet = new HashSet<SubDevice>();
                foreach (SubDevice sd in subdevices)
                {
                    if (!prev.HasValue)
                    {
                        prev = sd;
                        hashSet.Add(sd);
                        continue;
                    }

                    Assert.That(prev, Is.Not.EqualTo(sd));
                    Assert.That(prev, Is.Not.EqualTo(null));
                    Assert.That(prev.Equals(sd), Is.False);
                    Assert.That(prev.Equals((object)sd), Is.False);
                    Assert.That(prev.Equals(null), Is.False);
                    Assert.That(prev.Equals((object)sd), Is.False);
                    Assert.That(prev.Equals(null), Is.False);

                    if (!(prev!.Value.IsRoot || prev.Value.IsBroadcast || sd.IsRoot || sd.IsBroadcast))
                    {
                        Assert.That(prev <= sd, Is.True);
                        Assert.That(sd >= prev, Is.True);
                        Assert.That(sd <= prev, Is.False);
                        Assert.That(prev >= sd, Is.False);
                    }
                    else
                    {
                        Assert.That(prev <= sd, Is.False);
                        Assert.That(sd >= prev, Is.False);
                        Assert.That(sd <= prev, Is.False);
                        Assert.That(prev >= sd, Is.False);
                    }

                    Assert.That(prev != sd, Is.True);
                    Assert.That(prev == sd, Is.False);
                    Assert.That(string.IsNullOrWhiteSpace(prev.ToString()), Is.False);
                    Assert.That(string.IsNullOrWhiteSpace(sd.ToString()), Is.False);

                    prev = sd;
                    Assert.That(hashSet.Add(sd), Is.True);
                    Assert.That(hashSet, Does.Contain(prev.Value));
                }
                for (ushort i = 0; i < 1000; i++)
                {
                    ushort id = (ushort)rnd.Next((ushort)0x0201, (ushort)(0xFFFF - 1));
                    Assert.Throws(typeof(ArgumentOutOfRangeException), () => new SubDevice(id));
                }
            });
        }
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void TestIPv4Address()
        {
            var address = IPv4Address.LocalHost;
            Assert.Multiple(() =>
            {
                Assert.That(address.ToString(), Is.EqualTo("127.0.0.1"));
                Assert.That(address, Is.EqualTo(new IPv4Address(address.ToString())));
                Assert.Throws(typeof(FormatException), () => new IPv4Address("1.2.3."));

                address = new IPv4Address(1, 1, 1, 1);

                Assert.That(address, Is.EqualTo(new IPv4Address(new byte[] { 1, 1, 1, 1 })));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new IPv4Address(new byte[] { 1 }));
                Assert.That(address, Is.EqualTo(new IPv4Address(new List<byte>() { 1, 1, 1, 1 })));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new IPv4Address(new List<byte>() { 1 }));


                byte[] bytes = new byte[] { 8, 7, 6, 5 };
                System.Net.IPAddress ip = new System.Net.IPAddress(bytes);
                address = (IPv4Address)bytes;
                Assert.That(ip, Is.EqualTo((System.Net.IPAddress)address));
                Assert.That(address, Is.EqualTo((IPv4Address)ip));

                bytes = new byte[] { 1, 1, 1, 1 };
                Assert.That(bytes, Is.EqualTo((byte[])new IPv4Address(bytes)));

                Assert.That(new IPv4Address(3, 4, 5, 6) == new IPv4Address("3.4.5.6"), Is.True);
                Assert.That(new IPv4Address(3, 4, 5, 6) != new IPv4Address("3.4.5.6"), Is.False);
                Assert.That(((object)new IPv4Address(3, 4, 5, 6)).Equals(new IPv4Address("3.4.5.6")), Is.True);
                Assert.That(((object)new IPv4Address(3, 4, 5, 6)).Equals("3"), Is.False);


                address = new IPv4Address(8, 8, 8, 8);
                Assert.That(address, Is.Not.EqualTo(new IPv4Address(2, 3, 4, 5)));
                Assert.That(address, Is.Not.EqualTo(new IPv4Address(8, 3, 4, 5)));
                Assert.That(address, Is.Not.EqualTo(new IPv4Address(8, 8, 4, 5)));
                Assert.That(address, Is.Not.EqualTo(new IPv4Address(8, 8, 8, 5)));
                Assert.That(address, Is.EqualTo(new IPv4Address(8, 8, 8, 8)));

                ConcurrentDictionary<IPv4Address, string> dict = new ConcurrentDictionary<IPv4Address, string>();

                for (byte i1 = 1; i1 < 246; i1 += 8)
                    for (byte i2 = 1; i2 < 246; i2 += 8)
                        for (byte i3 = 1; i3 < 246; i3 += 8)
                            for (byte i4 = 1; i4 < 246; i4 += 8)
                            {
                                address = new IPv4Address(i1, i2, i3, i4);
                                var res = dict.TryAdd(address, address.ToString());
                                Assert.That(res, Is.True);
                            }

                Assert.Throws(typeof(ArgumentException), () => { var ip = (IPv4Address)System.Net.IPAddress.IPv6Any; });
            });
        }
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void TestMACAddress()
        {
            Assert.Multiple(() =>
            {
                var address = MACAddress.Empty;
                Assert.That(address.ToString(), Is.EqualTo("00:00:00:00:00:00"));
                var bytes = new byte[6] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
                address = new MACAddress(bytes);
                Assert.That((byte[])address, Is.EqualTo(bytes));
                Assert.That((MACAddress)bytes, Is.EqualTo(address));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new MACAddress(new byte[] { 1, 2, 3, 4, 5, 6, 7 }));
                Assert.That(address, Is.EqualTo(new MACAddress(new List<byte>(bytes))));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new MACAddress(new List<byte>() { 1, 2, 3, 4, 5 }));
                Assert.That(address, Is.EqualTo(new MACAddress("11:22:33:44:55:66")));
                Assert.That(address, Is.EqualTo(new MACAddress("11-22-33-44-55-66")));
                Assert.That(address, Is.EqualTo(new MACAddress("1122.3344.5566")));
                Assert.That(address, Is.EqualTo(new MACAddress("112233445566")));
                Assert.Throws(typeof(FormatException), () => new MACAddress("0123456789AG"));

                Assert.That(new MACAddress(0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff) == new MACAddress("AA:BB:CC:DD:EE:FF"), Is.True);
                Assert.That(new MACAddress(0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff) != new MACAddress("AA:BB:CC:DD:EE:FF"), Is.False);
                Assert.That(((object)new MACAddress(0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff)).Equals(new MACAddress("aa:bb:cc:dD:Ee:fF")), Is.True);
                Assert.That(((object)new MACAddress(0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff)).Equals("3"), Is.False);

                address = new MACAddress(8, 8, 8, 8, 8, 8);
                Assert.That(address, Is.Not.EqualTo(new MACAddress(2, 3, 4, 5, 6, 7)));
                Assert.That(address, Is.Not.EqualTo(new MACAddress(8, 3, 4, 5, 6, 7)));
                Assert.That(address, Is.Not.EqualTo(new MACAddress(8, 8, 4, 5, 6, 7)));
                Assert.That(address, Is.Not.EqualTo(new MACAddress(8, 8, 8, 5, 6, 7)));
                Assert.That(address, Is.Not.EqualTo(new MACAddress(8, 8, 8, 8, 6, 7)));
                Assert.That(address, Is.Not.EqualTo(new MACAddress(8, 8, 8, 8, 8, 7)));
                Assert.That(address, Is.EqualTo(new MACAddress(8, 8, 8, 8, 8, 8)));

                ConcurrentDictionary<MACAddress, string> dict = new ConcurrentDictionary<MACAddress, string>();
                for (byte i1 = 1; i1 < 200; i1 += 24)
                    for (byte i2 = 1; i2 < 200; i2 += 24)
                        for (byte i3 = 1; i3 < 200; i3 += 24)
                            for (byte i4 = 1; i4 < 200; i4 += 24)
                                for (byte i5 = 1; i5 < 200; i5 += 24)
                                    for (byte i6 = 1; i6 < 200; i6 += 24)
                                    {
                                        address = new MACAddress(i1, i2, i3, i4, i5, i6);
                                        var res = dict.TryAdd(address, address.ToString());
                                        Assert.That(res, Is.True);
                                    }
            });
        }
    }
}