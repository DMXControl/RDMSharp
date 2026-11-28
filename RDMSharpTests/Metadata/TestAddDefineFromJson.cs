using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharpTests.Metadata;

/// <summary>
/// A device can deliver a define as JSON (METADATA_JSON). <see cref="MetadataFactory.TryAddDefineFromJson"/> registers it for this device,
/// the JSON comes from the device and is not trusted: it has to fit the schema, the requested manufacturer specific parameter and the manufacturer of the device.
///
/// All data is invented: manufacturer 0x7FF0 is the ESTA prototyping range, models, software versions, PIDs and names do not belong to any real product.
/// </summary>
public class TestAddDefineFromJson
{
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0; // ESTA prototyping range, not a real vendor
    private static readonly UID DEVICE_UID = new UID(PROTOTYPE_MANUFACTURER, 0x00C0FFEE);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static RDMDeviceInfo deviceInfo(ushort deviceModelId) => new RDMDeviceInfo(deviceModelId: deviceModelId, softwareVersionId: 0x00020001);

    private static ParameterBag bagFor(RDMDeviceInfo info, ushort pid)
        => new ParameterBag((ERDM_Parameter)pid, DEVICE_UID.ManufacturerID, info.DeviceModelId, info.SoftwareVersionId);

    /// <summary>A compact define (it has to fit one METADATA_JSON message): one uint8 as GET response.</summary>
    internal static string MakeJson(int manufacturerId, int pid, string name = "FICTIONAL_JSON_PARAM")
        => $"{{\"name\":\"{name}\",\"displayName\":\"Fictional\",\"manufacturer_id\":{manufacturerId},\"pid\":{pid},\"version\":1," +
           "\"get_request_subdevice_range\":[\"root\"],\"get_request\":[],\"get_response\":[{\"name\":\"level\",\"type\":\"uint8\"}]}";

    [Test]
    public void TestJsonFitsOneMessage()
    {
        // a device delivers it with one METADATA_JSON response (2 bytes PID + text)
        Assert.That(MakeJson(PROTOTYPE_MANUFACTURER, 0x9001).Length, Is.LessThanOrEqualTo(RDMMetadataJson.PDL_MAX - 2));
    }

    [Test]
    public void TestValidJsonIsRegisteredAndParsesPayload()
    {
        var info = deviceInfo(0xC001);
        const ushort PID = 0x9001;
        Assert.That(MetadataFactory.GetDefine(bagFor(info, PID)), Is.Null, "precondition: not described yet");

        bool added = MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)PID, MakeJson(PROTOTYPE_MANUFACTURER, PID), out var define, out var reason);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.True, reason);
            Assert.That(reason, Is.Null);
            Assert.That(define, Is.Not.Null);
            Assert.That(define!.PID, Is.EqualTo(PID));
            Assert.That(define.Name, Is.EqualTo("FICTIONAL_JSON_PARAM"));
        }
        Assert.That(MetadataFactory.GetDefine(bagFor(info, PID)), Is.SameAs(define), "GetDefine has to return the registered define");

        var branch = MetadataFactory.ParseDataToPayload(define!, ECommandDublicate.GetResponse, new byte[] { 0x2A });
        Assert.That(branch.Children[0].Value, Is.EqualTo((byte)0x2A), "the payload has to be parsed with the JSON define");
    }

    [Test]
    public void TestDefineIsOnlyRegisteredForThisDeviceModel()
    {
        var info = deviceInfo(0xC002);
        var otherModel = deviceInfo(0xC003);
        const ushort PID = 0x9002;
        Assert.That(MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)PID, MakeJson(PROTOTYPE_MANUFACTURER, PID), out _, out var reason), Is.True, reason);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(MetadataFactory.GetDefine(bagFor(info, PID)), Is.Not.Null);
            Assert.That(MetadataFactory.GetDefine(bagFor(otherModel, PID)), Is.Null, "another model does not get the define of this device");
        }
    }

    [Test]
    public void TestJsonReplacesDefineFromParameterDescription()
    {
        var info = deviceInfo(0xC004);
        const ushort PID = 0x9003;
        MetadataFactory.AddDefineFromParameterDescription(DEVICE_UID, SubDevice.Root, info, new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8, commandClass: ERDM_CommandClass.GET,
            minValidValue: 0, maxValidValue: 255, description: "From Description"));
        Assert.That(MetadataFactory.GetDefine(bagFor(info, PID))!.Name, Is.EqualTo("From Description"), "precondition");

        Assert.That(MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)PID, MakeJson(PROTOTYPE_MANUFACTURER, PID, "FROM_JSON"), out _, out var reason), Is.True, reason);
        Assert.That(MetadataFactory.GetDefine(bagFor(info, PID))!.Name, Is.EqualTo("FROM_JSON"), "the JSON is more precise than the parameter description");
    }

    [Test]
    public void TestJsonDefineIsKeptWhenDescriptionComesLater()
    {
        // the order must not matter for the result of a later GetDefine: both write the same key, the last one wins - the JSON is replaced by
        // a later PARAMETER_DESCRIPTION only if TryAdd finds nothing, so the JSON stays
        var info = deviceInfo(0xC005);
        const ushort PID = 0x9004;
        Assert.That(MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)PID, MakeJson(PROTOTYPE_MANUFACTURER, PID, "FROM_JSON"), out _, out var reason), Is.True, reason);
        MetadataFactory.AddDefineFromParameterDescription(DEVICE_UID, SubDevice.Root, info, new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8, commandClass: ERDM_CommandClass.GET,
            minValidValue: 0, maxValidValue: 255, description: "From Description"));
        Assert.That(MetadataFactory.GetDefine(bagFor(info, PID))!.Name, Is.EqualTo("FROM_JSON"));
    }

    [TestCase(0x9010, 0x9010, 0x1234, "manufacturer", Description = "the JSON belongs to another manufacturer")]
    [TestCase(0x9011, 0x9012, PROTOTYPE_MANUFACTURER, "requested", Description = "the JSON describes another parameter than requested")]
    public void TestRejectedJson(int parameter, int pidInJson, int manufacturerInJson, string reasonPart)
    {
        var info = deviceInfo(0xC006);
        bool added = MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)parameter, MakeJson(manufacturerInJson, pidInJson), out var define, out var reason);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.False);
            Assert.That(define, Is.Null);
            Assert.That(reason, Does.Contain(reasonPart));
            Assert.That(MetadataFactory.GetDefine(bagFor(info, (ushort)parameter)), Is.Null, "a rejected JSON is not registered");
        }
    }

    [TestCase((int)ERDM_Parameter.DEVICE_INFO, "not a manufacturer specific", Description = "a device can not redefine a standard parameter")]
    [TestCase((int)ERDM_Parameter.DEVICE_LABEL, "not a manufacturer specific")]
    [TestCase(0xFFE0, "not a manufacturer specific", Description = "above the manufacturer specific range")]
    public void TestStandardParameterCanNotBeRedefined(int parameter, string reasonPart)
    {
        var info = deviceInfo(0xC007);
        // standard parameters have a built-in define (GetDefine throws for a PID above the manufacturer specific range)
        bool isStandard = parameter < 0x8000;
        var before = isStandard ? MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)parameter, 0, info.DeviceModelId, info.SoftwareVersionId)) : null;
        bool added = MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)parameter, MakeJson(PROTOTYPE_MANUFACTURER, parameter), out var define, out var reason);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.False);
            Assert.That(define, Is.Null);
            Assert.That(reason, Does.Contain(reasonPart));
            if (isStandard)
                Assert.That(MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)parameter, 0, info.DeviceModelId, info.SoftwareVersionId)), Is.SameAs(before), "the standard define stays as it is");
        }
    }

    [TestCase("", Description = "empty")]
    [TestCase("   ", Description = "white space")]
    [TestCase("nope", Description = "no JSON")]
    [TestCase("{\"foo\":1}", Description = "valid JSON, but no define (schema)")]
    [TestCase("[1,2,3]", Description = "JSON, but not an object")]
    public void TestInvalidJsonIsRejected(string json)
    {
        var info = deviceInfo(0xC008);
        bool added = MetadataFactory.TryAddDefineFromJson(DEVICE_UID, info, (ERDM_Parameter)0x9020, json, out var define, out var reason);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.False);
            Assert.That(define, Is.Null);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
            Assert.That(MetadataFactory.GetDefine(bagFor(info, 0x9020)), Is.Null);
        }
    }

    [Test]
    public void TestWithoutDeviceInfoIsRejected()
    {
        bool added = MetadataFactory.TryAddDefineFromJson(DEVICE_UID, null!, (ERDM_Parameter)0x9021, MakeJson(PROTOTYPE_MANUFACTURER, 0x9021), out var define, out var reason);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(added, Is.False);
            Assert.That(define, Is.Null);
            Assert.That(reason, Is.Not.Null);
        }
    }
}
