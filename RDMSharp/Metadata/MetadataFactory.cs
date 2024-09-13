using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.Metadata
{
    public static class MetadataFactory
    {
        private const string SCHEMA_FILE_NAME = "schema.json";
        private const string JSON_ENDING = ".json";
        public static string[] GetResources()
        {
            var assembly = typeof(MetadataFactory).Assembly;
            return assembly.GetManifestResourceNames();
        }
        
        public static IList<MetadataSchemaVersion> GetMetadataSchemaVersions()
        {
            var list = GetResources().Where(r => r.EndsWith(SCHEMA_FILE_NAME));
            return list.Select(r=>new MetadataSchemaVersion(r)).ToList();
        }
        public static IList<MetadataDefineVersion> GetMetadataDefineVersions()
        {
            var list = GetResources().Where(r => r.EndsWith(JSON_ENDING) && !r.EndsWith(SCHEMA_FILE_NAME));
            return list.Select(r => new MetadataDefineVersion(r)).ToList();
        }
    }
}
