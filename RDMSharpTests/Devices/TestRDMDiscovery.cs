using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMDiscovery
    {
        private static readonly Random random = new Random();
        private readonly List<MockGeneratedDevice1> mockDevices = new List<MockGeneratedDevice1>();
        private MockDiscoveryTool? mockDiscoveryTool;
        private List<UID>? expected;
        [SetUp]
        public void Setup()
        {
            GlobalTestSetup.ImitateRealConditions = true;
            mockDiscoveryTool = new MockDiscoveryTool();
        }
        [TearDown]
        public void Teardown()
        {
            GlobalTestSetup.ImitateRealConditions = false;
            foreach (IDisposable m in mockDevices)
                m.Dispose();

            mockDevices.Clear();
            mockDiscoveryTool?.Dispose();
            mockDiscoveryTool = null;
            expected?.Clear();
            expected = null;
        }

        private class DiscoveryProgress : IProgress<RDMDiscoveryStatus>
        {
            public event EventHandler<RDMDiscoveryStatus>? ProgressChanged;
            public RDMDiscoveryStatus status { get; private set; } = new RDMDiscoveryStatus();
            public void Report(RDMDiscoveryStatus value)
            {
                status = value;
                ProgressChanged?.Invoke(this, value);
            }
        }
        [Test, Retry(3), CancelAfter(120000)]
        public void TestDiscoveryProgress()
        {
            var progress = new DiscoveryProgress();
            progress.Report(new RDMDiscoveryStatus());
            bool wasCalled= false;
            progress.ProgressChanged+= (o,e)=>{ wasCalled = true; };
            Assert.That(wasCalled, Is.False);
            progress.Report(new RDMDiscoveryStatus());
            Assert.That(wasCalled, Is.True);
        }

        private async Task AssertDiscovery(bool full = true)
        {
            ArgumentNullException.ThrowIfNull(mockDiscoveryTool);
            ArgumentNullException.ThrowIfNull(expected);

            var progress = new DiscoveryProgress();
            ulong messageCount = 0;
            progress.ProgressChanged += (o, e) =>
            {
                if (e.MessageCount <= messageCount)
                    return;
                messageCount = e.MessageCount;
                //Console.WriteLine(e.ToString());
            };
            Assert.That(progress.status.RangeDoneInPercent, Is.EqualTo(0));
            var res = await mockDiscoveryTool!.PerformDiscovery(progress, full);
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.EquivalentTo(expected!));
                Assert.That(progress.status.MessageCount, Is.AtLeast(expected!.Count * 3));
                Assert.That(progress.status.FoundDevices, Is.EqualTo(expected!.Count));
                Assert.That(progress.status.RangeLeftToSearch, Is.EqualTo(0));
                Assert.That(progress.status.RangeDoneInPercent, Is.EqualTo(1));
            });
        }

        [Test, Retry(3), CancelAfter(30000)]
        public async Task TestDiscoveryMute()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4444)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4445)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4446)));
            RDMMessage muteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = UID.Broadcast,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_MUTE,
            };

            var response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            RDMMessage unmuteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = UID.Broadcast,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
            };
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(unmuteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.False);
            muteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = UID.Broadcast,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_MUTE,
            };
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            unmuteBroadcast.DestUID = new UID(0x9fff, 4444);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(unmuteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            unmuteBroadcast.DestUID = new UID(0x9fff, 4445);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(unmuteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            muteBroadcast.DestUID = new UID(0x9fff, 4444);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.False);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            muteBroadcast.DestUID = new UID(0x9fff, 4445);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[1].DiscoveryMuted, Is.True);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);
        }
        [Test, Retry(3), CancelAfter(30000)]
        public async Task TestDiscovery_UNIQUE_BRANCH()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b0000000100000001, 0b00000001)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b1000000101000001, 0b00000010)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b0000000101000001, 0b00000100)));

            RDMMessage unmuteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = UID.Broadcast,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
            };
            var response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(unmuteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Null);

            RDMMessage branchBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = UID.Broadcast,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                ParameterData = new DiscUniqueBranchRequest(UID.Empty, UID.Broadcast).ToPayloadData()
            };

            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(branchBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.ChecksumValid, Is.False);

            branchBroadcast.ParameterData = new DiscUniqueBranchRequest(UID.Empty, UID.Broadcast / 2).ToPayloadData();

            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(branchBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.ChecksumValid, Is.False);

            branchBroadcast.ParameterData = new DiscUniqueBranchRequest(UID.Empty, new UID(0x0111,0)).ToPayloadData();

            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(branchBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.ChecksumValid, Is.True);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[0].UID));

            RDMMessage muteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = response.Response.SourceUID,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_MUTE,
            };
            Assert.That(mockDevices[0].DiscoveryMuted, Is.False);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[0].UID));
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);

            branchBroadcast.ParameterData = new DiscUniqueBranchRequest(UID.Empty, UID.Broadcast / 2).ToPayloadData();

            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(branchBroadcast);
            Assert.That(mockDevices[0].DiscoveryMuted, Is.True);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.ChecksumValid, Is.True);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[2].UID));

            muteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = response.Response.SourceUID,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_MUTE,
            };
            Assert.That(mockDevices[2].DiscoveryMuted, Is.False);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[2].UID));
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);

            branchBroadcast.ParameterData = new DiscUniqueBranchRequest(UID.Broadcast/2, UID.Broadcast).ToPayloadData();

            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(branchBroadcast);
            Assert.That(mockDevices[2].DiscoveryMuted, Is.True);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.ChecksumValid, Is.True);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[1].UID));

            muteBroadcast = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                DestUID = response.Response.SourceUID,
                SourceUID = RDMSharp.RDMSharp.Instance.ControllerUID,
                SubDevice = SubDevice.Root,
                Parameter = ERDM_Parameter.DISC_MUTE,
            };
            Assert.That(mockDevices[1].DiscoveryMuted, Is.False);
            response = await RDMSharp.RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(muteBroadcast);
            Assert.That(response.Success, Is.True);
            Assert.That(response.Response, Is.Not.Null);
            Assert.That(response.Response.SourceUID, Is.EqualTo(mockDevices[1].UID));
            Assert.That(mockDevices[1].DiscoveryMuted, Is.True);

        }

        [Test, Retry(3), CancelAfter(30000)]
        public async Task TestDiscovery0()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b0000000100000001, 0b00000001)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b1000000101000001, 0b00000010)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0b0000000101000001, 0b00000100)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }

        [Test, Retry(3), CancelAfter(30000)]
        public async Task TestDiscovery1()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4444)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4445)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4446)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4447)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4448)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 21314)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 25252)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 636436)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 23252525)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test, Retry(3), CancelAfter(60000)]
        public async Task TestDiscovery2()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 234254)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 234243)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();

            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 0x123400)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 0x567800)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 0x9abc00)));

            expected = mockDevices.Select(m => m.UID).Except(expected).ToList();
            await AssertDiscovery(false); // Discover only new Devices

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test, Retry(3), CancelAfter(60000)]
        public async Task TestDiscovery3()
        {
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 234254)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 234243)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 124367)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 64687)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 487755)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 9696)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 7474574)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4784757)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 747747)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 7800)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 90789)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 225677856)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 989089)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4)));
            mockDevices.Add(new MockGeneratedDevice1(new UID(0x9fff, 4757342)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test, Retry(3), CancelAfter(100000)]
        public async Task TestDiscovery4()
        {
            HashSet<uint> ids = new HashSet<uint>();
            for (int i = 0; i < 50; i++)
            {
                uint id = 0;
                do
                {
                    id = (uint)random.Next();
                }
                while (!ids.Add(id));
                var m = new MockGeneratedDevice1(new UID(0x9fff, id));
                mockDevices.Add(m);
            }

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test, Retry(3), CancelAfter(100000)]
        public async Task TestDiscovery5TotalyRandom()
        {
            HashSet<uint> ids = new HashSet<uint>();
            HashSet<ushort> idsMan = new HashSet<ushort>();
            for (int i = 0; i < 30; i++)
            {
                uint id = 0;
                ushort idMan = 0;
                do
                {
                    id = (uint)random.Next();
                }
                while (!ids.Add(id));
                do
                {
                    idMan = (ushort)random.Next(ushort.MinValue + 100, ushort.MaxValue - 100);
                }
                while (!idsMan.Add(idMan));
                var m = new MockGeneratedDevice1(new UID(idMan, id));
                mockDevices.Add(m);
            }

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
    }
}