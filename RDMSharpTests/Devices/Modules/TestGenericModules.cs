using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module.Generic;
using System.Text.Json;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// <see cref="GenericModuleFactory"/>: a simple define (one boolean, integer or string field, GET without request data and/or SET)
/// becomes a generic module, everything else needs a module written for it.
/// The defines are invented (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestGenericModules
{
    private const ushort MANUFACTURER = 0x7FF0;
    private const ERDM_Parameter PID = (ERDM_Parameter)0x9300;

    [OneTimeSetUp]
    public async Task OneTimeSetup() => await MetadataFactory.AwaitInitialize();

    private static MetadataJSONObjectDefine define(string getRequest, string getResponse, string? setRequest = null, ushort manufacturer = MANUFACTURER, ushort pid = (ushort)PID)
    {
        string get = getRequest is null ? "" : $"\"get_request_subdevice_range\": [ \"root\" ], \"get_request\": {getRequest}, \"get_response\": {getResponse},";
        string set = setRequest is null ? "" : $"\"set_request_subdevice_range\": [ \"root\" ], \"set_request\": {setRequest}, \"set_response\": [],";
        string json = $$"""
        {
          "name": "FICTIONAL_GENERIC",
          "displayName": "Fictional Generic",
          "manufacturer_id": {{manufacturer}},
          "pid": {{pid}},
          {{get}}
          {{set}}
          "version": 1
        }
        """;
        return JsonSerializer.Deserialize<MetadataJSONObjectDefine>(json)!;
    }

    private static IGenericModule? create(MetadataJSONObjectDefine define, ERDM_Parameter parameter = PID)
        => GenericModuleFactory.TryCreate(null!, parameter, define, out var module) ? module : null;

    [Test]
    public void TestBoolean()
    {
        var module = create(define("[]", """[ { "name": "state", "type": "boolean", "labels": [ { "name": "Off", "value": false }, { "name": "On", "value": true } ] } ]""", "\"get_response\""));
        Assert.That(module, Is.TypeOf<GenericBooleanModule>());
        var boolean = (GenericBooleanModule)module!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(boolean.Parameter, Is.EqualTo(PID));
            Assert.That(boolean.Name, Is.EqualTo("Generic_0x7FF0_0x9300"), "manufacturer and PID: a manufacturer PID means something different for every manufacturer");
            Assert.That(boolean.SupportedCommandClass, Is.EqualTo(ERDM_CommandClass.GET | ERDM_CommandClass.SET));
            Assert.That(boolean.DisplayName, Is.EqualTo("Fictional Generic"));
            Assert.That(boolean.FalseLabel, Is.EqualTo("Off"));
            Assert.That(boolean.TrueLabel, Is.EqualTo("On"));
            Assert.That(boolean.SupportedParameters, Is.EqualTo(new[] { PID }));
            Assert.That(boolean.Define, Is.Not.Null);
        }
    }

    [Test]
    public void TestLabeledInteger()
    {
        var module = create(define("[]", """[ { "name": "mode", "type": "uint8", "restrictToLabeled": true, "labels": [ { "name": "Slow", "value": 0 }, { "name": "Fast", "value": 1 } ] } ]""", "\"get_response\""));
        Assert.That(module, Is.TypeOf<GenericLabeledIntegerModule<byte>>(), "the value of the device is stored as byte for uint8");
        var labeled = (GenericLabeledIntegerModule<byte>)module!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(labeled.Labels.Select(l => (l.Key, l.Value)), Is.EqualTo(new[] { (0L, "Slow"), (1L, "Fast") }));
            Assert.That(labeled.RestrictToLabeled, Is.True);
        }
    }

    [Test]
    public void TestOpenLabelsStayANumber()
    {
        // labels without restrictToLabeled name special values (e.g. 0 = off), the value is a number
        var module = create(define("[]", """[ { "name": "timeout", "type": "uint8", "labels": [ { "name": "Off", "value": 0 } ] } ]""", "\"get_response\""));
        Assert.That(module, Is.TypeOf<GenericNumericModule<byte>>());
        var numeric = (GenericNumericModule<byte>)module!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(numeric.Labels.Select(l => (l.Key, l.Value)), Is.EqualTo(new[] { (0L, "Off") }));
            Assert.That(numeric.RestrictToLabeled, Is.False);
        }
    }

    [Test]
    public void TestIntegerWithRange()
    {
        var module = create(define("[]", """[ { "name": "level", "type": "uint16", "ranges": [ { "minimum": 10, "maximum": 500 } ] } ]"""));
        Assert.That(module, Is.TypeOf<GenericNumericModule<ushort>>());
        var numeric = (GenericNumericModule<ushort>)module!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(numeric.SupportedCommandClass, Is.EqualTo(ERDM_CommandClass.GET), "no set_request: read only");
            Assert.That(numeric.MinValue, Is.EqualTo((ushort)10));
            Assert.That(numeric.MaxValue, Is.EqualTo((ushort)500));
        }
    }

    [Test]
    public void TestString()
    {
        var module = create(define("[]", """[ { "name": "label", "type": "string", "maxLength": 16 } ]""", "\"get_response\""));
        Assert.That(module, Is.TypeOf<GenericStringModule>());
        Assert.That(((GenericStringModule)module!).MaxLength, Is.EqualTo(16u));
    }

    [Test]
    public void TestSetOnlyWithOneField()
    {
        var module = create(define(null!, null!, """[ { "name": "level", "type": "uint8" } ]"""));
        Assert.That(module, Is.TypeOf<GenericNumericModule<byte>>());
        Assert.That(module!.SupportedCommandClass, Is.EqualTo(ERDM_CommandClass.SET));
    }

    [Test]
    public void TestNotSimpleIsNotCreated()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(create(define("""[ { "name": "index", "type": "uint8" } ]""", """[ { "name": "index", "type": "uint8" }, { "name": "value", "type": "uint8" } ]""")), Is.Null, "indexed GET");
            Assert.That(create(define("[]", """[ { "name": "x", "type": "uint16" }, { "name": "y", "type": "uint16" } ]""")), Is.Null, "two fields");
            Assert.That(create(define("[]", """[ { "name": "data", "type": "bytes" } ]""")), Is.Null, "bytes");
            Assert.That(create(define(null!, null!, "[]")), Is.Null, "SET without data is an action, not a value");
        }
    }

    [Test]
    public void TestEstaParameterWithDefinedTypeIsNotCreated()
    {
        // DMX_START_ADDRESS has no C# type, it is a plain uint16 (its labels name special values, it is not limited to them);
        // DEVICE_INFO is stored as RDMDeviceInfo
        var startAddress = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DMX_START_ADDRESS));
        var deviceInfo = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.DEVICE_INFO));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(create(startAddress, ERDM_Parameter.DMX_START_ADDRESS), Is.TypeOf<GenericNumericModule<ushort>>());
            Assert.That(create(deviceInfo, ERDM_Parameter.DEVICE_INFO), Is.Null);
            Assert.That(create(startAddress, ERDM_Parameter.DMX_START_ADDRESS)!.Name, Is.EqualTo("Generic_0x0000_0x00F0"), "an ESTA parameter has manufacturer 0x0000");
        }
    }
}
