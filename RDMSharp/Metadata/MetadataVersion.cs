using System;
using System.Text.RegularExpressions;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataVersion
    {
        public readonly string Version;
        public readonly string Path;
        public readonly string Name;
        public readonly bool IsSchema;
        public MetadataVersion(string path) : this(getVersion(path), getName(path), getIsSchema(path), path)
        {
        }
        public MetadataVersion(string version, string name, bool isSchema, string path)
        {
            Version = version;
            Path = path;
            Name = name;
            IsSchema = isSchema;
            string pattern = @"[^\.]+\.[json]+$";
            var match = Regex.Match(Path, pattern);

            if (match.Success)
            {
                Name = match.Value;
            }
            else
                throw new Exception($"Can't extract Name from Path: {path}");
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
        private static bool getIsSchema(string path)
        {
            return path.ToLower().EndsWith("schema.json");
        }
        private static string getName(string path)
        {
            string pattern = @"[^\.]+\.[json]+$";
            var match = Regex.Match(path, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            else
                throw new Exception($"Can't extract Name from Path: {path}");
        }
        public override string ToString()
        {
            return $"{Name} [{Version}]";
        }
    }
}
