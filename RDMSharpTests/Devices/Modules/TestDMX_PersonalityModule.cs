using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestDMX_PersonalityModule
{
    private MockGeneratedDevice1? generated;

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
        generated = new MockGeneratedDevice1(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test, Order(10)]
    public void TestGetDMX_PERSONALITY_DESCRIPTION()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var dmxPersonalityModuleModule = generated.Modules.OfType<DMX_PersonalityModule>().Single();
        Assert.That(dmxPersonalityModuleModule, Is.Not.Null);
        Assert.That(dmxPersonalityModuleModule.CurrentPersonality.ID, Is.EqualTo(1));
        Assert.That(dmxPersonalityModuleModule.CurrentPersonality.SlotCount, Is.EqualTo(5));

        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
            SubDevice = SubDevice.Root,
            ParameterData = new byte[] { 0x00 } // Requesting invalid 0 
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        for (byte b = 0; b < generated.Personalities.Count; b++)
        {
            request.ParameterData = new byte[] { (byte)(b + 1) };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            var pers = generated.Personalities.ElementAt(b);
            var expected = new RDMDMXPersonalityDescription(pers.ID, pers.SlotCount, pers.Description);
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            Assert.That(((RDMDMXPersonalityDescription)response.Value).Index, Is.EqualTo(expected.Index));
        }
        #endregion
    }
    [Test, Order(11)]
    public void TestGetDMX_PERSONALITY()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.CurrentPersonalityId, Is.EqualTo(1));

        var dmxPersonalityModule = generated.Modules.OfType<DMX_PersonalityModule>().Single();
        Assert.That(dmxPersonalityModule, Is.Not.Null);
        Assert.That(dmxPersonalityModule.CurrentPersonality.ID, Is.EqualTo(1));
        Assert.That(dmxPersonalityModule.CurrentPersonality.SlotCount, Is.EqualTo(5));

        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DMX_PERSONALITY,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        var pers = generated.Personalities.ElementAt(0);
        var expected = new RDMDMXPersonality(pers.ID, (byte)generated.Personalities.Count());
        Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
        Assert.That(response.Value, Is.EqualTo(expected));
        Assert.That(((RDMDMXPersonality)response.Value).MinIndex, Is.EqualTo(1));
        Assert.That(((RDMDMXPersonality)response.Value).Index, Is.EqualTo(1));
        Assert.That(((RDMDMXPersonality)response.Value).Count, Is.EqualTo(3));
        #endregion

        #region Test Label changed
        Assert.That(generated.CurrentPersonalityId, Is.EqualTo(1));
        generated.CurrentPersonalityId = 2;
        Assert.That(generated.CurrentPersonalityId, Is.EqualTo(2));
        Assert.That(dmxPersonalityModule.CurrentPersonality.SlotCount, Is.EqualTo(8));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        pers = generated.Personalities.ElementAt(1);
        expected = new RDMDMXPersonality(pers.ID, (byte)generated.Personalities.Count());
        Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
        Assert.That(response.Value, Is.EqualTo(expected));
        #endregion
    }

    [Test, Order(301), MaxTime(120000)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DMX_PersonalityModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.CurrentPersonality.ID, Is.EqualTo(1));
        Assert.That(generatedModule.CurrentPersonality.SlotCount, Is.EqualTo(5));

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DMX_PersonalityModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.CurrentPersonality, Is.Not.Null);
        Assert.That(module.CurrentPersonality.ID, Is.EqualTo(1));
        Assert.That(module.CurrentPersonality.SlotCount, Is.EqualTo(5));

        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        await module.SetPersonality(2);
        await semaphoreSlim.WaitAsync();
        await Task.Delay(1000);

        Assert.That(generatedModule.CurrentPersonality.ID, Is.EqualTo(2));
        Assert.That(module.CurrentPersonality.ID, Is.EqualTo(2));
        Assert.That(generatedModule.CurrentPersonality.SlotCount, Is.EqualTo(8));
    }
}