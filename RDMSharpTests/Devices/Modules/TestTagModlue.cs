using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;
using System.Text;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestTagModlue
{
    private TagsMockDevice? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(0x1e2e, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new TagsMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }


    [Test, Retry(3), Order(1)]
    public void TestGetLIST_TAGS()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var tagsModule = generated.Modules.OfType<TagsModule>().FirstOrDefault();
        Assert.That(tagsModule, Is.Not.Null);
        Assert.That(tagsModule.Tags, Is.Not.Null);
        Assert.That(tagsModule.Tags, Has.Count.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_TAGS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        #endregion

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.UTF8.GetBytes("Backtruss")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_TAGS,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(10));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.UTF8.GetBytes("Center")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_TAGS,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(17));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.UTF8.GetBytes("Center")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(true));

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.UTF8.GetBytes("Center")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_TAGS,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(10));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.UTF8.GetBytes("Center")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.EqualTo(false));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CLEAR_TAGS,
            SubDevice = SubDevice.Root
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CLEAR_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LIST_TAGS,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LIST_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
    }
    [Test, Retry(3), Order(2)]
    public void TestSetADD_TAG()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var tagsModule = generated.Modules.OfType<TagsModule>().FirstOrDefault();
        Assert.That(tagsModule, Is.Not.Null);
        Assert.That(tagsModule.Tags, Is.Not.Null);
        Assert.That(tagsModule.Tags, Has.Count.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("Backtruss")
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        #endregion

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("   ")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("asdfghjkljhgfdsdfghjhgfdsdfghjjhgfdsdfghjjhgfdfghjhgfdfghjhgfdfghjhgfdfghjjgfdfghj")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("GeNeRaTeHaRdWaReFaUlT")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));


        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ADD_TAG,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ADD_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
    }
    [Test, Retry(3), Order(3)]
    public void TestSetREMOVE_TAG()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var tagsModule = generated.Modules.OfType<TagsModule>().FirstOrDefault();
        Assert.That(tagsModule, Is.Not.Null);
        Assert.That(tagsModule.Tags, Is.Not.Null);
        Assert.That(tagsModule.Tags, Has.Count.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("Backtruss")
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        #endregion

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("   ")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("asdfghjkljhgfdsdfghjhgfdsdfghjjhgfdsdfghjjhgfdfghjhgfdfghjhgfdfghjhgfdfghjjgfdfghj")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("GeNeRaTeHaRdWaReFaUlT")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));


        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.REMOVE_TAG,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REMOVE_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
    }
    [Test, Retry(3), Order(3)]
    public void TestSetCHECK_TAG()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var tagsModule = generated.Modules.OfType<TagsModule>().FirstOrDefault();
        Assert.That(tagsModule, Is.Not.Null);
        Assert.That(tagsModule.Tags, Is.Not.Null);
        Assert.That(tagsModule.Tags, Has.Count.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("Backtruss")
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(1));
        Assert.That(response.Value, Is.False);
        #endregion

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("   ")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));


        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("asdfghjkljhgfdsdfghjhgfdsdfghjjhgfdsdfghjjhgfdfghjhgfdfghjhgfdfghjhgfdfghjjgfdfghj")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));


        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
            ParameterData = Encoding.ASCII.GetBytes("GeNeRaTeHaRdWaReFaUlT")
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));


        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CHECK_TAG,
            SubDevice = SubDevice.Root,
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CHECK_TAG));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
    }

    [Test, Retry(3), Order(4)]
    public void TestSetCLEAR_TAG()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        var tagsModule = generated.Modules.OfType<TagsModule>().FirstOrDefault();
        Assert.That(tagsModule, Is.Not.Null);
        Assert.That(tagsModule.Tags, Is.Not.Null);
        Assert.That(tagsModule.Tags, Has.Count.EqualTo(0));

        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CLEAR_TAGS,
            SubDevice = SubDevice.Root
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CLEAR_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        #endregion

        request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = new UID(0xeeee, 0xf0f0f0f0),//Hardware Failure trigger
            Parameter = ERDM_Parameter.CLEAR_TAGS,
            SubDevice = SubDevice.Root
        };

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.Not.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CLEAR_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));

        request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.CLEAR_TAGS,
            SubDevice = SubDevice.Root
        };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND_RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CLEAR_TAGS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
    }

    [Test, Retry(3), Order(301)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.LIST_TAGS), Is.True);
        var generatedModule = generated.Modules.OfType<TagsModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.Tags, Is.Not.Null);

        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        bool parameterPresent = mockDevice.DeviceModel.GetSupportedParameters().Any(sp => sp.Parameter == ERDM_Parameter.LIST_TAGS);
        Assert.That(parameterPresent, Is.True);
        var module = mockDevice.Modules.OfType<TagsModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(generatedModule.Tags, Is.Not.Null);

        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 1);
        module.PropertyChanged += (o, e) =>
        {
            semaphoreSlim.Release();
        };
        const string FRONT_TRUSS = "Front-Truss";
        const string MID_TRUSS = "Mid-Truss";
        const string BACK_TRUSS = "Back-Truss";

        await module.AddTag(FRONT_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS));

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await module.AddTag(MID_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS));

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await module.AddTag(BACK_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS).And.Contain(BACK_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS).And.Contain(BACK_TRUSS));


        Assert.That(await module.CheckTag(FRONT_TRUSS), Is.True);
        Assert.That(await module.CheckTag(MID_TRUSS), Is.True);
        Assert.That(await module.CheckTag(BACK_TRUSS), Is.True);

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await module.RemoveTag(MID_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(BACK_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(BACK_TRUSS));

        Assert.That(await module.CheckTag(FRONT_TRUSS), Is.True);
        Assert.That(await module.CheckTag(MID_TRUSS), Is.False);
        Assert.That(await module.CheckTag(BACK_TRUSS), Is.True);

        await module.ClearTags();

        Assert.That(await module.CheckTag(FRONT_TRUSS), Is.False);
        Assert.That(await module.CheckTag(MID_TRUSS), Is.False);
        Assert.That(await module.CheckTag(BACK_TRUSS), Is.False);

        Assert.That(generatedModule.Tags.ToArray(), Does.Not.Contain(FRONT_TRUSS).And.Not.Contain(MID_TRUSS).And.Not.Contain(BACK_TRUSS));



        await generatedModule.AddTag(FRONT_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS));

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await generatedModule.AddTag(MID_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS));

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await generatedModule.AddTag(BACK_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS).And.Contain(BACK_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(MID_TRUSS).And.Contain(BACK_TRUSS));


        Assert.That(await generatedModule.CheckTag(FRONT_TRUSS), Is.True);
        Assert.That(await generatedModule.CheckTag(MID_TRUSS), Is.True);
        Assert.That(await generatedModule.CheckTag(BACK_TRUSS), Is.True);

        semaphoreSlim = new SemaphoreSlim(0, 1);
        await generatedModule.RemoveTag(MID_TRUSS);
        await semaphoreSlim.WaitAsync();

        Assert.That(generatedModule.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(BACK_TRUSS));
        Assert.That(module.Tags.ToArray(), Does.Contain(FRONT_TRUSS).And.Contain(BACK_TRUSS));

        Assert.That(await generatedModule.CheckTag(FRONT_TRUSS), Is.True);
        Assert.That(await generatedModule.CheckTag(MID_TRUSS), Is.False);
        Assert.That(await generatedModule.CheckTag(BACK_TRUSS), Is.True);

        await generatedModule.ClearTags();

        Assert.That(await generatedModule.CheckTag(FRONT_TRUSS), Is.False);
        Assert.That(await generatedModule.CheckTag(MID_TRUSS), Is.False);
        Assert.That(await generatedModule.CheckTag(BACK_TRUSS), Is.False);

        Assert.That(generatedModule.Tags.ToArray(), Does.Not.Contain(FRONT_TRUSS).And.Not.Contain(MID_TRUSS).And.Not.Contain(BACK_TRUSS));
    }

    class TagsMockDevice : MockGeneratedDevice1
    {
        public TagsMockDevice(UID uid) : base(uid, new IModule[] { new TagsModule() })
        {
        }
    }
}