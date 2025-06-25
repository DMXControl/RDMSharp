using Json.Schema;
using Microsoft.Extensions.Logging;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata
{
    public static class MetadataFactory
    {
        private static readonly ILogger Logger = Logging.CreateLogger(typeof(MetadataFactory));
        private const string SCHEMA_FILE_NAME = "schema.json";
        private const string JSON_ENDING = ".json";
        private static ConcurrentDictionary<string, MetadataVersion> metadataVersionList;
        private static ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>> metadataVersionDefinesBagDictionary;
        private static ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine> parameterBagDefineCache;
        private static List<Assembly> resourceProvider = new List<Assembly>() { typeof(MetadataFactory).Assembly };

        public static void AddResourceProvider(Assembly assembly)
        {
            resourceProvider.Add(assembly);
        }
        public static IReadOnlyDictionary<string, MetadataVersion> MetadataVersionList
        {
            get
            {
                if (metadataVersionList == null)
                {
                    metadataVersionList = new ConcurrentDictionary<string, MetadataVersion>();
                    foreach (Assembly assembly in resourceProvider)
                    {
                        try
                        {
                            var metaDataVersions = GetResources(assembly).Select(r => new MetadataVersion(r, assembly));
                            foreach (var mv in metaDataVersions)
                                metadataVersionList.TryAdd(mv.Path, mv);
                        }
                        catch (Exception e)
                        {
                            Logger?.LogError(e);
                        }
                    }
                }
                return metadataVersionList.AsReadOnly();
            }
        }
        public static IReadOnlyCollection<string> GetResources(Assembly assembly)
        {
            return assembly.GetManifestResourceNames().Where(p => p.EndsWith(JSON_ENDING)).ToList().AsReadOnly();
        }
        private static void fillDefaultMetadataVersionList()
        {
            if (metadataVersionDefinesBagDictionary != null)
                return;

            metadataVersionDefinesBagDictionary = new ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>>();
            foreach (Assembly assembly in resourceProvider)
            {
                try
                {
                    var metaDataVersions = GetResources(assembly).Select(r => new MetadataVersion(r, assembly));
                    foreach (var mv in metaDataVersions)
                        metadataVersionList.TryAdd(mv.Path, mv);
                }
                catch (Exception e)
                {
                    Logger?.LogError(e);
                }
            }

            var schemaList = GetMetadataSchemaVersions();
            ConcurrentDictionary<string, JsonSchema> versionSchemas = new ConcurrentDictionary<string, JsonSchema>();

            var nonSchemaVersions = metadataVersionList.Values.Where(_mv => !_mv.IsSchema).ToList();

            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount, // Optional: Set the maximum degree of parallelism
            };
            Parallel.ForEach(nonSchemaVersions, parallelOptions, mv =>
            {
                try
                {
                    var schema = schemaList.First(s => s.Version.Equals(mv.Version));
                    if (!versionSchemas.TryGetValue(schema.Version, out JsonSchema jsonSchema))
                    {
                        jsonSchema = JsonSchema.FromText(new MetadataBag(schema).Content);
                        versionSchemas.TryAdd(schema.Version, jsonSchema);
                    }
                    MetadataBag metadataBag = new MetadataBag(mv);
                    var result = jsonSchema.Evaluate(JsonNode.Parse(metadataBag.Content));
                    if (result.IsValid)
                    {
                        MetadataJSONObjectDefine jsonDefine = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(metadataBag.Content);
                        metadataVersionDefinesBagDictionary.AddOrUpdate(schema,
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
                        throw new Exception($"Schema Invalid for {mv.Name}");
                }
                catch (Exception e)
                {
                    Logger?.LogError($"Exception while Deserialize {mv.Name}", e);
                }
            });
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
                fillDefaultMetadataVersionList();
            }
        }
        public static MetadataJSONObjectDefine GetDefine(ParameterBag parameter)
        {
            try
            {
                if (parameterBagDefineCache == null)
                    parameterBagDefineCache = new ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine>();

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
        public static void AddDefineFromParameterDescription(UID uid,SubDevice subDevice, RDMDeviceInfo deviceInfo, RDMParameterDescription parameterDescription)
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

            if (parameterDescription.CommandClass.HasFlag(ERDM_CommandClass.GET))
            {
                getCommandRequest = new Command();
                OneOfTypes? oneOfType = null;
                LabeledIntegerType[] labeledIntegerTypes = getLabeledIntegerTypes(parameterDescription);
                switch (parameterDescription.DataType)
                {
                    case ERDM_DataType.ASCII:
                        oneOfType = new OneOfTypes(new StringType(name, displayName, null, null, "string", null, null, 0, parameterDescription.PDLSize, null, null, true));
                        break;
                    case ERDM_DataType.UNSIGNED_BYTE:
                        oneOfType = new OneOfTypes(new IntegerType<byte>(name, displayName, null, null, EIntegerType.UInt8, labeledIntegerTypes, labeledIntegerTypes != null, new Range<byte>[] { new Range<byte>((byte)parameterDescription.MinValidValue, (byte)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1),1));
                        break;
                    case ERDM_DataType.SIGNED_BYTE:
                        oneOfType = new OneOfTypes(new IntegerType<sbyte>(name, displayName, null, null, EIntegerType.Int8, labeledIntegerTypes, labeledIntegerTypes != null, new Range<sbyte>[] { new Range<sbyte>((sbyte)parameterDescription.MinValidValue, (sbyte)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;

                    case ERDM_DataType.UNSIGNED_WORD:
                        oneOfType = new OneOfTypes(new IntegerType<ushort>(name, displayName, null, null, EIntegerType.UInt16, labeledIntegerTypes, labeledIntegerTypes != null, new Range<ushort>[] { new Range<ushort>((ushort)parameterDescription.MinValidValue, (ushort)parameterDescription.MaxValidValue) }, parameterDescription.Unit, (int)Tools.GetNormalizedValue(parameterDescription.Prefix, 1), 1));
                        break;
                    case ERDM_DataType.SIGNED_WORD:
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
            if(parameterDescription.Description.Contains(' ') && parameterDescription.Description.Contains('='))
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
        private static MetadataJSONObjectDefine getDefine(ParameterBag parameter)
        {
            var version = GetMetadataSchemaVersions().First();

            fillDefaultMetadataVersionList();
            var possibleDefines = metadataVersionDefinesBagDictionary[version].FindAll(d => d.PID == (ushort)parameter.PID && d.ManufacturerID == parameter.ManufacturerID);
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

                            if (define == null)
                                define = possibleDefines.MinBy(d => parameter.SoftwareVersionID - d.SoftwareVersionID);
                        }
                    }
                }
                return possibleDefines.MaxBy(d => d.Version);
            }
            if (parameter.ManufacturerID == 0)
                throw new InvalidOperationException($"{parameter.ManufacturerID} of 0 should lead to exact 1 Define");



            if ((ushort)parameter.PID < 0x8000 || (ushort)parameter.PID > 0xFFDF)
                throw new DefineNotFoundException($"{parameter}");
            return null;
        }

        internal static byte[] ParsePayloadToData(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, DataTreeBranch payload)
        {
            define.GetCommand(commandType, out Command? _command);
            if (_command is not Command command)
                throw new InvalidOperationException();

            if (command.GetIsEmpty())
                return new byte[0];

            if (command.SingleField.HasValue && payload.Children.SingleOrDefault() is DataTree dataTree)
                return command.SingleField.Value.ParsePayloadToData(dataTree);

            if (command.ListOfFields.Length != 0 && payload.Children is DataTree[] dataTreeArray)
            {
                if (dataTreeArray.Length != command.ListOfFields.Length)
                    throw new IndexOutOfRangeException();
                List<byte> data = new List<byte>();
                for (int i = 0; i < command.ListOfFields.Length; i++)
                    data.AddRange(command.ListOfFields[i].ParsePayloadToData(dataTreeArray[i]));
                return data.ToArray();
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
        internal static byte[] GetRequestMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.GetRequest, payloadData);
        }
        internal static byte[] GetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.GetResponse, payloadData);
        }
        internal static byte[] SetRequestMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.SetRequest, payloadData);
        }
        internal static byte[] SetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicate.SetResponse, payloadData);
        }

        private static List<Type> definedDataTreeObjects;
        public static IReadOnlyCollection<Type> DefinedDataTreeObjects
        {
            get
            {
                fillDefinedDataTreeObjects();
                return definedDataTreeObjects;
            }
        }

        private static void fillDefinedDataTreeObjects()
        {
            if (definedDataTreeObjects != null)
                return;

            definedDataTreeObjects = new List<Type>();

            definedDataTreeObjects.AddRange(Tools.FindClassesWithAttribute<DataTreeObjectAttribute>());
        }

        public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType)
        {
            return GetDefinedDataTreeObjectType((ERDM_Parameter)define.PID, commandType);
        }
        public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, ERDM_Command command)
        {
            Command.ECommandDublicate commandType = Tools.ConvertCommandDublicateToCommand(command);
            return GetDefinedDataTreeObjectType((ERDM_Parameter)define.PID, commandType);
        }
        public static Type GetDefinedDataTreeObjectType(ERDM_Parameter parameter, ERDM_Command command)
        {
            Command.ECommandDublicate commandType = Tools.ConvertCommandDublicateToCommand(command);
            return GetDefinedDataTreeObjectType(parameter, commandType);
        }
        public static Type GetDefinedDataTreeObjectType(ERDM_Parameter parameter, Command.ECommandDublicate commandType)
        {
            return DefinedDataTreeObjects.Where(t =>
            {
                if (t.GetCustomAttributes<DataTreeObjectAttribute>().Any(attribute => attribute.Parameter == parameter && attribute.Command == commandType))
                    return true;
                return false;
            }).FirstOrDefault();
        }
    }
}
