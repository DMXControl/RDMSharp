using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestLockModule
{
    private LockModuleMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x18f, 20933);
    private static UID DEVCIE_UID = new UID(211, 51500598);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new LockModuleMockDevice(DEVCIE_UID);
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
    public async Task TestGetLOCK_STATE()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.LOCK_STATE), Is.True);
        await Task.Delay(1000);
        var lockStateModule = generated.Modules.OfType<LockModule>().FirstOrDefault();
        Assert.That(lockStateModule, Is.Not.Null);
        Assert.That(lockStateModule.LockState, Is.Not.Null);
        Assert.That(lockStateModule.LockState.Value, Is.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LOCK_STATE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LOCK_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.TypeOf(typeof(GetLockStateResponse)));
        GetLockStateResponse lockStateResponse = (GetLockStateResponse)response.Value;
        Assert.That(lockStateResponse.CurrentLockStateId, Is.EqualTo(0));
        Assert.That(lockStateResponse.Count, Is.EqualTo(1));

        #endregion

    }
    [Test, Order(11)]
    public async Task TestGetLOCK_PIN()
    {
        await Task.Delay(500);
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.LOCK_PIN), Is.True);
        await Task.Delay(1000);
        var lockStateModule = generated.Modules.OfType<LockModule>().FirstOrDefault();
        Assert.That(lockStateModule, Is.Not.Null);
        Assert.That(lockStateModule.LockPin, Is.Not.Null);
        Assert.That(lockStateModule.LockPin.Value, Is.EqualTo(1234));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LOCK_PIN,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LOCK_PIN));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.TypeOf(typeof(ushort)));
        ushort lockPin = (ushort)response.Value;
        Assert.That(lockPin, Is.EqualTo(1234));

        #endregion

    }
    [Test, Order(101), MaxTime(15000)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<LockModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.LockState, Is.EqualTo(0));
        Assert.That(generatedModule.LockPin, Is.EqualTo(1234));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        await Task.Delay(1000);

        var module = mockDevice.Modules.OfType<LockModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.LockState, Is.EqualTo(0));
        Assert.That(module.LockPin, Is.EqualTo(1234));
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await module.SetLockState(1);
        await semaphoreSlim.WaitAsync(1000);
        await Task.Delay(1000);

        Assert.That(generatedModule.LockState, Is.EqualTo(1));
        Assert.That(module.LockState, Is.EqualTo(1));
        Assert.That(generatedModule.LockPin, Is.EqualTo(1234));
        Assert.That(module.LockPin, Is.EqualTo(1234));

        await module.SetLockPin(4321);
        await semaphoreSlim.WaitAsync(1000);
        await Task.Delay(1000);

        Assert.That(generatedModule.LockState, Is.EqualTo(1));
        Assert.That(module.LockState, Is.EqualTo(1));
        Assert.That(generatedModule.LockPin, Is.EqualTo(4321));
        Assert.That(module.LockPin, Is.EqualTo(4321));

        await module.SetLockState(0);
        await semaphoreSlim.WaitAsync(1000);
        await Task.Delay(1000);

        Assert.That(generatedModule.LockState, Is.EqualTo(0));
        Assert.That(module.LockState, Is.EqualTo(0));
        Assert.That(generatedModule.LockPin, Is.EqualTo(4321));
        Assert.That(module.LockPin, Is.EqualTo(4321));

        await module.SetLockPin(1234);
        await semaphoreSlim.WaitAsync(1000);
        await Task.Delay(1000);

        Assert.That(generatedModule.LockState, Is.EqualTo(0));
        Assert.That(module.LockState, Is.EqualTo(0));
        Assert.That(generatedModule.LockPin, Is.EqualTo(1234));
        Assert.That(module.LockPin, Is.EqualTo(1234));
    }

    class LockModuleMockDevice : MockGeneratedDevice1
    {
        public LockModuleMockDevice(UID uid) : base(uid, new IModule[] { new LockModule(0, 1234, new RDMLockStateDescription(1, "Lock")) })
        {
        }
    }
}