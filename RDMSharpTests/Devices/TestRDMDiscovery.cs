using RDMSharpTests.Devices.Mock;

namespace RDMSharpTest.RDM.Devices
{
    public class TestRDMDiscovery
    {
        private List<MockGeneratedDevice1> mockDevices = new List<MockGeneratedDevice1>();
        [SetUp]
        public void Setup()
        {            
        }
        [TearDown] public void Teardown()
        {
            foreach (var m in mockDevices)
                m.Dispose();

            mockDevices.Clear();
        }
        [Test]
        public async Task TestDiscovery1()
        {
            MockDiscoveryTool mockDiscoveryTool = new MockDiscoveryTool();
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4444)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4445)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4446)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4447)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 4448)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 21314)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 25252)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 636436)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 23252525)));
            foreach (var m in mockDevices)
                m.ImitateRealConditions = true;
            var res = await mockDiscoveryTool.PerformDiscovery(full: true);
            var expectedUIDs = mockDevices.Select(m=>m.UID).ToList();
            Assert.That(res, Is.EquivalentTo(expectedUIDs));
        }
        [Test]
        public async Task TestDiscovery2()
        {
            MockDiscoveryTool mockDiscoveryTool = new MockDiscoveryTool();
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234254)));
            mockDevices.Add(new MockGeneratedDevice1(new RDMUID(0x9fff, 234243)));
            foreach (var m in mockDevices)
                m.ImitateRealConditions = true;
            var res = await mockDiscoveryTool.PerformDiscovery(full: true);
            var expectedUIDs = mockDevices.Select(m => m.UID).ToList();
            Assert.That(res, Is.EquivalentTo(expectedUIDs));
        }
        [Test]
        public async Task TestDiscovery3()
        {
            MockDiscoveryTool mockDiscoveryTool = new MockDiscoveryTool();
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
            foreach (var m in mockDevices)
                m.ImitateRealConditions = true;
            var res = await mockDiscoveryTool.PerformDiscovery(full: true);
            var expectedUIDs = mockDevices.Select(m => m.UID).ToList();
            Assert.That(res, Is.EquivalentTo(expectedUIDs));
        }
    }
}