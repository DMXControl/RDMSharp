using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.PayloadObject;
using System.Text.Json;
using static RDMSharp.Metadata.JSON.Command;

namespace RDMSharpTests.Metadata;

/// <summary>
/// A remote device can describe its own manufacturer specific parameters at runtime (PARAMETER_DESCRIPTION / METADATA_JSON).
/// <see cref="MetadataFactory.AddDefineFromParameterDescription"/> turns such a description into a define that
/// <see cref="MetadataFactory.GetDefine"/> then returns and that is used to parse the payload.
/// This path is independent of the built-in defines and of the parsed-object type list, so making those load once
/// (the race fixes) must not break it.
///
/// All manufacturer data here is invented: manufacturer 0x7FF0 is the ESTA prototyping range, the device model, software
/// version and PIDs do not belong to any real product - no real device information is used.
/// </summary>
public class TestRuntimeDefineFromParameterDescription
{
    private const ushort PROTOTYPE_MANUFACTURER = 0x7FF0; // ESTA prototyping range, not a real vendor

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    private static (UID uid, RDMDeviceInfo deviceInfo) makeDevice(ushort deviceModelId, uint softwareVersionId, uint id = 0x00ABCDEF)
    {
        var uid = new UID(PROTOTYPE_MANUFACTURER, id);
        var deviceInfo = new RDMDeviceInfo(
            rdmProtocolVersionMajor: 1,
            rdmProtocolVersionMinor: 0,
            deviceModelId: deviceModelId,
            softwareVersionId: softwareVersionId,
            subDeviceCount: 0);
        return (uid, deviceInfo);
    }

    private static ParameterBag bagFor(UID uid, RDMDeviceInfo deviceInfo, ushort pid)
        => new ParameterBag((ERDM_Parameter)pid, uid.ManufacturerID, deviceInfo.DeviceModelId, deviceInfo.SoftwareVersionId);

    [Test]
    public void TestUnknownManufacturerParameterHasNoDefineByDefault()
    {
        // a manufacturer specific PID (0x8000..0xFFDF) that nobody described returns null (and must not throw)
        var (uid, deviceInfo) = makeDevice(0xAB01, 0x00010001);
        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, 0x8F01));
        Assert.That(define, Is.Null, "an undescribed manufacturer parameter has no define yet");
    }

    [Test]
    public void TestAddedRuntimeDefineIsReturnedByGetDefine()
    {
        var (uid, deviceInfo) = makeDevice(0xAB02, 0x00010002);
        const ushort PID = 0x8F02;
        Assert.That(MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID)), Is.Null, "precondition: unknown before it is described");

        var pd = new RDMParameterDescription(
            parameterId: PID,
            pdlSize: 1,
            dataType: ERDM_DataType.UINT8,
            commandClass: ERDM_CommandClass.GET | ERDM_CommandClass.SET,
            unit: ERDM_SensorUnit.NONE,
            prefix: ERDM_UnitPrefix.NONE,
            minValidValue: 0,
            maxValidValue: 200,
            defaultValue: 0,
            description: "Fictional Fan Speed");
        MetadataFactory.AddDefineFromParameterDescription(uid, SubDevice.Root, deviceInfo, pd);

        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(define, Is.Not.Null, "the runtime define has to be returned");
            Assert.That(define!.PID, Is.EqualTo(PID));
            Assert.That(define.ManufacturerID, Is.EqualTo(PROTOTYPE_MANUFACTURER));
            Assert.That(define.DeviceModelID, Is.EqualTo(deviceInfo.DeviceModelId));
            Assert.That(define.Name, Is.EqualTo("Fictional Fan Speed"));
        }
        // GetDefine is cached, so a second call returns the same instance
        Assert.That(MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID)), Is.SameAs(define));
    }

    [Test]
    public void TestRuntimeDefineParsesUint8GetResponse()
    {
        var (uid, deviceInfo) = makeDevice(0xAB03, 0x00010003);
        const ushort PID = 0x8F03;
        var pd = new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8,
            commandClass: ERDM_CommandClass.GET | ERDM_CommandClass.SET, unit: ERDM_SensorUnit.NONE, prefix: ERDM_UnitPrefix.NONE,
            minValidValue: 0, maxValidValue: 255, defaultValue: 0, description: "Fictional Level");
        MetadataFactory.AddDefineFromParameterDescription(uid, SubDevice.Root, deviceInfo, pd);
        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID));
        Assert.That(define, Is.Not.Null);

        var branch = MetadataFactory.ParseDataToPayload(define!, ECommandDublicate.GetResponse, new byte[] { 0x7D });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(branch.IsUnset, Is.False);
            Assert.That(branch.IsEmpty, Is.False);
            Assert.That(branch.Children[0].Value, Is.EqualTo((byte)0x7D), "the payload has to be parsed with the runtime define");
        }
    }

    [Test]
    public void TestRuntimeDefineParsesUint16GetResponse()
    {
        var (uid, deviceInfo) = makeDevice(0xAB04, 0x00010004);
        const ushort PID = 0x8F04;
        var pd = new RDMParameterDescription(
            parameterId: PID, pdlSize: 2, dataType: ERDM_DataType.UINT16,
            commandClass: ERDM_CommandClass.GET, unit: ERDM_SensorUnit.NONE, prefix: ERDM_UnitPrefix.NONE,
            minValidValue: 0, maxValidValue: 0xFFFF, defaultValue: 0, description: "Fictional Counter");
        MetadataFactory.AddDefineFromParameterDescription(uid, SubDevice.Root, deviceInfo, pd);
        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID));
        Assert.That(define, Is.Not.Null);

        var branch = MetadataFactory.ParseDataToPayload(define!, ECommandDublicate.GetResponse, new byte[] { 0x12, 0x34 });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(branch.IsUnset, Is.False);
            Assert.That(branch.Children[0].Value, Is.EqualTo((ushort)0x1234));
        }
    }

    [Test]
    public void TestRuntimeDefineWithLabelsParsesAndKeepsLabel()
    {
        // a description with "value=label" pairs becomes a labeled integer (same convention as the live W-DMX parameters)
        var (uid, deviceInfo) = makeDevice(0xAB05, 0x00010005);
        const ushort PID = 0x8F05;
        var pd = new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8,
            commandClass: ERDM_CommandClass.GET, unit: ERDM_SensorUnit.NONE, prefix: ERDM_UnitPrefix.NONE,
            minValidValue: 0, maxValidValue: 2, defaultValue: 0, description: "Fictional Mode 0=Off 1=On 2=Auto");
        MetadataFactory.AddDefineFromParameterDescription(uid, SubDevice.Root, deviceInfo, pd);
        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, PID));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(define, Is.Not.Null);
            Assert.That(define!.Name, Is.EqualTo("Fictional Mode 0=Off 1=On 2=Auto"));
        }
        var branch = MetadataFactory.ParseDataToPayload(define!, ECommandDublicate.GetResponse, new byte[] { 0x02 });
        Assert.That(branch.Children[0].Value, Is.EqualTo((byte)0x02));
    }

    private static MetadataJSONObjectDefine addDefine(ushort deviceModelId, ushort pid, RDMParameterDescription description)
    {
        var (uid, deviceInfo) = makeDevice(deviceModelId, 0x00020000u + deviceModelId);
        MetadataFactory.AddDefineFromParameterDescription(uid, SubDevice.Root, deviceInfo, description);
        var define = MetadataFactory.GetDefine(bagFor(uid, deviceInfo, pid));
        Assert.That(define, Is.Not.Null);
        return define!;
    }

    [Test]
    public void TestRuntimeDefineOfAString()
    {
        const ushort PID = 0x8F20;
        var define = addDefine(0xAB10, PID, new RDMParameterDescription(
            parameterId: PID, pdlSize: 8, dataType: ERDM_DataType.STRING,
            commandClass: ERDM_CommandClass.GET, description: "Fictional Text"));

        var branch = MetadataFactory.ParseDataToPayload(define, ECommandDublicate.GetResponse, System.Text.Encoding.ASCII.GetBytes("Hello"));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(branch.IsUnset, Is.False);
            Assert.That(branch.Children[0].Value, Is.EqualTo("Hello"));
        }
    }

    [Test]
    public void TestRuntimeDefineOfSignedIntegers()
    {
        const ushort PID_8 = 0x8F21;
        const ushort PID_16 = 0x8F22;
        var define8 = addDefine(0xAB11, PID_8, new RDMParameterDescription(
            parameterId: PID_8, pdlSize: 1, dataType: ERDM_DataType.INT8,
            commandClass: ERDM_CommandClass.GET, minValidValue: 0, maxValidValue: 100, description: "Fictional Offset"));
        var define16 = addDefine(0xAB12, PID_16, new RDMParameterDescription(
            parameterId: PID_16, pdlSize: 2, dataType: ERDM_DataType.INT16,
            commandClass: ERDM_CommandClass.GET, minValidValue: 0, maxValidValue: 1000, description: "Fictional Position"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(MetadataFactory.ParseDataToPayload(define8, ECommandDublicate.GetResponse, new byte[] { 0xFE }).Children[0].Value, Is.EqualTo((sbyte)-2));
            Assert.That(MetadataFactory.ParseDataToPayload(define16, ECommandDublicate.GetResponse, new byte[] { 0xFF, 0xFE }).Children[0].Value, Is.EqualTo((short)-2));
        }
    }

    [Test]
    public void TestRuntimeDefineOfAnotherDataTypeHasNoFields()
    {
        // a data type that can not be described as a field: the parameter is known, but nothing can be parsed
        const ushort PID = 0x8F23;
        var define = addDefine(0xAB13, PID, new RDMParameterDescription(
            parameterId: PID, pdlSize: 4, dataType: ERDM_DataType.UINT32,
            commandClass: ERDM_CommandClass.GET, description: "Fictional Counter 32"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(define.GetRequest.HasValue, Is.True);
            Assert.That(define.GetResponse.HasValue, Is.True);
            Assert.That(define.GetResponse!.Value.GetIsEmpty(), Is.True, "no field for this data type");
        }
    }

    [Test]
    public void TestRuntimeDefineOfASetOnlyParameter()
    {
        const ushort PID = 0x8F24;
        var define = addDefine(0xAB14, PID, new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8,
            commandClass: ERDM_CommandClass.SET, minValidValue: 1, maxValidValue: 2, description: "Fictional Action"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(define.GetRequest.HasValue, Is.False, "nothing can be read");
            Assert.That(define.GetResponse.HasValue, Is.False);
            Assert.That(define.SetRequest.HasValue, Is.True);
            Assert.That(define.SetResponse.HasValue, Is.True);
        }
    }

    [Test]
    public void TestRuntimeDefineWithLabelsNameFirst()
    {
        // the labels of a description are "value=name" or "name=value"
        const ushort PID = 0x8F25;
        var define = addDefine(0xAB15, PID, new RDMParameterDescription(
            parameterId: PID, pdlSize: 1, dataType: ERDM_DataType.UINT8,
            commandClass: ERDM_CommandClass.GET, minValidValue: 0, maxValidValue: 2, description: "Fictional Mode Off=0 On=1 Auto=2"));

        var branch = MetadataFactory.ParseDataToPayload(define, ECommandDublicate.GetResponse, new byte[] { 0x01 });
        Assert.That(branch.Children[0].Value, Is.EqualTo((byte)0x01));
    }

    // An invented JSON define in the same schema as the built-in resources and as a device's METADATA_JSON.
    // Manufacturer 32752 (0x7FF0, prototyping) and PID 36624 (0x8F10) are invented - no real product.
    private const string FICTIONAL_JSON_DEFINE = """
    {
      "name": "FICTIONAL_RUNTIME_PARAM",
      "displayName": "Fictional Runtime Param",
      "manufacturer_id": 32752,
      "pid": 36624,
      "version": 1,
      "get_request_subdevice_range": [ "root" ],
      "get_request": [],
      "get_response": [
        {
          "name": "level",
          "type": "uint8",
          "labels": [
            { "name": "Off", "value": 0 },
            { "name": "Full", "value": 255 }
          ]
        }
      ],
      "set_request_subdevice_range": [ "root" ],
      "set_request": "get_response",
      "set_response": []
    }
    """;

    /// <summary>
    /// A define can also arrive as a JSON string (the same shape as the embedded resources, and what a device sends via
    /// METADATA_JSON). It is deserialized into a <see cref="MetadataJSONObjectDefine"/> with the same deserializer the
    /// built-in defines use, and then parses a payload - independent of the race-fixed built-in define list.
    /// </summary>
    [Test]
    public void TestDefineComingInAsJsonDeserializesAndParses()
    {
        var define = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(FICTIONAL_JSON_DEFINE);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(define, Is.Not.Null);
            Assert.That(define!.PID, Is.EqualTo(0x8F10));
            Assert.That(define.ManufacturerID, Is.EqualTo(PROTOTYPE_MANUFACTURER));
            Assert.That(define.Name, Is.EqualTo("FICTIONAL_RUNTIME_PARAM"));
        }

        var branch = MetadataFactory.ParseDataToPayload(define!, ECommandDublicate.GetResponse, new byte[] { 0xFF });
        using (Assert.EnterMultipleScope())
        {
            Assert.That(branch.IsUnset, Is.False);
            Assert.That(branch.IsEmpty, Is.False);
            Assert.That(branch.Children[0].Value, Is.EqualTo((byte)0xFF), "the JSON define has to parse the payload");
        }

        // round trip: the parsed value encodes back to the same byte
        var data = MetadataFactory.ParsePayloadToData(define!, ECommandDublicate.GetResponse, branch).First();
        Assert.That(data, Is.EqualTo(new byte[] { 0xFF }));
    }
}
