using RDMSharp.Metadata;
using RDMSharp.RDM;
using System.Collections.Concurrent;
using System.Reflection;

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
            RDMDiscoveryStatus status1 = new RDMDiscoveryStatus(3, (ulong)(UID.Broadcast - 1), "Test", new UID(0x1234, 0x56789fab), 11);
            RDMDiscoveryStatus status2 = new RDMDiscoveryStatus(6, (ulong)(UID.Broadcast - 1) / 2, "Test2", new UID(0x1234, 0x56789fab), 22);
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
        [Test]
        public void TestSlot()
        {
            var slot = new Slot(1);
            Assert.That(slot.SlotId, Is.EqualTo(1));

            MethodInfo updateSlotDefaultValue = slot.GetType().GetMethod("UpdateSlotDefaultValue", BindingFlags.NonPublic | BindingFlags.Instance)!;
            MethodInfo updateSlotInfo = slot.GetType().GetMethod("UpdateSlotInfo", BindingFlags.NonPublic | BindingFlags.Instance)!;
            MethodInfo updateSlotDescription = slot.GetType().GetMethod("UpdateSlotDescription", BindingFlags.NonPublic | BindingFlags.Instance)!;
            Assert.Multiple(() =>
            {
                Assert.Throws(typeof(InvalidOperationException), () => Helper.ThrowInnerException<TargetInvocationException>(() => updateSlotDefaultValue.Invoke(slot, new object[] { new RDMDefaultSlotValue(2) })));
                Assert.Throws(typeof(InvalidOperationException), () => Helper.ThrowInnerException<TargetInvocationException>(() => updateSlotInfo.Invoke(slot, new object[] { new RDMSlotInfo(2) })));
                Assert.Throws(typeof(InvalidOperationException), () => Helper.ThrowInnerException<TargetInvocationException>(() => updateSlotDescription.Invoke(slot, new object[] { new RDMSlotDescription(2) })));
            });
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => { updateSlotDefaultValue.Invoke(slot, new object[] { new RDMDefaultSlotValue(1, 200) }); });
                Assert.DoesNotThrow(() => { updateSlotInfo.Invoke(slot, new object[] { new RDMSlotInfo(1, ERDM_SlotType.SEC_TIMING, ERDM_SlotCategory.CIE_X) }); });
                Assert.DoesNotThrow(() => { updateSlotDescription.Invoke(slot, new object[] { new RDMSlotDescription(1, "rrrr") }); });
                Assert.That(slot.DefaultValue, Is.EqualTo(200));
                Assert.That(slot.Type, Is.EqualTo(ERDM_SlotType.SEC_TIMING));
                Assert.That(slot.Category, Is.EqualTo(ERDM_SlotCategory.CIE_X));
                Assert.That(slot.Description, Is.EqualTo("rrrr"));
            });
            byte fired = 0;
            slot.PropertyChanged += (o, e) => { fired++; };
            for (int i = 0; i < 2; i++)// To Cover Unnessicerry Updates run two times
            {
                Assert.Multiple(() =>
                {
                    Assert.DoesNotThrow(() => { updateSlotDefaultValue.Invoke(slot, new object[] { new RDMDefaultSlotValue(1, 100) }); });
                    Assert.DoesNotThrow(() => { updateSlotInfo.Invoke(slot, new object[] { new RDMSlotInfo(1, ERDM_SlotType.PRIMARY, ERDM_SlotCategory.COLOR_SCROLL) }); });
                    Assert.DoesNotThrow(() => { updateSlotDescription.Invoke(slot, new object[] { new RDMSlotDescription(1, "aaa") }); });
                    Assert.That(fired, Is.EqualTo(4));
                    Assert.That(slot.DefaultValue, Is.EqualTo(100));
                    Assert.That(slot.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                    Assert.That(slot.Category, Is.EqualTo(ERDM_SlotCategory.COLOR_SCROLL));
                    Assert.That(slot.Description, Is.EqualTo("aaa"));
                });
            }
        }
        [Test]
        public void TestGeneratedPersonality()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new GeneratedPersonality(0, "5CH RGB",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer"),
                new Slot(1, ERDM_SlotCategory.STROBE, "Strobe", 33),
                new Slot(2, ERDM_SlotCategory.COLOR_ADD_RED, "Red"),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green"),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue")));
            Assert.Throws(typeof(Exception), () => new GeneratedPersonality(1, "5CH RGB",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer"),
                new Slot(1, ERDM_SlotCategory.STROBE, "Strobe", 33),
                new Slot(2, ERDM_SlotCategory.COLOR_ADD_RED, "Red"),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green"),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue")));
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new GeneratedPersonality(1, "5CH RGB",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer"),
                new Slot(2, ERDM_SlotCategory.STROBE, "Strobe", 33),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_RED, "Red"),
                new Slot(6, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green"),
                new Slot(8, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue")));

            var pers = new GeneratedPersonality(1, "5CH RGB",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer"),
                new Slot(1, ERDM_SlotCategory.STROBE, "Strobe", 33),
                new Slot(2, ERDM_SlotCategory.COLOR_ADD_RED, "Red"),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green"),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue"));

            Assert.Multiple(() =>
            {
                Assert.That(pers.ToString(), Contains.Substring(pers.ID.ToString()));
                Assert.That(pers.ToString(), Contains.Substring(pers.SlotCount.ToString()));
                Assert.That(pers.ToString(), Contains.Substring(pers.Description));
                Assert.That(pers.SlotCount, Is.EqualTo(5));
                Assert.That(pers.Description, Is.EqualTo("5CH RGB"));
                Assert.That(pers.ID, Is.EqualTo(1));
            });
        }
        [Test]
        public void TestPDL()
        {
            Assert.Multiple(() =>
            {
                PDL pdl = new PDL();
                Assert.That(pdl.Value.HasValue, Is.True);
                Assert.That(pdl.MinLength.HasValue, Is.False);
                Assert.That(pdl.MaxLength.HasValue, Is.False);
                Assert.That(pdl.Value, Is.Not.Null);
                Assert.That(pdl.Value!.Value, Is.EqualTo(0));

                pdl = new PDL(13);
                Assert.That(pdl.Value.HasValue, Is.True);
                Assert.That(pdl.MinLength.HasValue, Is.False);
                Assert.That(pdl.MaxLength.HasValue, Is.False);
                Assert.That(pdl.Value, Is.Not.Null);
                Assert.That(pdl.Value!.Value, Is.EqualTo(13));


                pdl = new PDL(3, 5);
                Assert.That(pdl.Value.HasValue, Is.False);
                Assert.That(pdl.MinLength.HasValue, Is.True);
                Assert.That(pdl.MaxLength.HasValue, Is.True);
                Assert.That(pdl.MinLength!.Value, Is.EqualTo(3));
                Assert.That(pdl.MaxLength!.Value, Is.EqualTo(5));

                pdl = new PDL(5, 5);
                Assert.That(pdl.Value.HasValue, Is.True);
                Assert.That(pdl.MinLength.HasValue, Is.False);
                Assert.That(pdl.MaxLength.HasValue, Is.False);
                Assert.That(pdl.Value, Is.Not.Null);
                Assert.That(pdl.Value!.Value, Is.EqualTo(5));


                List<PDL> list = new List<PDL>();
                list.Add(new PDL(1));
                list.Add(new PDL(2));
                list.Add(new PDL(3));
                pdl = new PDL(list.ToArray());
                Assert.That(pdl.Value.HasValue, Is.True);
                Assert.That(pdl.MinLength.HasValue, Is.False);
                Assert.That(pdl.MaxLength.HasValue, Is.False);
                Assert.That(pdl.Value, Is.Not.Null);
                Assert.That(pdl.Value!.Value, Is.EqualTo(6));

                list.Clear();
                list.Add(new PDL(1, 2));
                list.Add(new PDL(1, 2));
                list.Add(new PDL(3, 4));
                pdl = new PDL(list.ToArray());
                Assert.That(pdl.Value.HasValue, Is.False);
                Assert.That(pdl.MinLength.HasValue, Is.True);
                Assert.That(pdl.MaxLength.HasValue, Is.True);
                Assert.That(pdl.MinLength!.Value, Is.EqualTo(5));
                Assert.That(pdl.MaxLength!.Value, Is.EqualTo(8));


                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new PDL(uint.MaxValue));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new PDL(uint.MaxValue, 9));
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => new PDL(1, uint.MaxValue));
            });
        }

        [Test]
        public void TestParameterUpdatedBag()
        {
            Assert.Multiple(() =>
            {
                ParameterUpdatedBag bag = new ParameterUpdatedBag(ERDM_Parameter.SENSOR_DEFINITION, 1);
                Assert.That(bag.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
                Assert.That(bag.Index, Is.EqualTo(1));
                var secounds = DateTime.UtcNow.TimeOfDay.TotalSeconds;
                var utc = DateTime.UtcNow;
                Assert.That(bag.Timestamp.TimeOfDay.TotalSeconds, Is.InRange(secounds - 2, secounds + 2));
                Assert.That(bag.ToString(), Contains.Substring("SENSOR_DEFINITION"));
                Assert.That(bag.ToString(), Contains.Substring("(1)"));
                Assert.That(bag.ToString(), Contains.Substring(utc.Year.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Month.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Day.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Hour.ToString()));

                bag = new ParameterUpdatedBag(ERDM_Parameter.ADD_TAG);
                Assert.That(bag.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
                Assert.That(bag.Index, Is.Null);
                Assert.That(bag.ToString(), Contains.Substring("ADD_TAG"));
                Assert.That(bag.ToString(), Contains.Substring(utc.Year.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Month.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Day.ToString()));
                Assert.That(bag.ToString(), Contains.Substring(utc.Hour.ToString()));

            });
        }
        [Test]
        public void TestParameterDataCacheBag()
        {
            Assert.Multiple(() =>
            {
                ParameterDataCacheBag bag = new ParameterDataCacheBag(ERDM_Parameter.SENSOR_DEFINITION, 1);
                Assert.That(bag.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
                Assert.That(bag.Index, Is.EqualTo(1));
                Assert.That(bag.ToString(), Contains.Substring("SENSOR_DEFINITION"));
                Assert.That(bag.ToString(), Contains.Substring("(1)"));

                bag = new ParameterDataCacheBag(ERDM_Parameter.ADD_TAG);
                Assert.That(bag.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
                Assert.That(bag.Index, Is.Null);
                Assert.That(bag.ToString(), Is.EqualTo("ADD_TAG"));
            });
        }
        [Test]
        public void TestPeerToPeerProcess()
        {

            ParameterBag bag = new ParameterBag(ERDM_Parameter.DMX_START_ADDRESS, 1234, 333, 4);
            PeerToPeerProcess ptp = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, new UID(1234, 5678), SubDevice.Root, bag);
            Assert.Multiple(() =>
            {
                Assert.That(ptp.Command, Is.EqualTo(ERDM_Command.GET_COMMAND));
                Assert.That(ptp.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(ptp.RequestPayloadObject, Is.EqualTo(DataTreeBranch.Unset));
            });
            Assert.Multiple(() =>
            {
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.DISCOVERY_COMMAND, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.DISCOVERY_COMMAND_RESPONSE, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.GET_COMMAND_RESPONSE, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.SET_COMMAND_RESPONSE, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.NONE, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
                Assert.Throws(typeof(ArgumentException), () => new PeerToPeerProcess(ERDM_Command.RESPONSE, new UID(1234, 5678), SubDevice.Root, bag, DataTreeBranch.Unset));
            });
        }

        [Test]
        public void TestSensor()
        {
            ConcurrentDictionary<string,object> dict = new ConcurrentDictionary<string, object>();
            Sensor sensor = new Sensor(2);
            Assert.DoesNotThrow(() => { sensor.PropertyChanged += Sensor_PropertyChanged; });
            sensor.UpdateDescription(new RDMSensorDefinition(2, 1, 2, 3, 4, 5, 6, 7, true, true, "test"));
            sensor.UpdateValue(new RDMSensorValue(2, 44, 33, 55, 40));
            Assert.Multiple(() =>
            {
                Assert.That(sensor, Is.Not.Null);
                Assert.That(Sensor.Equals(sensor, null), Is.False);
                Assert.That(Object.Equals(sensor, null), Is.False);
                Assert.That(sensor!.Equals(null), Is.False);
                Assert.That(sensor!.SensorId, Is.EqualTo(2));
                Assert.That(sensor.Description, Is.Not.Null);
                Assert.That(sensor.Description, Is.EqualTo("test"));
                Assert.That(sensor.PresentValue, Is.EqualTo(44));
                Assert.That(sensor.LowestValue, Is.EqualTo(33));
                Assert.That(sensor.HighestValue, Is.EqualTo(55));
                Assert.That(sensor.RecordedValue, Is.EqualTo(40));
                Assert.That(sensor.Type, Is.EqualTo(ERDM_SensorType.VOLTAGE));
                Assert.That(sensor.Unit, Is.EqualTo(ERDM_SensorUnit.VOLTS_DC));
                Assert.That(sensor.Prefix, Is.EqualTo(ERDM_UnitPrefix.MILLI));
                Assert.That(sensor.RangeMaximum, Is.EqualTo(5));
                Assert.That(sensor.RangeMinimum, Is.EqualTo(4));
                Assert.That(sensor.NormalMaximum, Is.EqualTo(7));
                Assert.That(sensor.NormalMinimum, Is.EqualTo(6));
                Assert.That(sensor.LowestHighestValueSupported, Is.True);
                Assert.That(sensor.RecordedValueSupported, Is.True);
                Assert.That(sensor.ToString(), Contains.Substring("Sensor: 2"));
                Assert.That(sensor.ToString(), Contains.Substring("PresentValue: 44"));
                Assert.That(sensor.ToString(), Contains.Substring("LowestValue: 33"));
                Assert.That(sensor.ToString(), Contains.Substring("HighestValue: 55"));
                Assert.That(sensor.ToString(), Contains.Substring("RecordedValue: 40"));
                Assert.That(sensor.ToString(), Contains.Substring("Description: test"));
                Assert.DoesNotThrow(() => { sensor.PropertyChanged -= Sensor_PropertyChanged; });
            });
            Assert.Throws(typeof(InvalidOperationException), () => sensor.UpdateDescription(new RDMSensorDefinition(5, 1, 2, 3, 4, 5, 6, 7, true, true, "test")));
            Assert.Throws(typeof(InvalidOperationException), () => sensor.UpdateValue(new RDMSensorValue(9, 44, 33, 55, 40)));
            Assert.Throws(typeof(ArgumentNullException), () => new Sensor(1, ERDM_SensorType.TIME, ERDM_SensorUnit.NEWTON, ERDM_UnitPrefix.PETA, null, 1, 2, 3, 4, true, false));

            //Test again to cover the Update methods and CodeCoverage
            sensor.UpdateDescription(new RDMSensorDefinition(2, 1, 2, 3, 4, 5, 6, 7, true, true, "test"));
            sensor.UpdateValue(new RDMSensorValue(2, 44, 33, 55, 40));

            sensor.UpdateDescription(new RDMSensorDefinition(2, 1, 2, 3, 4, 5, 6, 7, false, false, "test"));
            sensor.UpdateValue(new RDMSensorValue(2, 44, 33, 55, 40));

            Assert.That(sensor.LowestHighestValueSupported, Is.False);
            Assert.That(sensor.RecordedValueSupported, Is.False);
            Assert.That(dict, Has.Count.EqualTo(14));
            Assert.That(sensor.GetHashCode(), Is.Not.EqualTo(0));

            void Sensor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                dict.AddOrUpdate(e!.PropertyName!, 1, (key, oldValue) => null!);
            }
        }
    }
}