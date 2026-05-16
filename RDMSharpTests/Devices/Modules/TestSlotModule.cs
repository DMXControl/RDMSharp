using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestSlotModule
{
    private MockGeneratedDevice1? generated;

    private static UID CONTROLLER_UID = new UID(12538, 619451964);
    private static UID DEVCIE_UID = new UID(0x5ab9, 55229915);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new MockGeneratedDevice1(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test, Order(40)]
    public void TestGetSLOT_INFO()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Slots, Has.Count.EqualTo(5));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.SLOT_INFO,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
        #endregion

        #region Test Change Personality
        Assert.That(generated, Is.Not.Null);
        generated.CurrentPersonalityId = 2; // Change to personality 2
        Assert.That(generated.Slots, Has.Count.EqualTo(8));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
        #endregion

        #region Test Change Personality
        Assert.That(generated, Is.Not.Null);
        generated.CurrentPersonalityId = 3; // Change to personality 3
        Assert.That(generated.Slots, Has.Count.EqualTo(9));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
        #endregion

        #region Test Invalid Calls
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 0); // Change to personality 0
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 4); // Change to personality 4
        Assert.DoesNotThrow(() => generated.CurrentPersonalityId = 3); // Change to personality 3
        #endregion
    }

    [Test, Order(41)]
    public void TestGetDEFAULT_SLOT_VALUE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Slots, Has.Count.EqualTo(5));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DEFAULT_SLOT_VALUE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
        #endregion

        #region Test Change Personality
        Assert.That(generated, Is.Not.Null);
        generated.CurrentPersonalityId = 2; // Change to personality 2
        Assert.That(generated.Slots, Has.Count.EqualTo(8));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
        #endregion

        #region Test Change Personality
        Assert.That(generated, Is.Not.Null);
        generated.CurrentPersonalityId = 3; // Change to personality 3
        Assert.That(generated.Slots, Has.Count.EqualTo(9));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
        Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
        #endregion

        #region Test Invalid Calls
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 0); // Change to personality 0
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 4); // Change to personality 4
        Assert.DoesNotThrow(() => generated.CurrentPersonalityId = 3); // Change to personality 3
        #endregion
    }

    [Test, Order(42)]
    public void TestGetSLOT_DESCRIPTION()
    {
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.SLOT_DESCRIPTION,
            SubDevice = SubDevice.Root,
        };
        RDMMessage? response = null;
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Slots, Has.Count.EqualTo(5));
        doTests(generated.Slots.Values.ToArray());
        #endregion

        #region Test Change Personality 2
        generated.CurrentPersonalityId = 2; // Change to personality 2
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Slots, Has.Count.EqualTo(8));
        doTests(generated.Slots.Values.ToArray());
        #endregion

        #region Test Change Personality 3
        generated.CurrentPersonalityId = 3; // Change to personality 3
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Slots, Has.Count.EqualTo(9));
        doTests(generated.Slots.Values.ToArray());
        #endregion

        #region Test Invalid Calls
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 0); // Change to personality 0
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonalityId = 4); // Change to personality 4
        Assert.DoesNotThrow(() => generated.CurrentPersonalityId = 3); // Change to personality 3

        request.ParameterData = new byte[] { (byte)((10 >> 8) & 0xFF), (byte)(10 & 0xFF) };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        #endregion

        void doTests(Slot[] slots)
        {
            foreach (Slot slot in slots)
            {
                request.ParameterData = new byte[] { (byte)((slot.SlotId >> 8) & 0xFF), (byte)(slot.SlotId & 0xFF) };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(2 + slot.Description.Length));
                Assert.That(response.Value, Is.EqualTo(new RDMSlotDescription(slot.SlotId, slot.Description)));
            }
        }
    }

    [Test, Order(301)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<SlotsModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.Slots, Is.Not.Null);
        Assert.That(generatedModule.Slots, Has.Count.EqualTo(5));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<SlotsModule>().Single();
        var personalityModule = mockDevice.Modules.OfType<DMX_PersonalityModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.Slots, Is.Not.Null);
        Assert.That(module.Slots, Has.Count.EqualTo(5));
        Assert.That(personalityModule.CurrentPersonality.ID, Is.EqualTo(1));
        Assert.That(personalityModule.CurrentPersonality.SlotCount, Is.EqualTo(5));

        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        personalityModule.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await personalityModule.SetPersonality(2);
        await semaphoreSlim.WaitAsync();
        await Task.Delay(1000);

        Assert.That(generatedModule.Slots, Has.Count.EqualTo(8));
        Assert.That(generatedModule.Slots, Is.Not.Null);
        while (module.CurrentPersonality.ID != generatedModule.CurrentPersonality.ID)
            await Task.Delay(100);

        while (!((RemotePersonality)module.CurrentPersonality).AllDataPulled)
            await Task.Delay(100);

        await Task.Delay(100);
        Assert.That(personalityModule.CurrentPersonality.SlotCount, Is.EqualTo(8));

        Assert.That(module.CurrentPersonality.ID, Is.EqualTo(generatedModule.CurrentPersonality.ID));
        Assert.That(module.Slots, Is.Not.Null);
        Assert.That(module.Slots, Has.Count.EqualTo(8));
    }
}