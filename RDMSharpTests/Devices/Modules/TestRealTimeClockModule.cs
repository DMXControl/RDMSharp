using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestRealTimeClockModule
{
    private RealTimeClockModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(123, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new RealTimeClockModuleMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test, Retry(3), Order(1)]
    public async Task TestGetREAL_TIME_CLOCK()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        await Task.Delay(1000);
        var realTimeClockModule = generated.Modules.OfType<RealTimeClockModule>().FirstOrDefault();
        Assert.That(realTimeClockModule, Is.Not.Null);
        Assert.That(realTimeClockModule.RealTimeClock, Is.Not.Null);
        Assert.That(realTimeClockModule.RealTimeClock.Value.Minute, Is.EqualTo(DateTime.Now.Minute));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REAL_TIME_CLOCK,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REAL_TIME_CLOCK));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(7));
        Assert.That(response.Value, Is.TypeOf(typeof(RDMRealTimeClock)));
        var timeGen = new RDMRealTimeClock(realTimeClockModule.RealTimeClock.Value);
        var timeRem = (RDMRealTimeClock)response.Value;
        Assert.That(timeRem.Year, Is.EqualTo(timeGen.Year));
        Assert.That(timeRem.Month, Is.EqualTo(timeGen.Month));
        Assert.That(timeRem.Day, Is.EqualTo(timeGen.Day));
        Assert.That(timeRem.Minute, Is.EqualTo(timeGen.Minute));
        Assert.That(timeRem.Second, Is.AtLeast(timeGen.Second - 2).And.AtMost(timeGen.Second + 2));
        #endregion

    }
    class RealTimeClockModuleMockDevice : MockGeneratedDevice1
    {
        public RealTimeClockModuleMockDevice(UID uid) : base(uid, new IModule[] { new RealTimeClockModule() })
        {
        }
    }
}