using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;
using System.Text.Json;

namespace RDMSharpTests.Metadata.Parser;

/// <summary>
/// Not a test: writes the layout of all defines (PDL, fields, sizes) as JSON for tools/RdmDumpTests/faulty_gen.py.
/// Run it explicitly, the target file is given by the environment variable RDMSHARP_DEFINE_LAYOUTS.
/// </summary>
[Explicit("Tool for tools/RdmDumpTests, not a test")]
public class ExportDefineLayouts
{
    [Test]
    public async Task Export()
    {
        string? target = Environment.GetEnvironmentVariable("RDMSHARP_DEFINE_LAYOUTS");
        if (string.IsNullOrWhiteSpace(target))
            Assert.Ignore("RDMSHARP_DEFINE_LAYOUTS is not set");

        await MetadataFactory.AwaitInitialize();
        var all = new List<object>();
        foreach (ERDM_Parameter parameter in Enum.GetValues<ERDM_Parameter>().Distinct())
        {
            MetadataJSONObjectDefine define;
            try { define = MetadataFactory.GetDefine(new ParameterBag(parameter)); }
            catch { continue; }
            if (define is null)
                continue;

            foreach (var commandDublicate in new[] { Command.ECommandDublicate.GetRequest, Command.ECommandDublicate.GetResponse, Command.ECommandDublicate.SetRequest, Command.ECommandDublicate.SetResponse })
            {
                try
                {
                    define.GetCommand(commandDublicate, out Command? cmd);
                    if (cmd is null || !cmd.HasValue)
                        continue;
                    var command = cmd.Value;
                    var pdl = command.GetIsEmpty() ? new PDL() : command.GetDataLength();
                    var fields = new List<object>();
                    if (!command.GetIsEmpty())
                    {
                        if (command.SingleField.HasValue)
                            fields.Add(describe(resolve(command.SingleField.Value)));
                        else if (command.ListOfFields != null)
                            fields.AddRange(command.ListOfFields.Select(f => describe(resolve(f))));
                    }
                    all.Add(new Dictionary<string, object?>
                    {
                        ["parameter"] = parameter.ToString(),
                        ["command"] = commandDublicate.ToString(),
                        ["empty"] = command.GetIsEmpty(),
                        ["size"] = pdl.Value,
                        ["min"] = pdl.MinLength,
                        ["max"] = pdl.MaxLength,
                        ["fields"] = fields
                    });
                }
                catch (Exception ex)
                {
                    all.Add(new Dictionary<string, object?> { ["parameter"] = parameter.ToString(), ["command"] = commandDublicate.ToString(), ["error"] = ex.Message });
                }
            }
        }
        File.WriteAllText(target!, JsonSerializer.Serialize(all, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static Dictionary<string, object?> describe(CommonPropertiesForNamed? objectType)
    {
        var description = new Dictionary<string, object?>();
        if (objectType is null)
        {
            description["kind"] = "unknown";
            return description;
        }
        var pdl = objectType.GetDataLength();
        description["name"] = objectType.Name;
        description["size"] = pdl.Value;
        description["min"] = pdl.MinLength;
        description["max"] = pdl.MaxLength;
        switch (objectType)
        {
            case BooleanType:
            case LabeledBooleanType:
                description["kind"] = "bool";
                break;
            case StringType stringType:
                description["kind"] = "string";
                description["minLength"] = stringType.MinLength;
                description["maxLength"] = stringType.MaxLength;
                description["minBytes"] = stringType.MinBytes;
                description["maxBytes"] = stringType.MaxBytes;
                break;
            case ListType listType:
                description["kind"] = "list";
                description["item"] = describe(resolve(listType.ItemType));
                description["minItems"] = listType.MinItems;
                description["maxItems"] = listType.MaxItems;
                break;
            case CompoundType compoundType:
                description["kind"] = "compound";
                description["fields"] = compoundType.Subtypes.Select(s => describe(resolve(s))).ToList();
                break;
            default:
                description["kind"] = objectType.GetType().Name;
                break;
        }
        return description;
    }
    private static CommonPropertiesForNamed? resolve(OneOfTypes oneOf)
    {
        if (oneOf.ObjectType is not null)
            return oneOf.ObjectType;
        if (oneOf.ReferenceType.HasValue)
            return oneOf.ReferenceType.Value.ReferencedObject;
        return null;
    }
}
