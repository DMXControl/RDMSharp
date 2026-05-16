using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestRealTimeClockModule
{
    private RealTimeClockModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(876, 555198);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new RealTimeClockModuleMockDevice(DEVCIE_UID);
        while (!generated.IsInitialized)
            await Task.Delay(100);
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
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.REAL_TIME_CLOCK), Is.True);
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

    [Test, Retry(3), Order(301)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.REAL_TIME_CLOCK), Is.True);
        var generatedModule = generated.Modules.OfType<RealTimeClockModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.RealTimeClock, Is.Not.Null);
        Assert.That(generatedModule.RealTimeClock.Value.Minute, Is.EqualTo(DateTime.Now.Minute));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        bool parameterPresent = mockDevice.DeviceModel.GetSupportedParameters().Any(sp => sp.Parameter == ERDM_Parameter.REAL_TIME_CLOCK);
        Assert.That(parameterPresent, Is.True);
        var module = mockDevice.Modules.OfType<RealTimeClockModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.RealTimeClock, Is.Not.Null);
        Assert.That(module.RealTimeClock.Value.Minute, Is.EqualTo(DateTime.Now.Minute));

        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        DateTime newDate = new DateTime(2026, 12, 30, 23, 29, 45);
        module.RealTimeClock = newDate;
        await semaphoreSlim.WaitAsync();
    }

    class RealTimeClockModuleMockDevice : MockGeneratedDevice1
    {
        public RealTimeClockModuleMockDevice(UID uid) : base(uid, new IModule[] { new RealTimeClockModule() })
        {
        }
    }
}