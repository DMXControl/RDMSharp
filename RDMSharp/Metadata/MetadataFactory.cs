using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;

namespace RDMSharp.Metadata
{
    public static class MetadataFactory
    {
        private const string SCHEMA_FILE_NAME = "schema.json";
        private const string JSON_ENDING = ".json";
        private static List<MetadataVersion> metadataVersionList;
        private static Dictionary<MetadataVersion,List<MetadataJSONObjectDefine>> metadataVersionDefinesBagDictionary;
        public static IReadOnlyCollection<MetadataVersion> MetadataVersionList
        {
            get
            {
                if (metadataVersionList == null)
                {
                    metadataVersionList = new List<MetadataVersion>();
                    fillDefaultMetadataVersionList();
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
            return;
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
                else
                {

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
    }
}
