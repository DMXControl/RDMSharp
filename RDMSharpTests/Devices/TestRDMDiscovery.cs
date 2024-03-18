using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMDiscovery
    {
        private readonly List<MockGeneratedDevice1> mockDevices = new List<MockGeneratedDevice1>();
        private MockDiscoveryTool? mockDiscoveryTool;
        private List<RDMUID>? expected;
        [SetUp]
        public void Setup()
        {
            mockDiscoveryTool = new MockDiscoveryTool();
        }
        [TearDown] public void Teardown()
        {
            foreach (var m in mockDevices)
                m.Dispose();

            mockDevices.Clear();
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
                if (status == value)
                    return;

                status = value;
                ProgressChanged?.Invoke(this,value);
            }
        }

        private async Task AssertDiscovery(bool full = true)
        {
            if (mockDiscoveryTool == null)
                Assert.Fail($"{nameof(mockDiscoveryTool)} is null");
            if (expected == null)
                Assert.Fail($"{nameof(expected)} is null");

            foreach (var m in mockDevices)
                m.ImitateRealConditions = true;
            var progress = new DiscoveryProgress();
            ulong messageCount = 0;
            progress.ProgressChanged += (o,e) => 
            {
                if (e.MessageCount <= messageCount)
                    return;
                messageCount = e.MessageCount;
                Console.WriteLine(e.ToString());
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

        [Test]
        public async Task TestDiscovery1()
        {
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4444)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4445)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4446)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4447)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4448)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 21314)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 25252)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 636436)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 23252525)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test]
        public async Task TestDiscovery2()
        {
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234254)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234243)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();

            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 0x123400)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 0x567800)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 0x9abc00)));

            expected = mockDevices.Select(m => m.UID).Except(expected).ToList();
            await AssertDiscovery(false); // Discover only new Devices

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test]
        public async Task TestDiscovery3()
        {
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234254)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234243)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 124367)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 64687)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 487755)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 9696)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 7474574)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4784757)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 747747)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 7800)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 90789)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 225677856)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 989089)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4757342)));

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
        [Test]
        public async Task TestDiscovery4()
        {
            Random random = new Random();
            HashSet<uint> ids=new HashSet<uint>();
            for (int i = 0; i < 150; i++)
            {
                uint id = 0;
                do
                {
                    id = (uint)random.Next();
                }
                while (!ids.Add(id));
                var m = new MockGeneratedDevice1(new RDMUID(0x9fff, id));
                mockDevices.Add(m);
            }

            expected = mockDevices.Select(m => m.UID).ToList();
            await AssertDiscovery();
        }
    }
}