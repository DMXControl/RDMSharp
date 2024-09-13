using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataSchemaVersion
    {
        public readonly string Version;
        public readonly string Schema;
        public readonly string Path;
        public MetadataSchemaVersion(string path) : this(getVersion(path), getSchema(path), path)
        {
        }
        public MetadataSchemaVersion(string version, string schema, string path)
        {
            Version = version;
            Schema = schema;
            Path = path;
        }
        private static string getVersion(string path)
        {
            string pattern = @"_(\d+)\._(\d+)\._(\d+)";
            var match = Regex.Match(path, pattern);

            if (match.Success)
            {
                return match.Value.Replace("_", "");
            }
            else
                throw new Exception($"Can't extract Version from Path: {path}");
        }
        private static string getSchema(string path)
        {
            var assembly = typeof(MetadataFactory).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(path);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
