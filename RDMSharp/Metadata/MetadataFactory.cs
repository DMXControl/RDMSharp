﻿using Json.Schema;
using Microsoft.Extensions.Logging;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata
{
    public static class MetadataFactory
    {
        private static readonly ILogger Logger = LoggingTools.CreateLogger(typeof(MetadataFactory));
        private const string SCHEMA_FILE_NAME = "schema.json";
        private const string JSON_ENDING = ".json";
        private static ConcurrentDictionary<string, MetadataVersion> metadataVersionList;
        private static ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>> metadataVersionDefinesBagDictionary;
        private static ConcurrentDictionary<ParameterBag, MetadataJSONObjectDefine> parameterBagDefineCache;

        public static IReadOnlyDictionary<string, MetadataVersion> MetadataVersionList
        {
            get
            {
                if (metadataVersionList == null)
                {
                    metadataVersionList = new ConcurrentDictionary<string, MetadataVersion>();
                    var metaDataVersions = GetResources().Select(r => new MetadataVersion(r));
                    foreach (var mv in metaDataVersions)
                        metadataVersionList.TryAdd(mv.Path, mv);
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
            var metaDataVersions = GetResources().Select(r => new MetadataVersion(r));
            foreach (var mv in metaDataVersions)
                metadataVersionList.TryAdd(mv.Path, mv);

            if (metadataVersionDefinesBagDictionary == null)
                metadataVersionDefinesBagDictionary = new ConcurrentDictionary<MetadataVersion, List<MetadataJSONObjectDefine>>();

            var schemaList = GetMetadataSchemaVersions();
            ConcurrentDictionary<string, JsonSchema> versionSchemas = new ConcurrentDictionary<string, JsonSchema>();

            foreach (var mv in metadataVersionList.Values.Where(_mv => !_mv.IsSchema))
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
                    if (!metadataVersionDefinesBagDictionary.ContainsKey(schema))
                        metadataVersionDefinesBagDictionary.TryAdd(schema, new List<MetadataJSONObjectDefine>());

                    metadataVersionDefinesBagDictionary[schema].Add(jsonDefine);
                }
            }
        }

        public static IReadOnlyCollection<MetadataVersion> GetMetadataSchemaVersions()
        {
            return MetadataVersionList.Values.Where(r => r.IsSchema).ToList().AsReadOnly();
        }
        public static IReadOnlyCollection<MetadataVersion> GetMetadataDefineVersions()
        {
            return MetadataVersionList.Values.Where(r => !r.IsSchema).ToList().AsReadOnly();
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
