using Json.Schema;
using Microsoft.Extensions.Logging;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata;

public static partial class MetadataFactory
{
    private static readonly ILogger Logger = Logging.CreateLogger(typeof(MetadataFactory));
    private const string SCHEMA_FILE_NAME = "schema.json";
    private const string JSON_ENDING = ".json";
    private static volatile ConcurrentDictionary<string, MetadataVersion> metadataVersionList;
    private static readonly object metadataVersionListLock = new object();
    // published only when it is completely loaded
    private static volatile ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>> metadataVersionDefinesBagDictionary;
    private static readonly ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine> parameterBagDefineCache = new ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine>();
    private static readonly List<Assembly> resourceProvider = new List<Assembly>() { typeof(MetadataFactory).Assembly };
    public static bool IsInitialized = false;
    /// <summary>
    /// The defines are loaded once, every caller waits for the same Task (the first access may come from many threads at the same time).
    /// </summary>
    private static readonly Lazy<Task> loadDefinesTask = new Lazy<Task>(() => Task.Run(loadDefines), LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly SemaphoreSlim readSchemaSemaphoreSlim = new SemaphoreSlim(1, 1);
    private static readonly BuildOptions schemaBuildOptions = new BuildOptions { SchemaRegistry = new SchemaRegistry() };

    private static readonly EvaluationOptions options = new EvaluationOptions
    {
        OutputFormat = OutputFormat.List
    };

    public static void AddResourceProvider(Assembly assembly)
    {
        if (!resourceProvider.Contains(assembly))
            resourceProvider.Add(assembly);
    }
    public static IReadOnlyDictionary<string, MetadataVersion> MetadataVersionList
    {
        get
        {
            var list = metadataVersionList;
            if (list != null)
                return list.AsReadOnly();

            lock (metadataVersionListLock)
            {
                list = metadataVersionList;
                if (list == null)
                {
                    var loading = new ConcurrentDictionary<string, MetadataVersion>();
                    foreach (Assembly assembly in resourceProvider)
                    {
                        try
                        {
                            var resoures = GetResources(assembly);
                            var metaDataVersions = resoures.Select(r => new MetadataVersion(r, assembly));
                            foreach (var mv in metaDataVersions)
                                loading.TryAdd(mv.Path, mv);
                        }
                        catch (Exception e)
                        {
                            Logger?.LogError(e);
                        }
                    }
                    metadataVersionList = list = loading;
                }
            }
            return list.AsReadOnly();
        }
    }
    public static IReadOnlyCollection<string> GetResources(Assembly assembly)
    {
        return assembly.GetManifestResourceNames().Where(p => p.EndsWith(JSON_ENDING)).ToList().AsReadOnly();
    }
    /// <summary>
    /// Starts loading the defines (only once) and returns the Task every caller can wait for.
    /// </summary>
    private static Task fillDefaultMetadataVersionList()
    {
        return loadDefinesTask.Value;
    }
    private static async Task loadDefines()
    {
        // the defines are collected in a local dictionary and published when all of them are loaded
        var defines = new ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>>();
        {
            var schemaList = GetMetadataSchemaVersions();
            ConcurrentDictionary<string, JsonSchema> versionSchemas = new ConcurrentDictionary<string, JsonSchema>();
            var nonSchemaVersions = metadataVersionList.Values.Where(_mv => !_mv.IsSchema).ToList();
            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };
            await Parallel.ForEachAsync(nonSchemaVersions, parallelOptions, async (mv, token) =>
            {
                try
                {
                    var schema = schemaList.First(s => s.Version.Equals(mv.Version));
                    JsonSchema jsonSchema;
                    await readSchemaSemaphoreSlim.WaitAsync();
                    try
                    {
                        if (!versionSchemas.TryGetValue(schema.Version, out jsonSchema))
                        {
                            jsonSchema = JsonSchema.FromText(new MetadataBag(schema).Content, schemaBuildOptions);
                            versionSchemas.TryAdd(schema.Version, jsonSchema);
                        }
                    }
                    finally
                    {
                        readSchemaSemaphoreSlim.Release();
                    }

                    MetadataBag metadataBag = new MetadataBag(mv);
                    using JsonDocument doc = JsonDocument.Parse(metadataBag.Content);
                    EvaluationResults result = jsonSchema.Evaluate(doc.RootElement, options);
                    if (result.IsValid)
                    {
                        MetadataJSONObjectDefine jsonDefine = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(metadataBag.Content);
                        defines.AddOrUpdate(schema,
                            _ => new List<MetadataJSONObjectDefine> { jsonDefine },
                            (_, list) =>
                            {
                                lock (list)
                                {
                                    list.Add(jsonDefine);
                                    return list;
                                }
                            });
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();

                        var errors = result.Details
                            .Where(x => !x.IsValid)
                            .SelectMany(x => x.Errors?.Select(error =>
                                $"{x.InstanceLocation}: {error.Value}")
                                ?? Enumerable.Empty<string>());

                        foreach (var error in errors)
                            sb.AppendLine(error);
                        throw new Exception($"Schema Invalid for {mv.Name}{Environment.NewLine}{Environment.NewLine}Schema result{sb}");
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"Exception while Deserialize {mv.Name}");
                }
            });
            metadataVersionDefinesBagDictionary = defines;
            IsInitialized = true;
        }
    }

    public static IReadOnlyCollection<MetadataVersion> GetMetadataSchemaVersions()
    {
        return MetadataVersionList.Values.Where(r => r.IsSchema).ToList().AsReadOnly();
    }
    public static IReadOnlyCollection<MetadataVersion> GetMetadataDefineVersions()
    {
        try
        {
            return MetadataVersionList.Values.Where(r => !r.IsSchema).ToList().AsReadOnly();
        }
        finally
        {
            _ = fillDefaultMetadataVersionList();
        }
    }
    public static MetadataJSONObjectDefine GetDefine(ParameterBag parameter)
    {
        try
        {
            if (parameterBagDefineCache.TryGetValue(parameter, out var define))
                return define;

            define = getDefine(parameter);
            if (define != null)
            {
                parameterBagDefineCache.TryAdd(parameter, define);
                return define;
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex);
        }
        if ((ushort)parameter.PID < 0x8000 || (ushort)parameter.PID > 0xFFDF)
            throw new DefineNotFoundException($"{parameter}");

        return null;
    }
    public static void AddDefineFromParameterDescription(UID uid, SubDevice subDevice, RDMDeviceInfo deviceInfo, RDMParameterDescription parameterDescription)
    {
        SubdevicesForRequests[] getRequestSubdeviceRange = null;
        SubdevicesForResponses[] getResponseSubdeviceRange = null;
        SubdevicesForRequests[] setRequestSubdeviceRange = null;
        SubdevicesForResponses[] setResponseSubdeviceRange = null;
        Command? getCommandRequest = null;
        Command? getCommandResponse = null;
        Command? setCommandRequest = null;
        Command? setCommandResponse = null;
        if (deviceInfo.SubDeviceCount == 0)
        {
            if (parameterDescription.CommandClass.HasFlag(ERDM_CommandClass.GET))
            {
                getRequestSubdeviceRange = new SubdevicesForRequests[] { new SubdevicesForRequests(SubdevicesForRequests.ESubdevicesForRequests.Root) };
                getResponseSubdeviceRange = new SubdevicesForResponses[] { new SubdevicesForResponses(SubdevicesForResponses.ESubdevicesForResponses.Root) };
            }
            if (parameterDescription.CommandClass.HasFlag(ERDM_CommandClass.SET))
            {
                setRequestSubdeviceRange = new SubdevicesForRequests[] { new SubdevicesForRequests(SubdevicesForRequests.ESubdevicesForRequests.Root) };
                setResponseSubdeviceRange = new SubdevicesForResponses[] { new SubdevicesForResponses(SubdevicesForResponses.ESubdevicesForResponses.Root) };
            }
        }

        string name = parameterDescription.Description;
        string displayName = getDisplayName(parameterDescription);

        if (parameterDescription.CommandClass == ERDM_CommandClass.SET)
        {
            setCommandRequest = new Command();
            setCommandResponse = new Command();
        }
        else
        {
            if (parameterDescription.CommandClass.HasFlag(ERDM_CommandClass.GET))
            {
                getCommandRequest = new Command();
                OneOfTypes? oneOfType = null;
                LabeledIntegerType[] labeledIntegerTypes = getLabeledIntegerTypes(parameterDescription);
                switch (parameterDescription.DataType)
                {
                    case ERDM_DataType.STRING:
                        oneOfType = new OneOfTypes(new StringType(name, displayName, null, null, "string", null, null, 0, parameterDescription.PDLSize, null, null, true));
                        break;
                    case ERDM_DataType.UINT8:
                        oneOfType = new OneOfTypes(new IntegerType<byte>(name, displayName, null, null, EIntegerType.UInt8, labeledIntegerTypes, labeledIntegerTypes != null, new Range<byte>[] { new Range<byte>((byte)parameterDescription.MinValidValue, (byte)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;
                    case ERDM_DataType.INT8:
                        oneOfType = new OneOfTypes(new IntegerType<sbyte>(name, displayName, null, null, EIntegerType.Int8, labeledIntegerTypes, labeledIntegerTypes != null, new Range<sbyte>[] { new Range<sbyte>((sbyte)parameterDescription.MinValidValue, (sbyte)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;

                    case ERDM_DataType.UINT16:
                        oneOfType = new OneOfTypes(new IntegerType<ushort>(name, displayName, null, null, EIntegerType.UInt16, labeledIntegerTypes, labeledIntegerTypes != null, new Range<ushort>[] { new Range<ushort>((ushort)parameterDescription.MinValidValue, (ushort)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;
                    case ERDM_DataType.INT16:
                        oneOfType = new OneOfTypes(new IntegerType<short>(name, displayName, null, null, EIntegerType.Int16, labeledIntegerTypes, labeledIntegerTypes != null, new Range<short>[] { new Range<short>((short)parameterDescription.MinValidValue, (short)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;
                    default:
                        break;
                }
                if (oneOfType.HasValue)
                    getCommandResponse = new Command(oneOfType.Value);
                else
                    getCommandResponse = new Command();
            }
            if (parameterDescription.CommandClass.HasFlag(ERDM_CommandClass.SET))
            {
                setCommandRequest = new Command(Command.ECommandDublicate.GetResponse);
                setCommandResponse = new Command();
            }
        }
        MetadataJSONObjectDefine define = new MetadataJSONObjectDefine(
            parameterDescription.Description,
            displayName,
            null,
            uid.ManufacturerID,
            deviceInfo.DeviceModelId,
            deviceInfo.SoftwareVersionId,
            parameterDescription.ParameterId,
            0,
            getRequestSubdeviceRange,
            getResponseSubdeviceRange,
            setRequestSubdeviceRange,
            setResponseSubdeviceRange,
            getCommandRequest,
            getCommandResponse,
            setCommandRequest,
            setCommandResponse);

        parameterBagDefineCache.TryAdd(new ParameterBag((ERDM_Parameter)define.PID, define.ManufacturerID, define.DeviceModelID, define.SoftwareVersionID), define);
    }
    private static string getDisplayName(RDMParameterDescription parameterDescription)
    {
        if (parameterDescription.Description.Contains(' ') && parameterDescription.Description.Contains('='))
        {
            string[] parts = parameterDescription.Description.Split(' ');
            string result = string.Join(' ', parts.TakeWhile(p => !p.Contains('=')));
            return result;
        }
        return null;
    }
    private static LabeledIntegerType[] getLabeledIntegerTypes(RDMParameterDescription parameterDescription)
    {
        List<LabeledIntegerType> labeledIntegerTypes = new List<LabeledIntegerType>();
        if (parameterDescription.Description.Contains(' ') && parameterDescription.Description.Contains('='))
        {
            var parts = parameterDescription.Description.Split(' ').Where(p => p.Contains('='));
            foreach (var part in parts)
            {
                string[] labelAndValue = part.Split('=');
                int value = 0;
                if (labelAndValue.Length == 2)
                {
                    if (int.TryParse(labelAndValue[0], out value))
                        labeledIntegerTypes.Add(new LabeledIntegerType(labelAndValue[1], value));
                    else if (int.TryParse(labelAndValue[1], out value))
                        labeledIntegerTypes.Add(new LabeledIntegerType(labelAndValue[0], value));
                }
            }
        }
        if (labeledIntegerTypes.Count == 0)
            return null;
        return labeledIntegerTypes.ToArray();
    }
    /// <summary>
    /// The completely loaded defines, waits (blocking) if they are still loading.
    /// </summary>
    private static ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>> waitForDefines()
    {
        var defines = metadataVersionDefinesBagDictionary;
        if (defines != null)
            return defines;
        // loadDefines runs on the thread pool without a synchronization context, so blocking here can not dead lock
        fillDefaultMetadataVersionList().GetAwaiter().GetResult();
        return metadataVersionDefinesBagDictionary;
    }
    private static MetadataJSONObjectDefine getDefine(ParameterBag parameter)
    {
        var defines = waitForDefines();
        var version = GetMetadataSchemaVersions().First();
        if (parameter.PID == ERDM_Parameter.QUEUED_MESSAGE)
            return defines[version].FirstOrDefault(d => d.PID == (ushort)parameter.PID);

        var possibleDefines = defines[version].FindAll(d => d.PID == (ushort)parameter.PID && d.ManufacturerID == parameter.ManufacturerID);
        if (possibleDefines.Count == 1)
            return possibleDefines[0];

        if (possibleDefines.Count > 1)
        {
            MetadataJSONObjectDefine define = possibleDefines.FirstOrDefault(d => d.DeviceModelID == null && d.SoftwareVersionID == null);
            if (parameter.DeviceModelID != null)
            {
                possibleDefines = possibleDefines.Where(d => d.DeviceModelID == parameter.DeviceModelID).ToList();
                if (possibleDefines.Count == 0)
                    return define;
                if (possibleDefines.Count == 1)
                    define = possibleDefines[0];
                else if (possibleDefines.Count > 1)
                {
                    define = possibleDefines.FirstOrDefault(d => d.SoftwareVersionID == null) ?? define;
                    if (parameter.SoftwareVersionID != null)
                    {
                        define = possibleDefines.FirstOrDefault(d => d.SoftwareVersionID == parameter.SoftwareVersionID) ?? define;

                        define ??= possibleDefines.MinBy(d => parameter.SoftwareVersionID - d.SoftwareVersionID);
                    }
                }
            }
            return possibleDefines.MaxBy(d => d.Version);
        }
        // no define: GetDefine throws a DefineNotFoundException for the ESTA range (e.g. a draft or reserved PID a device reports),
        // a manufacturer specific PID without define is null
        return null;
    }

    internal static IEnumerable<byte[]> ParsePayloadToData(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, DataTreeBranch payload)
    {
        define.GetCommand(commandType, out Command? _command);
        if (_command is not Command command)
            throw new InvalidOperationException();
        List<byte[]> result = new List<byte[]>();
        if (command.GetIsEmpty())
        {
            result.Add(new byte[0]);
            return result.AsReadOnly();
        }

        if (command.SingleField.HasValue && payload.Children.SingleOrDefault() is DataTree dataTree)
        {
            List<byte> singleData = new List<byte>();
            foreach (var nData in command.SingleField.Value.ParsePayloadToData(dataTree))
                addToPackets(result, singleData, nData);
            if (singleData.Count != 0)
                result.Add(singleData.ToArray());
            return result.AsReadOnly();
        }
        if (command.ListOfFields.Length != 0 && payload.Children is DataTree[] dataTreeArray)
        {
            if (dataTreeArray.Length != command.ListOfFields.Length)
                throw new IndexOutOfRangeException();

            List<byte> data = new List<byte>();
            for (int i = 0; i < command.ListOfFields.Length; i++)
            {
                var newData = command.ListOfFields[i].ParsePayloadToData(dataTreeArray[i]);
                foreach (var nData in newData)
                    addToPackets(result, data, nData);
                //data.AddRange(newData.SelectMany(en=>en).ToArray());
            }

            if (data.Count != 0)
                result.Add(data.ToArray());
            return result.AsReadOnly();
        }

        throw new ArithmeticException();
    }
    internal static DataTreeBranch ParseDataToPayload(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, byte[] data)
    {
        define.GetCommand(commandType, out Command? _command);
        if (_command is not Command command)
            throw new InvalidOperationException();
        try
        {
            if (command.GetIsEmpty())
                return DataTreeBranch.Empty;
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }

        if (command.SingleField.HasValue)
            return new DataTreeBranch(define, commandType, command.SingleField.Value.ParseDataToPayload(ref data));

        if (command.ListOfFields.Length != 0)
        {
            List<DataTree> tree = new List<DataTree>();
            for (int i = 0; i < command.ListOfFields.Length; i++)
                tree.Add(command.ListOfFields[i].ParseDataToPayload(ref data));
            return new DataTreeBranch(define, commandType, tree.ToArray());
        }

        throw new ArithmeticException();
    }
    private const int MAX_PDL = 231;
    /// <summary>
    /// Adds the data of one field to the packets (one packet = max. 231 bytes of Parameter Data).
    /// A field that fits into one packet is not split, it starts a new packet if the current one is too full.
    /// A field larger than one packet (e.g. a long URL, E1.37-5) is split over several packets, which are sent with ACK_OVERFLOW.
    /// </summary>
    private static void addToPackets(List<byte[]> packets, List<byte> current, byte[] fieldData)
    {
        if (current.Count + fieldData.Length > MAX_PDL && fieldData.Length <= MAX_PDL && current.Count != 0)
        {
            packets.Add(current.ToArray());
            current.Clear();
        }
        int offset = 0;
        while (current.Count + (fieldData.Length - offset) > MAX_PDL)
        {
            int take = MAX_PDL - current.Count;
            current.AddRange(fieldData.Skip(offset).Take(take));
            offset += take;
            packets.Add(current.ToArray());
            current.Clear();
        }
        current.AddRange(fieldData.Skip(offset));
    }
    internal static IEnumerable<byte[]> GetRequestMessageData(ParameterBag parameter, DataTreeBranch payloadData)
    {
        return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.GetRequest, payloadData);
    }
    internal static IEnumerable<byte[]> GetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
    {
        return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.GetResponse, payloadData);
    }
    internal static IEnumerable<byte[]> SetRequestMessageData(ParameterBag parameter, DataTreeBranch payloadData)
    {
        return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.SetRequest, payloadData);
    }
    internal static IEnumerable<byte[]> SetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
    {
        return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.SetResponse, payloadData);
    }

    /// <summary>
    /// Built once and published when it is complete (the first parse may come from many threads at the same time,
    /// a list that is filled while others read it leads to an empty result or "Collection was modified").
    /// </summary>
    private static readonly Lazy<IReadOnlyCollection<Type>> definedDataTreeObjects = new Lazy<IReadOnlyCollection<Type>>(
        () => Tools.FindClassesWithAttribute<DataTreeObjectAttribute>().ToList().AsReadOnly(),
        LazyThreadSafetyMode.ExecutionAndPublication);
    public static IReadOnlyCollection<Type> DefinedDataTreeObjects => definedDataTreeObjects.Value;

    public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType)
    {
        return GetDefinedDataTreeObjectType((EManufacturer)define.ManufacturerID, (ERDM_Parameter)define.PID, commandType);
    }
    public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, ERDM_Command command)
    {
        Command.ECommandDublicate commandType = Tools.ConvertCommandDublicateToCommand(command);
        return GetDefinedDataTreeObjectType((EManufacturer)define.ManufacturerID, (ERDM_Parameter)define.PID, commandType);
    }
    public static Type GetDefinedDataTreeObjectType(EManufacturer manufacturer, ERDM_Parameter parameter, ERDM_Command command)
    {
        Command.ECommandDublicate commandType = Tools.ConvertCommandDublicateToCommand(command);
        return GetDefinedDataTreeObjectType(manufacturer, parameter, commandType);
    }
    public static Type GetDefinedDataTreeObjectType(EManufacturer manufacturer, ERDM_Parameter parameter, Command.ECommandDublicate commandType)
    {
        bool checkManufacturer(EManufacturer attrManu)
        {
            if (attrManu == EManufacturer.ESTA)
                return true;

            return attrManu == manufacturer;
        }
        return DefinedDataTreeObjects.Where(t =>
        {
            if (t.GetCustomAttributes<DataTreeObjectAttribute>().Any(attribute => attribute.Parameter == parameter && attribute.Command == commandType && checkManufacturer(attribute.Manufacturer)))
                return true;
            return false;
        }).FirstOrDefault();
    }

    public static async Task AwaitInitialize()
    {
        await fillDefaultMetadataVersionList();
    }
}
