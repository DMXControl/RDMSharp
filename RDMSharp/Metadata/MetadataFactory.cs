using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata
{
    public static class MetadataFactory
    {
        private const string SCHEMA_FILE_NAME = "schema.json";
        private const string JSON_ENDING = ".json";
        private static List<MetadataVersion> metadataVersionList;
        private static Dictionary<MetadataVersion,List<MetadataJSONObjectDefine>> metadataVersionDefinesBagDictionary;
        private static ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine> parameterBagDefineCache;

        public static IReadOnlyCollection<MetadataVersion> MetadataVersionList
        {
            get
            {
                if (metadataVersionList == null)
                {
                    metadataVersionList = new List<MetadataVersion>();
                    metadataVersionList.AddRange(GetResources().Select(r => new MetadataVersion(r)));
                }
                return metadataVersionList.AsReadOnly();
            }
        }
        public static string[] GetResources()
        {
            var assembly = typeof(MetadataFactory).Assembly;
            return assembly.GetManifestResourceNames();
        }
        private static void fillDefaultMetadataVersionList()
        {
            metadataVersionList.AddRange(GetResources().Select(r => new MetadataVersion(r)));

            if (metadataVersionDefinesBagDictionary == null)
                metadataVersionDefinesBagDictionary = new Dictionary<MetadataVersion, List<MetadataJSONObjectDefine>>();
            
            var schemaList = GetMetadataSchemaVersions();
            ConcurrentDictionary<string, JsonSchema> versionSchemas= new ConcurrentDictionary<string, JsonSchema>();

            foreach (var mv in metadataVersionList.Where(_mv => !_mv.IsSchema))
            {
                var schema = schemaList.First(s => s.Version.Equals(mv.Version));
                if(!versionSchemas.TryGetValue(schema.Version, out JsonSchema jsonSchema))
                {
                    jsonSchema= JsonSchema.FromText(new MetadataBag(schema).Content);
                    versionSchemas.TryAdd(schema.Version, jsonSchema);
                }
                MetadataBag metadataBag = new MetadataBag(mv);
                var result = jsonSchema.Evaluate(JsonNode.Parse(metadataBag.Content));
                if (result.IsValid)
                {
                    MetadataJSONObjectDefine jsonDefine = JsonSerializer.Deserialize<MetadataJSONObjectDefine>(metadataBag.Content);
                    if (!metadataVersionDefinesBagDictionary.ContainsKey(schema))
                        metadataVersionDefinesBagDictionary.Add(schema, new List<MetadataJSONObjectDefine>());

                    metadataVersionDefinesBagDictionary[schema].Add(jsonDefine);
                }
            }
        }
        
        public static IReadOnlyCollection<MetadataVersion> GetMetadataSchemaVersions()
        {
            return MetadataVersionList.Where(r => r.IsSchema).ToList().AsReadOnly();
        }
        public static IReadOnlyCollection<MetadataVersion> GetMetadataDefineVersions()
        {
            return MetadataVersionList.Where(r => !r.IsSchema).ToList().AsReadOnly();
        }
        internal static MetadataJSONObjectDefine GetDefine(ParameterBag parameter)
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

            }
            throw new DefineNotFoundException($"{parameter}");
        }
        private static MetadataJSONObjectDefine getDefine(ParameterBag parameter)
        {
            var version = GetMetadataSchemaVersions().First();
            if (metadataVersionDefinesBagDictionary == null)
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



            throw new DefineNotFoundException($"{parameter}");
        }

        internal static byte[] ParsePayloadToData(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType, DataTreeBranch payload)
        {
            define.GetCommand(commandType, out Command? _command);
            if (_command is not Command command)
                throw new InvalidOperationException();

            if (command.GetIsEmpty())
                return new byte[0];

            if (payload.Children.SingleOrDefault() is DataTree dataTree && command.SingleField.HasValue)
                return command.SingleField.Value.ParsePayloadToData(dataTree);

            if (payload.Children is DataTree[] dataTreeArray && command.ListOfFields.Length != 0)
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
        internal static DataTreeBranch ParseDataToPayload(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType, byte[] data)
        {
            define.GetCommand(commandType, out Command? _command);
            if (_command is not Command command)
                throw new InvalidOperationException();

            if (command.GetIsEmpty())
                return DataTreeBranch.Empty;

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
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicte.GetRequest, payloadData);
        }
        internal static byte[] GetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicte.GetResponse, payloadData);
        }
        internal static byte[] SetRequestMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicte.SetRequest, payloadData);
        }
        internal static byte[] SetResponseMessageData(ParameterBag parameter, DataTreeBranch payloadData)
        {
            return ParsePayloadToData(GetDefine(parameter), Command.ECommandDublicte.SetResponse, payloadData);
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
        
        public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, Command.ECommandDublicte commandType)
        {
            return GetDefinedDataTreeObjectType((ERDM_Parameter)define.PID, commandType);
        }
        public static Type GetDefinedDataTreeObjectType(MetadataJSONObjectDefine define, ERDM_Command command)
        {
            Command.ECommandDublicte commandType = Tools.ConvertCommandDublicteToCommand(command);
            return GetDefinedDataTreeObjectType((ERDM_Parameter)define.PID, commandType);        
        }
        public static Type GetDefinedDataTreeObjectType(ERDM_Parameter parameter, ERDM_Command command)
        {
            Command.ECommandDublicte commandType = Tools.ConvertCommandDublicteToCommand(command);
            return GetDefinedDataTreeObjectType(parameter, commandType);
        }
        public static Type GetDefinedDataTreeObjectType(ERDM_Parameter parameter, Command.ECommandDublicte commandType)
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
