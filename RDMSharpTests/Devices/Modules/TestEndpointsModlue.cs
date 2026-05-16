using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestEndpointsModlue
{
    private EndpointsMockDevice? generated;
    private EndpointsModule? endpointsModule;

    private static UID CONTROLLER_UID = new UID(0x1fff, 876545);
    private static UID DEVCIE_UID = new UID(9031, 45862713);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }


    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new EndpointsMockDevice(DEVCIE_UID);
        endpointsModule = generated.Modules.OfType<EndpointsModule>().FirstOrDefault();
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
        endpointsModule = null;
    }

    [Test, Order(1)]
    public void TestGetENDPOINT_LIST()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_LIST,
            SubDevice = SubDevice.Root,
        };
        RDMMessage requestChangede = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_LIST_CHANGE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(requestChangede);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LIST_CHANGE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(uint)));
        Assert.That(response.Value, Is.EqualTo(0));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LIST));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(40));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointListResponse)));
        var getEndpointListResponse = (GetEndpointListResponse)response.Value;
        Assert.That(getEndpointListResponse.ListChangedNumber, Is.EqualTo(0));
        var endpoints = getEndpointListResponse.Endpoints;
        Assert.That(endpoints, Has.Length.EqualTo(12));

        for (int i = 0; i < endpoints.Length; i++)
        {
            Assert.That(endpoints[i].EndpointId, Is.EqualTo(i + 1));
            Assert.That(endpoints[i].EndpointType, Is.EqualTo(ERDM_EndpointType.VIRTUAL));
        }

        endpointsModule.AddEndpoint(new EndpointOutputMock(13, 2));

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LIST));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(43));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointListResponse)));
        getEndpointListResponse = (GetEndpointListResponse)response.Value;
        Assert.That(getEndpointListResponse.ListChangedNumber, Is.EqualTo(1));
        endpoints = getEndpointListResponse.Endpoints;
        Assert.That(endpoints, Has.Length.EqualTo(13));

        for (int i = 0; i < endpoints.Length; i++)
        {
            Assert.That(endpoints[i].EndpointId, Is.EqualTo(i + 1));
            Assert.That(endpoints[i].EndpointType, Is.EqualTo(ERDM_EndpointType.VIRTUAL));
        }

        endpointsModule.RemoveEndpoint(13);

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LIST));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(40));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointListResponse)));
        getEndpointListResponse = (GetEndpointListResponse)response.Value;
        Assert.That(getEndpointListResponse.ListChangedNumber, Is.EqualTo(2));
        endpoints = getEndpointListResponse.Endpoints;
        Assert.That(endpoints, Has.Length.EqualTo(12));

        for (int i = 0; i < endpoints.Length; i++)
        {
            Assert.That(endpoints[i].EndpointId, Is.EqualTo(i + 1));
            Assert.That(endpoints[i].EndpointType, Is.EqualTo(ERDM_EndpointType.VIRTUAL));
        }

        response = generated.ProcessRequestMessage_Internal(requestChangede);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LIST_CHANGE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(uint)));
        Assert.That(response.Value, Is.EqualTo(2));

        #endregion

    }

    [Test, Order(21)]
    public void TestGetIDENTIFY_ENDPOINT()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IDENTIFY_ENDPOINT,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
        var getSetEndpointLable = (GetSetIdentifyEndpoint)response.Value;
        Assert.That(getSetEndpointLable.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointLable.IdentifyState, Is.EqualTo(false));
        #endregion

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).Identify = true;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetIdentifyEndpoint)));
        getSetEndpointLable = (GetSetIdentifyEndpoint)response.Value;
        Assert.That(getSetEndpointLable.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointLable.IdentifyState, Is.EqualTo(true));
    }
    [Test, Order(22)]
    public void TestGetENDPOINT_TO_UNIVERSE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
        var getSetEndpointToUniverse = (GetSetEndpointToUniverse)response.Value;
        Assert.That(getSetEndpointToUniverse.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointToUniverse.Universe, Is.EqualTo(1));
        #endregion

        request.ParameterData = Tools.ValueToData((ushort)9);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
        getSetEndpointToUniverse = (GetSetEndpointToUniverse)response.Value;
        Assert.That(getSetEndpointToUniverse.EndpointId, Is.EqualTo(9));
        Assert.That(getSetEndpointToUniverse.Universe, Is.EqualTo(2));

        endpointsModule.Endpoints.First(x => x.EndpointId == 9).Universe = 1;

        request.ParameterData = Tools.ValueToData((ushort)9);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointToUniverse)));
        getSetEndpointToUniverse = (GetSetEndpointToUniverse)response.Value;
        Assert.That(getSetEndpointToUniverse.EndpointId, Is.EqualTo(9));
        Assert.That(getSetEndpointToUniverse.Universe, Is.EqualTo(1));

        endpointsModule.Endpoints.First(x => x.EndpointId == 9).Universe = 2;
    }
    [Test, Order(23)]
    public void TestGetENDPOINT_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointMode)));
        var getSetEndpointMode = (GetSetEndpointMode)response.Value;
        Assert.That(getSetEndpointMode.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointMode.EndpointMode, Is.EqualTo(ERDM_EndpointMode.INPUT));
        #endregion

        request.ParameterData = Tools.ValueToData((ushort)9);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointMode)));
        getSetEndpointMode = (GetSetEndpointMode)response.Value;
        Assert.That(getSetEndpointMode.EndpointId, Is.EqualTo(9));
        Assert.That(getSetEndpointMode.EndpointMode, Is.EqualTo(ERDM_EndpointMode.OUTPUT));

        endpointsModule.Endpoints.First(x => x.EndpointId == 9).Mode = ERDM_EndpointMode.DISABLED;

        request.ParameterData = Tools.ValueToData((ushort)9);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointMode)));
        getSetEndpointMode = (GetSetEndpointMode)response.Value;
        Assert.That(getSetEndpointMode.EndpointId, Is.EqualTo(9));
        Assert.That(getSetEndpointMode.EndpointMode, Is.EqualTo(ERDM_EndpointMode.DISABLED));

        endpointsModule.Endpoints.First(x => x.EndpointId == 9).Mode = ERDM_EndpointMode.OUTPUT;
    }
    [Test, Order(24)]
    public void TestGetENDPOINT_LABEL()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_LABEL,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(23));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointLabel)));
        var getSetEndpointLable = (GetSetEndpointLabel)response.Value;
        Assert.That(getSetEndpointLable.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointLable.EndpointLabel, Is.EqualTo("Test Endpoint Input 1"));
        #endregion

        request.ParameterData = Tools.ValueToData((ushort)4);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(24));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointLabel)));
        getSetEndpointLable = (GetSetEndpointLabel)response.Value;
        Assert.That(getSetEndpointLable.EndpointId, Is.EqualTo(4));
        Assert.That(getSetEndpointLable.EndpointLabel, Is.EqualTo("Test Endpoint Output 4"));

        endpointsModule.Endpoints.First(x => x.EndpointId == 4).Lable = "Test Endpoint Output 4 renamed";

        request.ParameterData = Tools.ValueToData((ushort)4);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(32));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointLabel)));
        getSetEndpointLable = (GetSetEndpointLabel)response.Value;
        Assert.That(getSetEndpointLable.EndpointId, Is.EqualTo(4));
        Assert.That(getSetEndpointLable.EndpointLabel, Is.EqualTo("Test Endpoint Output 4 renamed"));
    }
    [Test, Order(25)]
    public void TestGetRDM_TRAFFIC_ENABLE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
        var getSetEndpointRDMTrafficEnable = (GetSetEndpointRDMTrafficEnable)response.Value;
        Assert.That(getSetEndpointRDMTrafficEnable.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointRDMTrafficEnable.RDMTrafficEnabled, Is.EqualTo(true));
        #endregion

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).RDMTraffic = false;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointRDMTrafficEnable)));
        getSetEndpointRDMTrafficEnable = (GetSetEndpointRDMTrafficEnable)response.Value;
        Assert.That(getSetEndpointRDMTrafficEnable.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointRDMTrafficEnable.RDMTrafficEnabled, Is.EqualTo(false));

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).RDMTraffic = true;
    }

    [Test, Order(26)]
    public void TestGetDISCOVERY_STATE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DISCOVERY_STATE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
        var getDiscoveryStateResponse = (GetDiscoveryStateResponse)response.Value;
        Assert.That(getDiscoveryStateResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getDiscoveryStateResponse.DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.INCOMPLETE));
        Assert.That(getDiscoveryStateResponse.DeviceCount, Is.EqualTo(0));
        #endregion

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryState = ERDM_DiscoveryState.FULL;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryStateCount = 4;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(5));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetDiscoveryStateResponse)));
        getDiscoveryStateResponse = (GetDiscoveryStateResponse)response.Value;
        Assert.That(getDiscoveryStateResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getDiscoveryStateResponse.DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.FULL));
        Assert.That(getDiscoveryStateResponse.DeviceCount, Is.EqualTo(4));

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryState = ERDM_DiscoveryState.INCOMPLETE;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryStateCount = 0;
    }
    [Test, Order(27)]
    public void TestGetBACKGROUND_DISCOVERY()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BACKGROUND_DISCOVERY,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointBackgroundDiscovery)));
        var getSetEndpointBackgroundDiscovery = (GetSetEndpointBackgroundDiscovery)response.Value;
        Assert.That(getSetEndpointBackgroundDiscovery.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointBackgroundDiscovery.BackgroundDiscovery, Is.EqualTo(false));
        #endregion

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).BackgroundDiscovery = true;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(3));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetSetEndpointBackgroundDiscovery)));
        getSetEndpointBackgroundDiscovery = (GetSetEndpointBackgroundDiscovery)response.Value;
        Assert.That(getSetEndpointBackgroundDiscovery.EndpointId, Is.EqualTo(1));
        Assert.That(getSetEndpointBackgroundDiscovery.BackgroundDiscovery, Is.EqualTo(true));

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).BackgroundDiscovery = false;
    }
    [Test, Order(28)]
    public void TestGetENDPOINT_TIMING()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_TIMING,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointTimingResponse)));
        var getEndpointTimingResponse = (GetEndpointTimingResponse)response.Value;
        Assert.That(getEndpointTimingResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointTimingResponse.TimingId, Is.EqualTo(2));
        Assert.That(getEndpointTimingResponse.Timings, Is.EqualTo(5));
        #endregion

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).Timing = 1;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(4));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointTimingResponse)));
        getEndpointTimingResponse = (GetEndpointTimingResponse)response.Value;
        Assert.That(getEndpointTimingResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointTimingResponse.TimingId, Is.EqualTo(1));
        Assert.That(getEndpointTimingResponse.Timings, Is.EqualTo(5));

        endpointsModule.Endpoints.First(x => x.EndpointId == 1).Timing = 2;
    }
    [Test, Order(29)]
    public void TestGetENDPOINT_TIMING_DESCRIPTION()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        foreach (var timing in endpointsModule.TimingDescriptions)
        {
            request.ParameterData = Tools.ValueToData(timing.Key);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(14));
            Assert.That(response.Value, Is.Not.Null);
            Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointTimingDescriptionResponse)));
            var etEndpointTimingDescriptionResponse = (GetEndpointTimingDescriptionResponse)response.Value;
            Assert.That(etEndpointTimingDescriptionResponse.TimingId, Is.EqualTo(timing.Key));
            Assert.That(etEndpointTimingDescriptionResponse.Description, Is.EqualTo(timing.Value));
        }
        #endregion
    }
    [Test, Order(30)]
    public void TestGetENDPOINT_RESPONDERS()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_RESPONDERS,
            SubDevice = SubDevice.Root,
        };
        RDMMessage requestChange = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDERS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        requestChange.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(requestChange);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
        var getEndpointResponderListChangeResponse = (GetEndpointResponderListChangeResponse)response.Value;
        Assert.That(getEndpointResponderListChangeResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointResponderListChangeResponse.ListChangeNumber, Is.EqualTo(0));

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDERS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
        var getEndpointRespondersResponse = (GetEndpointRespondersResponse)response.Value;
        Assert.That(getEndpointRespondersResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointRespondersResponse.ListChangedNumber, Is.EqualTo(0));
        Assert.That(getEndpointRespondersResponse.UIDs, Is.EqualTo(new UID[0]));
        #endregion

        ((EndpointMock)endpointsModule.Endpoints.First(endpoint => endpoint.EndpointId == 1)).AddResponder(new UID(0x123456789ABC));

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDERS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(12));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
        getEndpointRespondersResponse = (GetEndpointRespondersResponse)response.Value;
        Assert.That(getEndpointRespondersResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointRespondersResponse.ListChangedNumber, Is.EqualTo(1));
        Assert.That(getEndpointRespondersResponse.UIDs, Is.EqualTo(new UID[] { new UID(0x123456789ABC) }));

        ((EndpointMock)endpointsModule.Endpoints.First(endpoint => endpoint.EndpointId == 1)).RemoveResponder(new UID(0x123456789ABC));

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDERS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointRespondersResponse)));
        getEndpointRespondersResponse = (GetEndpointRespondersResponse)response.Value;
        Assert.That(getEndpointRespondersResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointRespondersResponse.ListChangedNumber, Is.EqualTo(2));
        Assert.That(getEndpointRespondersResponse.UIDs, Is.EqualTo(new UID[0]));

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(requestChange);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(6));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetEndpointResponderListChangeResponse)));
        getEndpointResponderListChangeResponse = (GetEndpointResponderListChangeResponse)response.Value;
        Assert.That(getEndpointResponderListChangeResponse.EndpointId, Is.EqualTo(1));
        Assert.That(getEndpointResponderListChangeResponse.ListChangeNumber, Is.EqualTo(2));
    }
    [Test, Order(31)]
    public void TestGetBINDING_CONTROL_FIELDS()
    {
        UID uid = new UID(0x123456789ABC);
        UID bindingUid = new UID(0xABCDEF123456);
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BINDING_CONTROL_FIELDS,
            SubDevice = SubDevice.Root,
        };

        ((EndpointMock)endpointsModule.Endpoints.First(endpoint => endpoint.EndpointId == 6)).SetBindingControlField(uid, 5, bindingUid);

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BINDING_CONTROL_FIELDS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        request.ParameterData = Tools.ValueToData((ushort)6).Concat(Tools.ValueToData(uid)).ToArray();
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BINDING_CONTROL_FIELDS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(16));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetBindingAndControlFieldsResponse)));
        var getBindingAndControlFieldsResponse = (GetBindingAndControlFieldsResponse)response.Value;
        Assert.That(getBindingAndControlFieldsResponse.EndpointId, Is.EqualTo(6));
        Assert.That(getBindingAndControlFieldsResponse.UID, Is.EqualTo(uid));
        Assert.That(getBindingAndControlFieldsResponse.ControlField, Is.EqualTo(5));
        Assert.That(getBindingAndControlFieldsResponse.BindingUID, Is.EqualTo(bindingUid));
        #endregion

        ((EndpointMock)endpointsModule.Endpoints.First(endpoint => endpoint.EndpointId == 6)).SetBindingControlField(uid, 4, bindingUid);

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BINDING_CONTROL_FIELDS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(16));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetBindingAndControlFieldsResponse)));
        getBindingAndControlFieldsResponse = (GetBindingAndControlFieldsResponse)response.Value;
        Assert.That(getBindingAndControlFieldsResponse.EndpointId, Is.EqualTo(6));
        Assert.That(getBindingAndControlFieldsResponse.UID, Is.EqualTo(uid));
        Assert.That(getBindingAndControlFieldsResponse.ControlField, Is.EqualTo(4));
        Assert.That(getBindingAndControlFieldsResponse.BindingUID, Is.EqualTo(bindingUid));

        ((EndpointMock)endpointsModule.Endpoints.First(endpoint => endpoint.EndpointId == 6)).RemoveBindingControlField(uid);

        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BINDING_CONTROL_FIELDS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(16));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetBindingAndControlFieldsResponse)));
        getBindingAndControlFieldsResponse = (GetBindingAndControlFieldsResponse)response.Value;
        Assert.That(getBindingAndControlFieldsResponse.EndpointId, Is.EqualTo(6));
        Assert.That(getBindingAndControlFieldsResponse.UID, Is.EqualTo(uid));
        Assert.That(getBindingAndControlFieldsResponse.ControlField, Is.EqualTo(0));
        Assert.That(getBindingAndControlFieldsResponse.BindingUID, Is.EqualTo(UID.Empty));


        request.ParameterData = Tools.ValueToData((ushort)60).Concat(Tools.ValueToData(uid)).ToArray();
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BINDING_CONTROL_FIELDS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        Assert.That(response.Value, Is.Null);
    }
    [Test, Order(32)]
    public void TestGetBACKGROUND_QUEUED_STATUS_POLICY()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetBackgroundQueuedStatusPolicyResponse)));
        var getBackgroundQueuedStatusPolicyResponse = (GetBackgroundQueuedStatusPolicyResponse)response.Value;
        Assert.That(getBackgroundQueuedStatusPolicyResponse.PolicyId, Is.EqualTo(1));
        Assert.That(getBackgroundQueuedStatusPolicyResponse.Policies, Is.EqualTo(5));
        #endregion

        endpointsModule.BackgroundQueuedStatusPolicy = 2;

        request.ParameterData = Tools.ValueToData((ushort)1);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(2));
        Assert.That(response.Value, Is.Not.Null);
        Assert.That(response.Value, Is.TypeOf(typeof(GetBackgroundQueuedStatusPolicyResponse)));
        getBackgroundQueuedStatusPolicyResponse = (GetBackgroundQueuedStatusPolicyResponse)response.Value;
        Assert.That(getBackgroundQueuedStatusPolicyResponse.PolicyId, Is.EqualTo(2));
        Assert.That(getBackgroundQueuedStatusPolicyResponse.Policies, Is.EqualTo(5));

        endpointsModule.BackgroundQueuedStatusPolicy = 1;
    }
    [Test, Order(33)]
    public void TestGetBACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        foreach (var timing in endpointsModule.BackgroundQueuedStatusPolicyDescriptions)
        {
            request.ParameterData = Tools.ValueToData(timing.Key);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(14));
            Assert.That(response.Value, Is.Not.Null);
            Assert.That(response.Value, Is.TypeOf(typeof(GetBackgroundQueuedStatusPolicyDescriptionResponse)));
            var getBackgroundQueuedStatusPolicyDescriptionResponse = (GetBackgroundQueuedStatusPolicyDescriptionResponse)response.Value;
            Assert.That(getBackgroundQueuedStatusPolicyDescriptionResponse.PolicyId, Is.EqualTo(timing.Key));
            Assert.That(getBackgroundQueuedStatusPolicyDescriptionResponse.Description, Is.EqualTo(timing.Value));
        }
        #endregion
    }


    [Test, Order(51)]
    public void TestSetIDENTIFY_ENDPOINT()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.IDENTIFY_ENDPOINT,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Identify, Is.EqualTo(false));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.Identify))
                changed = true;
        };

        var payload = new GetSetIdentifyEndpoint(1, true);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Identify, Is.EqualTo(true));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetIdentifyEndpoint(1, false);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Identify, Is.EqualTo(false));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetIdentifyEndpoint(20, false);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetIdentifyEndpoint(1, false);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_ENDPOINT));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(52)]
    public void TestSetENDPOINT_TO_UNIVERSE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Universe, Is.EqualTo(1));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.Universe))
                changed = true;
        };

        var payload = new GetSetEndpointToUniverse(1, 99);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Universe, Is.EqualTo(99));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetEndpointToUniverse(1, 1);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Universe, Is.EqualTo(1));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetEndpointToUniverse(20, 1);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetEndpointToUniverse(1, 1);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TO_UNIVERSE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(53)]
    public void TestSetENDPOINT_MODE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_MODE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Mode, Is.EqualTo(ERDM_EndpointMode.INPUT));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.Mode))
                changed = true;
        };

        var payload = new GetSetEndpointMode(1, ERDM_EndpointMode.DISABLED);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Mode, Is.EqualTo(ERDM_EndpointMode.DISABLED));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetEndpointMode(1, ERDM_EndpointMode.INPUT);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Mode, Is.EqualTo(ERDM_EndpointMode.INPUT));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetEndpointMode(20, ERDM_EndpointMode.INPUT);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetEndpointMode(1, ERDM_EndpointMode.INPUT);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_MODE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(54)]
    public void TestSetENDPOINT_LABEL()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_LABEL,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Lable, Is.EqualTo("Test Endpoint Input 1"));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.Lable))
                changed = true;
        };

        var payload = new GetSetEndpointLabel(1, "Test Endpoint Input 1 renamed");

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Lable, Is.EqualTo("Test Endpoint Input 1 renamed"));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetEndpointLabel(1, "Test Endpoint Input 1");

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Lable, Is.EqualTo("Test Endpoint Input 1"));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetEndpointLabel(20, "Test Endpoint Input 20");

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetEndpointLabel(1, "Test Endpoint Input 1");
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_LABEL));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(55)]
    public void TestSetRDM_TRAFFIC_ENABLE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.RDM_TRAFFIC_ENABLE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).RDMTraffic, Is.EqualTo(true));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.RDMTraffic))
                changed = true;
        };

        var payload = new GetSetEndpointRDMTrafficEnable(1, false);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).RDMTraffic, Is.EqualTo(false));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetEndpointRDMTrafficEnable(1, true);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).RDMTraffic, Is.EqualTo(true));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetEndpointRDMTrafficEnable(20, true);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetEndpointRDMTrafficEnable(1, true);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RDM_TRAFFIC_ENABLE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(56)]
    public void TestSetDISCOVERY_STATE()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DISCOVERY_STATE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.INCOMPLETE));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.DiscoveryState))
                changed = true;
        };

        var payload = new SetDiscoveryStateRequest(1, ERDM_DiscoveryState.FULL);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.FULL));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new SetDiscoveryStateRequest(1, ERDM_DiscoveryState.INCOMPLETE);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).DiscoveryState, Is.EqualTo(ERDM_DiscoveryState.INCOMPLETE));
        Assert.That(changed, Is.EqualTo(true));

        payload = new SetDiscoveryStateRequest(20, ERDM_DiscoveryState.INCOMPLETE);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new SetDiscoveryStateRequest(1, ERDM_DiscoveryState.INCOMPLETE);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DISCOVERY_STATE));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(57)]
    public void TestSetBACKGROUND_DISCOVERY()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BACKGROUND_DISCOVERY,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).BackgroundDiscovery, Is.EqualTo(false));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.BackgroundDiscovery))
                changed = true;
        };

        var payload = new GetSetEndpointBackgroundDiscovery(1, true);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).BackgroundDiscovery, Is.EqualTo(true));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new GetSetEndpointBackgroundDiscovery(1, false);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).BackgroundDiscovery, Is.EqualTo(false));
        Assert.That(changed, Is.EqualTo(true));

        payload = new GetSetEndpointBackgroundDiscovery(20, false);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new GetSetEndpointBackgroundDiscovery(1, false);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_DISCOVERY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(58)]
    public void TestSetENDPOINT_TIMING()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.ENDPOINT_TIMING,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Timing, Is.EqualTo(2));

        bool changed = false;
        endpointsModule.Endpoints.First(x => x.EndpointId == 1).PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Endpoint.Timing))
                changed = true;
        };

        var payload = new SetEndpointTimingRequest(1, 4);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Timing, Is.EqualTo(4));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = new SetEndpointTimingRequest(1, 2);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.Endpoints.First(x => x.EndpointId == 1).Timing, Is.EqualTo(2));
        Assert.That(changed, Is.EqualTo(true));

        payload = new SetEndpointTimingRequest(20, 3);

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        payload = new SetEndpointTimingRequest(1, 2);
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }
    [Test, Order(58)]
    public void TestSetBACKGROUND_QUEUED_STATUS_POLICY()
    {
        #region Test Basic (Empty)
        Assert.That(generated, Is.Not.Null);
        Assert.That(endpointsModule, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Is.Not.Null);
        Assert.That(endpointsModule.Endpoints, Has.Count.Not.EqualTo(0));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.FORMAT_ERROR));
        Assert.That(response.Value, Is.Null);

        Assert.That(endpointsModule.BackgroundQueuedStatusPolicy, Is.EqualTo(1));

        bool changed = false;
        endpointsModule.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(EndpointsModule.BackgroundQueuedStatusPolicy))
                changed = true;
        };

        var payload = (byte)3;

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.BackgroundQueuedStatusPolicy, Is.EqualTo(3));
        Assert.That(changed, Is.EqualTo(true));
        #endregion

        changed = false;
        payload = (byte)1;

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
        Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        Assert.That(response.Value, Is.Null);
        Assert.That(endpointsModule.BackgroundQueuedStatusPolicy, Is.EqualTo(1));
        Assert.That(changed, Is.EqualTo(true));

        payload = (byte)100;

        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));



        payload = (byte)1;
        request.SourceUID = new UID(0xeeee, 0xf0f0f0f0); //Hardware Failure trigger
        request.ParameterData = Tools.ValueToData(payload);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.HARDWARE_FAULT));
    }

    class EndpointsMockDevice : MockGeneratedDevice1
    {
        public EndpointsMockDevice(UID uid) : base(uid, new IModule[] { new EndpointsModule(
            1,
            getPolicys(),
            getTimings(),
            new Endpoint[] {
                new EndpointInputMock(1, 1),
                new EndpointInputMock(2, 2),
                new EndpointOutputMock(3, 1),
                new EndpointOutputMock(4, 1),
                new EndpointOutputMock(5, 1),
                new EndpointOutputMock(6, 1),
                new EndpointOutputMock(7, 1),
                new EndpointOutputMock(8, 2),
                new EndpointOutputMock(9, 2),
                new EndpointOutputMock(10, 2),
                new EndpointOutputMock(11, 2),
                new EndpointOutputMock(12, 2)
            }) })
        {
        }

        private static IDictionary<byte, string> getPolicys()
        {
            var dict = new Dictionary<byte, string>();
            dict.Add(1, "Test Policy 1");
            dict.Add(2, "Test Policy 2");
            dict.Add(3, "Test Policy 3");
            dict.Add(4, "Test Policy 4");
            dict.Add(5, "Test Policy 5");
            return dict;
        }
        private static IDictionary<byte, string> getTimings()
        {
            var dict = new Dictionary<byte, string>();
            dict.Add(1, "Test Timing 1");
            dict.Add(2, "Test Timing 2");
            dict.Add(3, "Test Timing 3");
            dict.Add(4, "Test Timing 4");
            dict.Add(5, "Test Timing 5");
            return dict;
        }
    }
    class EndpointMock : Endpoint
    {
        public EndpointMock(ushort endpointId, ushort universe)
            : base(endpointId, ERDM_EndpointType.VIRTUAL)
        {
            this.Universe = universe;
            base.RDMTraffic = true;
        }

        public new void AddResponder(UID uid)
        {
            base.AddResponder(uid);
        }
        public new void RemoveResponder(UID uid)
        {
            base.RemoveResponder(uid);
        }


        public new void SetBindingControlField(UID uid, ushort controlField, UID bindingUid)
        {
            base.SetBindingControlField(uid, controlField, bindingUid);
        }
        public new void RemoveBindingControlField(UID uid)
        {
            base.RemoveBindingControlField(uid);
        }
    }
    class EndpointInputMock : EndpointMock
    {
        public EndpointInputMock(ushort endpointId, ushort universe)
            : base(endpointId, universe)
        {
            this.Mode = ERDM_EndpointMode.INPUT;
            base.Lable = $"Test Endpoint Input {endpointId}";
            base.Timing = 2;
            base.Identify = false;
            base.DiscoveryState = ERDM_DiscoveryState.INCOMPLETE;
            base.BackgroundDiscovery = false;
        }
    }
    class EndpointOutputMock : EndpointMock
    {
        public EndpointOutputMock(ushort endpointId, ushort universe)
            : base(endpointId, universe)
        {
            this.Mode = ERDM_EndpointMode.OUTPUT;
            base.Lable = $"Test Endpoint Output {endpointId}"; ;
            base.Timing = 3;
        }
    }
}