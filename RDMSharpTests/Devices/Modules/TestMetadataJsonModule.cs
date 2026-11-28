using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;
using RDMSharpTests.Metadata;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// Invented data only: manufacturer 0x7FF0 is the ESTA prototyping range, the PIDs and names belong to no real product.
/// </summary>
public class TestMetadataJsonModule
{
    private MetadataJsonModuleMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.METADATA_JSON;
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0; // ESTA prototyping range, not a real vendor
    private const ERDM_Parameter FICTIONAL_PARAMETER = (ERDM_Parameter)0x9201;
    private const byte FICTIONAL_LEVEL = 42;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0001);

    private static readonly string FICTIONAL_JSON = TestAddDefineFromJson.MakeJson(PROTOTYPE_MANUFACTURER, (int)FICTIONAL_PARAMETER);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new MetadataJsonModuleMockDevice(DEVCIE_UID, FICTIONAL_JSON);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<MetadataJsonModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Metadata JSON"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetMETADATA_JSON()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.METADATA_JSON), Is.True);

        var module = generated.Modules.OfType<MetadataJsonModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.MetadataJsons, Has.Count.EqualTo(1));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.METADATA_JSON,
            SubDevice = SubDevice.Root,
            ParameterData = PayloadTestTools.GetRequestData(ERDM_Parameter.METADATA_JSON, FICTIONAL_PARAMETER)
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.METADATA_JSON));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMMetadataJson)));
        }
        var metadataJson = (RDMMetadataJson)response.Value;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(metadataJson.ParameterId, Is.EqualTo(FICTIONAL_PARAMETER));
            Assert.That(metadataJson.JSON, Is.EqualTo(FICTIONAL_JSON));
            Assert.That(response.PDL, Is.EqualTo(metadataJson.ToPayloadData().Length));
        }
        #endregion

        #region Test a parameter the device does not describe
        request.ParameterData = PayloadTestTools.GetRequestData(ERDM_Parameter.METADATA_JSON, (ERDM_Parameter)0x9202);
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.METADATA_JSON));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));
        }
        #endregion
    }

    [Test]
    public void TestOnlyManufacturerSpecificParametersAreAllowed()
    {
        // a device can only describe its own parameters, not redefine a standard one
        Assert.Throws<ArgumentOutOfRangeException>(() => new MetadataJsonModule(new RDMMetadataJson(ERDM_Parameter.DEVICE_INFO, FICTIONAL_JSON)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MetadataJsonModule(new RDMMetadataJson((ERDM_Parameter)0xFFE0, FICTIONAL_JSON)));
    }

    [Test]
    public void TestParameterCanBeDescribedOnlyOnce()
    {
        Assert.Throws<ArgumentException>(() => new MetadataJsonModule(
            new RDMMetadataJson(FICTIONAL_PARAMETER, FICTIONAL_JSON),
            new RDMMetadataJson(FICTIONAL_PARAMETER, FICTIONAL_JSON)));
    }

    /// <summary>
    /// The remote device gets the JSON the generated device delivers, registers the define and parses the manufacturer specific parameter with it.
    /// </summary>
    [Test, DependsOnTest(nameof(TestGetMETADATA_JSON))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<MetadataJsonModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<MetadataJsonModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.RegisteredParameters, Does.Contain(FICTIONAL_PARAMETER)));

        var bag = new ParameterBag(FICTIONAL_PARAMETER, DEVCIE_UID.ManufacturerID, mockDevice.DeviceInfo.DeviceModelId, mockDevice.DeviceInfo.SoftwareVersionId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.RegisteredParameters, Does.Contain(FICTIONAL_PARAMETER));
            Assert.That(module.RejectedParameters, Is.Empty);
            Assert.That(MetadataFactory.GetDefine(bag), Is.Not.Null, "the define of the JSON has to be registered in the factory");
        }

        // the define makes the manufacturer specific parameter readable
        await Wait.UntilAsync(() => mockDevice.GetAllParameterValues().ContainsKey(FICTIONAL_PARAMETER));
        Assert.That(mockDevice.GetAllParameterValues(), Does.ContainKey(FICTIONAL_PARAMETER));
        Assert.That(mockDevice.GetAllParameterValues()[FICTIONAL_PARAMETER], Is.EqualTo(FICTIONAL_LEVEL));

        // no module is written for the parameter: a generic one is built from the define (one uint8, read only)
        await Wait.UntilAsync(() => mockDevice.Modules.OfType<RDMSharp.RDM.Device.Module.Generic.IGenericModule>().Any(m => m.Parameter == FICTIONAL_PARAMETER));
        var generic = mockDevice.Modules.OfType<RDMSharp.RDM.Device.Module.Generic.GenericNumericModule<byte>>().Single(m => m.Parameter == FICTIONAL_PARAMETER);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generic.Value, Is.EqualTo(FICTIONAL_LEVEL));
            Assert.That(generic.SupportedCommandClass, Is.EqualTo(ERDM_CommandClass.GET));
            Assert.That(mockDevice.Modules.OfType<RDMSharp.RDM.Device.Module.Generic.IGenericModule>().Select(m => m.Parameter), Has.None.EqualTo(ERDM_Parameter.METADATA_JSON),
                "a parameter with a written module gets no generic one");
        }
    }

    /// <summary>
    /// Devices of the same model share what the model delivered (blueprint parameters), so a second device of the model
    /// gets the JSON defines at once, without asking for them again.
    /// </summary>
    [Test, DependsOnTest(nameof(TestRemoteDevice))]
    public async Task TestRemoteDeviceOfKnownModelGetsTheDefines()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0003);
        using var generatedSecond = new MetadataJsonModuleMockDevice(uid, FICTIONAL_JSON);

        // the model was already pulled by the remote device of TestRemoteDevice
        using MockDevice mockDevice = new MockDevice(uid);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<MetadataJsonModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.RegisteredParameters, Does.Contain(FICTIONAL_PARAMETER)));
        Assert.That(module.RegisteredParameters, Does.Contain(FICTIONAL_PARAMETER));
    }

    /// <summary>
    /// A JSON that belongs to another manufacturer than the device is not registered.
    /// </summary>
    [Test]
    public async Task TestRemoteDeviceRejectsJsonOfAnotherManufacturer()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0002);
        const ERDM_Parameter parameter = (ERDM_Parameter)0x9202;
        // an own device model, so nothing of the other tests is shared
        using var generatedOther = new MetadataJsonOtherModelMockDevice(uid, TestAddDefineFromJson.MakeJson(0x1234, (int)parameter), parameter);

        using MockDevice mockDevice = new MockDevice(uid);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<MetadataJsonModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.RejectedParameters.Keys, Does.Contain(parameter)));

        var bag = new ParameterBag(parameter, uid.ManufacturerID, mockDevice.DeviceInfo.DeviceModelId, mockDevice.DeviceInfo.SoftwareVersionId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.RegisteredParameters, Is.Empty);
            Assert.That(module.RejectedParameters, Does.ContainKey(parameter));
            Assert.That(module.RejectedParameters[parameter], Does.Contain("manufacturer"));
            Assert.That(MetadataFactory.GetDefine(bag), Is.Null, "a rejected JSON is not registered");
        }
    }

    [Test]
    public void TestWithoutDefinesNothingIsDelivered()
    {
        Assert.That(new MetadataJsonModule((RDMMetadataJson[])null!).MetadataJsons, Is.Empty);

        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0010);
        using var device = new MockGeneratedDevice1(uid, new IModule[] { new MetadataJsonModule() });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(device.Parameters, Does.Contain(ERDM_Parameter.METADATA_JSON));
            Assert.That(device.GetAllParameterValues(), Does.Not.ContainKey(ERDM_Parameter.METADATA_JSON), "there is nothing to deliver");
        }
    }

    /// <summary>
    /// A JSON that arrives after the module exists (e.g. the device sends it again after an update) is registered,
    /// the module does not only read what the device model already has.
    /// </summary>
    [Test]
    public async Task TestJsonDeliveredLaterIsRegistered()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0011);
        const ERDM_Parameter parameter = (ERDM_Parameter)0x9211;
        using var generatedPlain = new PlainOtherModelMockDevice(uid);
        using var remote = new MetadataJsonInjectDevice(uid);
        while (!remote.IsInitialized)
            await Task.Delay(100);
        var module = new MetadataJsonModule(remote);
        Assert.That(module.RegisteredParameters, Is.Empty, "the device has no JSON defines");

        remote.InjectMetadataJson(parameter, TestAddDefineFromJson.MakeJson(PROTOTYPE_MANUFACTURER, (int)parameter));
        await Wait.UntilAsync(() => Wait.Holds(module.RegisteredParameters, Does.Contain(parameter)));

        var bag = new ParameterBag(parameter, uid.ManufacturerID, remote.DeviceInfo.DeviceModelId, remote.DeviceInfo.SoftwareVersionId);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.RegisteredParameters, Does.Contain(parameter));
            Assert.That(MetadataFactory.GetDefine(bag), Is.Not.Null);
        }
    }

    /// <summary>
    /// A parameter whose GET needs a request payload (an index) can not be requested by the module, but its define is registered.
    /// </summary>
    [Test]
    public async Task TestParameterWithRequestPayloadIsRegisteredButNotRequested()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x4D4A0012);
        const ERDM_Parameter parameter = (ERDM_Parameter)0x9212;
        using var generatedPlain = new PlainOtherModelMockDevice(uid);
        using var remote = new MetadataJsonInjectDevice(uid);
        while (!remote.IsInitialized)
            await Task.Delay(100);
        var module = new MetadataJsonModule(remote);

        string json = $"{{\"name\":\"FICTIONAL_INDEXED\",\"manufacturer_id\":{PROTOTYPE_MANUFACTURER},\"pid\":{(int)parameter},\"version\":1," +
                      "\"get_request_subdevice_range\":[\"root\"],\"get_request\":[{\"name\":\"index\",\"type\":\"uint8\"}],\"get_response\":[{\"name\":\"level\",\"type\":\"uint8\"}]}";
        remote.InjectMetadataJson(parameter, json);
        await Wait.UntilAsync(() => Wait.Holds(module.RegisteredParameters, Does.Contain(parameter)));
        await Task.Delay(200);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.RegisteredParameters, Does.Contain(parameter));
            Assert.That(module.RejectedParameters, Is.Empty);
            Assert.That(remote.GetAllParameterValues(), Does.Not.ContainKey(parameter), "a GET with a request payload is not requested by the module");
        }
    }

    [Test]
    public void TestModulesExtensionCreatesModuleForMetadataJson()
    {
        Assert.That(RDMSharp.Extensions.ExtensionsManager.Instance.TryGetModulesExtensions(EManufacturer.ESTA, out var extensions), Is.True);
        var modules = new List<Type>();
        foreach (var extension in extensions)
            if (extension.TryGetModules(new[] { ERDM_Parameter.METADATA_JSON }, out var found))
                modules.AddRange(found);
        Assert.That(modules, Does.Contain(typeof(MetadataJsonModule)));
    }

    private static IModule[] metadataJsonModules(string json, ERDM_Parameter parameter) => new IModule[] {
        new MetadataJsonModule(new RDMMetadataJson(parameter, json)),
        new FictionalLevelModule(parameter, FICTIONAL_LEVEL) };

    /// <summary>
    /// A device with a manufacturer specific parameter and the JSON define that describes it.
    /// </summary>
    class MetadataJsonModuleMockDevice : MockGeneratedDevice1
    {
        public MetadataJsonModuleMockDevice(UID uid, string json, ERDM_Parameter parameter = FICTIONAL_PARAMETER) : base(uid, metadataJsonModules(json, parameter))
        {
        }
    }

    /// <summary>
    /// The same, but another device model: it does not share the data of the model with <see cref="MetadataJsonModuleMockDevice"/>.
    /// </summary>
    class MetadataJsonOtherModelMockDevice : MockGeneratedDevice1
    {
        public override ushort DeviceModelID => 0x4D4A;

        public MetadataJsonOtherModelMockDevice(UID uid, string json, ERDM_Parameter parameter) : base(uid, metadataJsonModules(json, parameter))
        {
        }
    }

    /// <summary>
    /// A device of another model without any JSON define, so nothing of the other tests is shared with it.
    /// </summary>
    class PlainOtherModelMockDevice : MockGeneratedDevice1
    {
        public override ushort DeviceModelID => 0x4D4B;

        public PlainOtherModelMockDevice(UID uid) : base(uid)
        {
        }
    }

    /// <summary>
    /// A remote device that gets a METADATA_JSON value like a real response: the payload is parsed with the define, then it is stored in the cache.
    /// </summary>
    internal sealed class MetadataJsonInjectDevice : AbstractMockDevice
    {
        public MetadataJsonInjectDevice(UID uid) : base(uid)
        {
        }

        protected sealed override AbstractMockSubDevice createSubDevice(UID uid, SubDevice subDevice)
        {
            return new MockSubDevice(uid, subDevice);
        }

        protected override void OnDispose()
        {
        }

        public void InjectMetadataJson(ERDM_Parameter parameter, string json)
        {
            var payload = new RDMMetadataJson(parameter, json);
            var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.METADATA_JSON));
            var branch = MetadataFactory.ParseDataToPayload(define, ECommandDublicate.GetResponse, payload.ToPayloadData());
            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.METADATA_JSON, parameter), branch);
        }
    }

    /// <summary>
    /// The manufacturer specific parameter itself: a GET returns one byte (the level). Without the JSON define nobody can parse it.
    /// </summary>
    sealed class FictionalLevelModule : AbstractParameterIDModule
    {
        private readonly byte _level;
        public override string DisplayName => "Fictional Level";

        public FictionalLevelModule(ERDM_Parameter parameter, byte level) : base("FictionalLevel", parameter)
        {
            _level = level;
        }

        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            return SupportedParameters.Contains(parameter) && command == ERDM_Command.GET_COMMAND;
        }

        protected override RDMMessage handleRequest(RDMMessage message)
        {
            return new RDMMessage()
            {
                DestUID = message.SourceUID,
                SourceUID = message.DestUID,
                Parameter = message.Parameter,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                ParameterData = new byte[] { _level }
            };
        }
    }
}
