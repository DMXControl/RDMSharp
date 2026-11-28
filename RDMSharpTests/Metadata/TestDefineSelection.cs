using RDMSharp.Metadata;
using System.Text.Json;

namespace RDMSharpTests.Metadata;

/// <summary>
/// <see cref="MetadataFactory.selectDefine"/>: several defines for one PID and manufacturer (for all models, for a model, for a software version).
/// The defines are invented (manufacturer 0x7FF0 is the ESTA prototyping range).
/// </summary>
public class TestDefineSelection
{
    private const ushort MANUFACTURER = 0x7FF0;
    private const ushort PID = 0x9100;

    private static MetadataJSONObjectDefine define(string name, ushort? model = null, uint? software = null, int version = 1)
    {
        string json = $$"""
        {
          "name": "{{name}}",
          "manufacturer_id": {{MANUFACTURER}},
          {{(model is null ? "" : $"\"device_model_id\": {model},")}}
          {{(software is null ? "" : $"\"software_version_id\": {software},")}}
          "pid": {{PID}},
          "version": {{version}},
          "get_request_subdevice_range": [ "root" ],
          "get_request": [],
          "get_response": [ { "name": "level", "type": "uint8" } ]
        }
        """;
        return JsonSerializer.Deserialize<MetadataJSONObjectDefine>(json)!;
    }

    private static string? select(ParameterBag bag, params MetadataJSONObjectDefine[] defines) => MetadataFactory.selectDefine(defines, bag)?.Name;
    private static ParameterBag bag(ushort? model = null, uint? software = null) => new ParameterBag((ERDM_Parameter)PID, MANUFACTURER, model, software);

    [Test]
    public void TestDefineOfTheModelWinsOverTheGeneralOne()
    {
        // the general define had the higher version, which used to win regardless of the model
        var all = define("all", version: 2);
        var model = define("model", model: 0x0010);
        Assert.That(select(bag(0x0010, 1), all, model), Is.EqualTo("model"));
    }

    [Test]
    public void TestWithoutModelTheGeneralDefineIsUsed()
    {
        var all = define("all");
        var model = define("model", model: 0x0010, version: 3);
        Assert.That(select(bag(), all, model), Is.EqualTo("all"));
    }

    [Test]
    public void TestOtherModelGetsTheGeneralDefineOrNone()
    {
        var all = define("all");
        var model = define("model", model: 0x0010);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(select(bag(0x0020, 1), all, model), Is.EqualTo("all"));
            Assert.That(select(bag(0x0020, 1), model, define("model2", model: 0x0011)), Is.Null, "a define of another model does not fit");
        }
    }

    [Test]
    public void TestSoftwareVersionOfTheModel()
    {
        var model = define("model", model: 0x0010);
        var v2 = define("v2", model: 0x0010, software: 2);
        var v5 = define("v5", model: 0x0010, software: 5);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(select(bag(0x0010, 2), model, v2, v5), Is.EqualTo("v2"), "exact software version");
            Assert.That(select(bag(0x0010, 3), model, v2, v5), Is.EqualTo("model"), "no exact version: the define for all versions of the model");
            Assert.That(select(bag(0x0010, 4), v2, v5), Is.EqualTo("v5"), "neither: the closest software version");
        }
    }

    [Test]
    public void TestEqualDefinesTheHighestVersionWins()
    {
        Assert.That(select(bag(0x0010, 1), define("old", model: 0x0010), define("new", model: 0x0010, version: 2)), Is.EqualTo("new"));
    }
}
