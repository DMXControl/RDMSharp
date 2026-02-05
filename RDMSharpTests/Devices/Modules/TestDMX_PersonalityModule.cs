using RDMSharp.Metadata;
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
        Assert.That(generated.CurrentPersonality, Is.EqualTo(1));
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
        Assert.That(generated.CurrentPersonality, Is.EqualTo(1));
        generated.CurrentPersonality = 2;
        Assert.That(generated.CurrentPersonality, Is.EqualTo(2));
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
}