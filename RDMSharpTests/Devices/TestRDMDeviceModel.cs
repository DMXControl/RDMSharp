using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;
using System.Collections.Concurrent;

namespace RDMSharpTests.Devices;

/// <summary>
/// The <see cref="RDMDeviceModel"/>: what devices of one model share (supported parameters, descriptions of manufacturer specific parameters).
/// Invented data only: manufacturer 0x7FF0 is the ESTA prototyping range, the PIDs and names belong to no real product.
/// </summary>
public class TestRDMDeviceModel
{
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0;
    private static readonly ERDM_Parameter FIRST = (ERDM_Parameter)0x9301;
    private static readonly ERDM_Parameter SECOND = (ERDM_Parameter)0x9302;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    #region Models
    private static RDMDeviceInfo info(
        byte major = 1, byte minor = 0, ushort model = 0x0100, uint software = 0x00010000,
        byte numberOfPersonalities = 2, ERDM_ProductCategoryCoarse coarse = ERDM_ProductCategoryCoarse.DIMMER,
        ERDM_ProductCategoryFine fine = ERDM_ProductCategoryFine.DIMMER_CS_LED, byte sensors = 3)
        => new RDMDeviceInfo(
            rdmProtocolVersionMajor: major, rdmProtocolVersionMinor: minor, deviceModelId: model,
            productCategoryCoarse: coarse, productCategoryFine: fine, softwareVersionId: software,
            dmx512CurrentPersonality: 1, dmx512NumberOfPersonalities: numberOfPersonalities, sensorCount: sensors);

    [Test]
    public void TestIsModelOf()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00000001);
        using var model = new RDMDeviceModel(uid, SubDevice.Root, info());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info()), Is.True, "the same");
            Assert.That(model.IsModelOf(new UID(PROTOTYPE_MANUFACTURER, 0x00000002), SubDevice.Root, info()), Is.True, "another device of the model");

            Assert.That(model.IsModelOf(uid, SubDevice.Root, null!), Is.False, "no DEVICE_INFO");
            Assert.That(model.IsModelOf(new UID(0x7FF1, 0x00000001), SubDevice.Root, info()), Is.False, "another manufacturer");
            Assert.That(model.IsModelOf(uid, new SubDevice(1), info()), Is.False, "another sub device");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(model: 0x0101)), Is.False, "another model id");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(major: 2)), Is.False, "another RDM protocol (major)");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(minor: 1)), Is.False, "another RDM protocol (minor)");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(software: 0x00010001)), Is.False, "another software version");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(numberOfPersonalities: 3)), Is.False, "another number of personalities");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(coarse: ERDM_ProductCategoryCoarse.FIXTURE)), Is.False, "another product category");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(fine: ERDM_ProductCategoryFine.DIMMER_DC_PWM)), Is.False, "another fine product category");
            Assert.That(model.IsModelOf(uid, SubDevice.Root, info(sensors: 4)), Is.False, "another number of sensors");
        }
    }

    [Test]
    public void TestModelsAreShared()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00000003);
        var model = RDMDeviceModel.getDeviceModel(uid, SubDevice.Root, info(model: 0x0110), _ => Task.CompletedTask);
        var sameModel = RDMDeviceModel.getDeviceModel(new UID(PROTOTYPE_MANUFACTURER, 0x00000004), SubDevice.Root, info(model: 0x0110), _ => Task.CompletedTask);
        var otherModel = RDMDeviceModel.getDeviceModel(uid, SubDevice.Root, info(model: 0x0111), _ => Task.CompletedTask);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sameModel, Is.SameAs(model), "devices of one model share the model");
            Assert.That(otherModel, Is.Not.SameAs(model));
            Assert.That(RDMDeviceModel.KnownDeviceModels, Does.Contain(model));
            Assert.That(RDMDeviceModel.KnownDeviceModels, Does.Contain(otherModel));
        }
    }

    [Test]
    public void TestToString()
    {
        using var model = new RDMDeviceModel(new UID(PROTOTYPE_MANUFACTURER, 0x00000005), SubDevice.Root, info());
        Assert.That(model.ToString(), Is.EqualTo(Enum.GetName(typeof(EManufacturer), (EManufacturer)PROTOTYPE_MANUFACTURER) ?? string.Empty));
    }

    [Test]
    public void TestWithoutValuesThereAreNoSensorDefinitions()
    {
        using var model = new RDMDeviceModel(new UID(PROTOTYPE_MANUFACTURER, 0x00000006), SubDevice.Root, info());
        Assert.That(model.GetSensorDefinitions(), Is.Empty);
    }

    [Test]
    public async Task TestDispose()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x00000007);
        var model = new RDMDeviceModel(uid, SubDevice.Root, info());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(model.IsDisposed, Is.False);
            Assert.That(model.IsDisposing, Is.False);
        }

        model.Dispose();
        model.Dispose(); // nothing happens the second time
        using (Assert.EnterMultipleScope())
        {
            Assert.That(model.IsDisposed, Is.True);
            Assert.That(model.IsDisposing, Is.False);
        }
        await Task.CompletedTask;
    }
    #endregion

    #region Initialization
    [Test, MaxTime(60000)]
    public async Task TestInitializeRunsOnlyOnce()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x0000000A);
        using var generated = new MockGeneratedDevice1(uid);
        using var model = new RDMDeviceModel(uid, SubDevice.Root, generated.DeviceInfo);

        // the second call while the first one is running does not start another one
        var first = model.Initialize();
        var second = model.Initialize();
        await Task.WhenAll(first, second);
        Assert.That(model.IsInitialized, Is.True);

        // an initialized model is not initialized again
        await model.Initialize();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(model.IsInitialized, Is.True);
            Assert.That(model.IsInitializing, Is.False);
            Assert.That(model.GetSupportedParameters(), Is.Not.Empty);
        }
    }
    #endregion

    #region Manufacturer specific parameters described by the device
    [Test, MaxTime(60000)]
    public async Task TestParametersDescribedByTheDeviceAreReadable()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x0000000B);
        using var generated = new DescribedParametersMockDevice(uid);

        using MockDevice mockDevice = new MockDevice(uid);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);
        await Wait.UntilAsync(() => mockDevice.GetAllParameterValues().ContainsKey(SECOND));

        var values = mockDevice.GetAllParameterValues();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(values, Does.ContainKey(FIRST));
            Assert.That(values, Does.ContainKey(SECOND));
            Assert.That(values[FIRST], Is.EqualTo((byte)7), "parsed with the define made of the description");
            Assert.That(values[SECOND], Is.EqualTo((ushort)0x0102));
            Assert.That(mockDevice.DeviceModel.GetSupportedParameters().Select(p => p.Parameter), Does.Contain(FIRST));
        }
    }

    /// <summary>
    /// The device describes its parameters with PARAMETER_DESCRIPTION, it does not know METADATA_JSON. The model asks once, learns that it is not
    /// supported and does not ask for every manufacturer specific parameter.
    /// </summary>
    [Test, MaxTime(60000)]
    public async Task TestDeviceWithoutMetadataJsonIsKnownNotToSupportIt()
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, 0x0000000C);
        using var generated = new DescribedParametersMockDevice(uid);

        using MockDevice mockDevice = new MockDevice(uid);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        var notSupported = mockDevice.DeviceModel.GetKnownNotSupportedParameters().Select(p => p.Parameter).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(notSupported, Does.Contain(ERDM_Parameter.METADATA_JSON));
            Assert.That(mockDevice.DeviceModel.GetSupportedParameters().Select(p => p.Parameter), Does.Not.Contain(ERDM_Parameter.METADATA_JSON));
        }
    }

    /// <summary>A device of its own model (its own manufacturer specific parameters), so nothing is shared with other tests.</summary>
    private sealed class DescribedParametersMockDevice : MockGeneratedDevice1
    {
        public override ushort DeviceModelID => 0x4D4C;

        public DescribedParametersMockDevice(UID uid) : base(uid, new IModule[] { new DescribedParametersModule() })
        {
        }
    }

    /// <summary>
    /// Two manufacturer specific parameters: a GET returns the value, PARAMETER_DESCRIPTION describes them. There is no METADATA_JSON.
    /// </summary>
    private sealed class DescribedParametersModule : AbstractModule
    {
        public override string DisplayName => "Described Parameters";

        public DescribedParametersModule() : base("DescribedParameters", ERDM_Parameter.PARAMETER_DESCRIPTION, FIRST, SECOND)
        {
        }

        protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            // the index of a GET is the parameter the request is about
            var descriptions = new ConcurrentDictionary<object, object>();
            descriptions.TryAdd(FIRST, new RDMParameterDescription(
                parameterId: (ushort)FIRST, pdlSize: 1, dataType: ERDM_DataType.UINT8, commandClass: ERDM_CommandClass.GET,
                minValidValue: 0, maxValidValue: 255, description: "Fictional Described Level"));
            descriptions.TryAdd(SECOND, new RDMParameterDescription(
                parameterId: (ushort)SECOND, pdlSize: 2, dataType: ERDM_DataType.UINT16, commandClass: ERDM_CommandClass.GET,
                minValidValue: 0, maxValidValue: 65535, description: "Fictional Described Counter"));
            device.setParameterValue(ERDM_Parameter.PARAMETER_DESCRIPTION, descriptions);
        }

        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            return (parameter == FIRST || parameter == SECOND) && command == ERDM_Command.GET_COMMAND;
        }

        protected override RDMMessage handleRequest(RDMMessage message)
        {
            return new RDMMessage()
            {
                DestUID = message.SourceUID,
                SourceUID = message.DestUID,
                Parameter = message.Parameter,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                ParameterData = message.Parameter == FIRST ? new byte[] { 7 } : new byte[] { 0x01, 0x02 }
            };
        }
    }
    #endregion
}
