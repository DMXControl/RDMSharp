using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestPowerStateModule
{
    private PowerStateModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1f8f, 320933);
    private static UID DEVCIE_UID = new UID(8211, 521500598);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new PowerStateModuleMockDevice(DEVCIE_UID);
        while (!generated.IsInitialized)
            await Task.Delay(100);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }
    [Test, Order(10)]
    public async Task TestGetPOWER_STATE()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.POWER_STATE), Is.True);
        await Task.Delay(1000);
        var powerStateModule = generated.Modules.OfType<PowerStateModule>().FirstOrDefault();
        Assert.That(powerStateModule, Is.Not.Null);
        Assert.That(powerStateModule.PowerState, Is.Not.Null);
        Assert.That(powerStateModule.PowerState.Value, Is.EqualTo(ERDM_PowerState.NORMAL));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.POWER_STATE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.POWER_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.TypeOf(typeof(ERDM_PowerState)));
        ERDM_PowerState powerState = (ERDM_PowerState)response.Value;
        Assert.That(powerState, Is.EqualTo(ERDM_PowerState.NORMAL));

        #endregion

    }
    [Test, Retry(3), Order(101)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PowerStateModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.PowerState, Is.EqualTo(ERDM_PowerState.NORMAL));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PowerStateModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.PowerState, Is.EqualTo(ERDM_PowerState.NORMAL));
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await module.SetPowerState(ERDM_PowerState.SHUTDOWN);
        await semaphoreSlim.WaitAsync();
        await Task.Delay(1000);

        Assert.That(generatedModule.PowerState, Is.EqualTo(ERDM_PowerState.NORMAL));
        Assert.That(module.PowerState, Is.EqualTo(ERDM_PowerState.SHUTDOWN));
    }

    class PowerStateModuleMockDevice : MockGeneratedDevice1
    {
        public PowerStateModuleMockDevice(UID uid) : base(uid, new IModule[] { new PowerStateModule(ERDM_PowerState.NORMAL) })
        {
        }
    }
}