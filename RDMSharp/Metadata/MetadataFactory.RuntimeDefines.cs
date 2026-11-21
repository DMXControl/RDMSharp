using Json.Schema;
using Microsoft.Extensions.Logging;
using RDMSharp.Metadata.JSON;
using RDMSharp.PayloadObject;
using RDMSharp.RDM;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;

namespace RDMSharp.Metadata;

public static partial class MetadataFactory
{
    /// <summary>
    /// First PID of the manufacturer specific range (E1.20), a device may only describe PIDs of this range itself.
    /// </summary>
    private const ushort MANUFACTURER_SPECIFIC_PID_MIN = 0x8000;
    /// <summary>
    /// Last PID of the manufacturer specific range (E1.20).
    /// </summary>
    private const ushort MANUFACTURER_SPECIFIC_PID_MAX = 0xFFDF;

    // Own registry: the runtime check must not interfere with the loading of the built-in defines.
    private static readonly BuildOptions runtimeSchemaBuildOptions = new BuildOptions { SchemaRegistry = new SchemaRegistry() };
    private static readonly ConcurrentDictionary<string, JsonSchema> runtimeJsonSchemas = new ConcurrentDictionary<string, JsonSchema>();
    private static readonly object runtimeJsonSchemasLock = new object();

    private static JsonSchema getRuntimeJsonSchema(MetadataVersion schema)
    {
        if (runtimeJsonSchemas.TryGetValue(schema.Version, out var jsonSchema))
            return jsonSchema;

        lock (runtimeJsonSchemasLock)
        {
            if (!runtimeJsonSchemas.TryGetValue(schema.Version, out jsonSchema))
            {
                jsonSchema = JsonSchema.FromText(new MetadataBag(schema).Content, runtimeSchemaBuildOptions);
                runtimeJsonSchemas.TryAdd(schema.Version, jsonSchema);
            }
        }
        return jsonSchema;
    }

    /// <summary>
    /// Registers a define a device delivered as JSON (METADATA_JSON) for this device, the define is then returned by
    /// <see cref="GetDefine"/> for the parameter of this device (model and software version) and used to parse its payloads.
    /// </summary>
    /// <remarks>
    /// The JSON comes from the device, so it is not trusted. It is only accepted if
    /// <list type="bullet">
    /// <item>it is valid against the JSON schema of the defines (like the built-in defines),</item>
    /// <item>it describes the requested <paramref name="parameter"/>,</item>
    /// <item>the parameter is manufacturer specific (0x8000 - 0xFFDF), a device can not redefine a standard parameter and</item>
    /// <item>it belongs to the manufacturer of the device (<see cref="UID.ManufacturerID"/>).</item>
    /// </list>
    /// A define that was derived from a PARAMETER_DESCRIPTION for the same parameter is replaced, the JSON is more precise.
    /// </remarks>
    /// <param name="uid">The device that delivered the JSON.</param>
    /// <param name="deviceInfo">The DEVICE_INFO of the device (model and software version the define is registered for).</param>
    /// <param name="parameter">The parameter the device described (the PID of the METADATA_JSON request).</param>
    /// <param name="json">The JSON define.</param>
    /// <param name="define">The registered define, otherwise null.</param>
    /// <param name="reason">Why the JSON was rejected, otherwise null.</param>
    /// <returns>true if the define was registered.</returns>
    public static bool TryAddDefineFromJson(UID uid, RDMDeviceInfo deviceInfo, ERDM_Parameter parameter, string json, out MetadataJSONObjectDefine define, out string reason)
    {
        define = null;
        reason = null;
        try
        {
            if (deviceInfo == null)
            {
                reason = "No DEVICE_INFO of the device";
                return false;
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                reason = "The JSON is empty";
                return false;
            }
            ushort pid = (ushort)parameter;
            if (pid < MANUFACTURER_SPECIFIC_PID_MIN || pid > MANUFACTURER_SPECIFIC_PID_MAX)
            {
                reason = $"0x{pid:X4} is not a manufacturer specific parameter, a device can only describe its own parameters";
                return false;
            }

            var schemaVersion = waitForSchemaVersion();
            using JsonDocument document = JsonDocument.Parse(json);
            EvaluationResults result = getRuntimeJsonSchema(schemaVersion).Evaluate(document.RootElement, options);
            if (!result.IsValid)
            {
                var errors = result.Details
                    .Where(x => !x.IsValid)
                    .SelectMany(x => x.Errors?.Select(error => $"{x.InstanceLocation}: {error.Value}") ?? Enumerable.Empty<string>());
                reason = $"The JSON does not fit the schema: {string.Join("; ", errors)}";
                return false;
            }

            MetadataJSONObjectDefine jsonDefine = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(json);
            if (jsonDefine == null)
            {
                reason = "The JSON can not be read as a define";
                return false;
            }
            if (jsonDefine.PID != pid)
            {
                reason = $"The JSON describes 0x{jsonDefine.PID:X4}, requested was 0x{pid:X4}";
                return false;
            }
            if (jsonDefine.ManufacturerID != uid.ManufacturerID)
            {
                reason = $"The JSON belongs to manufacturer 0x{jsonDefine.ManufacturerID:X4}, the device to 0x{uid.ManufacturerID:X4}";
                return false;
            }

            // the same key AddDefineFromParameterDescription and GetDefine use for this device
            parameterBagDefineCache[new ParameterBag(parameter, uid.ManufacturerID, deviceInfo.DeviceModelId, deviceInfo.SoftwareVersionId)] = jsonDefine;
            define = jsonDefine;
            return true;
        }
        catch (Exception ex)
        {
            reason = $"{ex.GetType().Name}: {ex.Message}";
            Logger?.LogWarning(ex, $"Can't register the JSON define of {uid} for {parameter}");
            return false;
        }
    }

    private static MetadataVersion waitForSchemaVersion()
    {
        waitForDefines();
        return GetMetadataSchemaVersions().First();
    }
}
