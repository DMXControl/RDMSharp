using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata
{
    public readonly struct MetadataVersion
    {
        public readonly string Version;
        public readonly string Path;
        public readonly string Name;
        public readonly bool IsSchema;
        public readonly Assembly Assembly;
        public MetadataVersion(string path, Assembly assembly) : this(getVersion(path), getName(path), getIsSchema(path), path, assembly)
        {
        }
        public MetadataVersion(string version, string name, bool isSchema, string path, Assembly assembly)
        {
            Version = version;
            Path = path;
            Name = name;
            IsSchema = isSchema;
            Assembly = assembly;
        }
        internal static string getVersion(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.ToLower().EndsWith(".json"))
                throw new ArgumentException($"The given Paths should end with .json ({nameof(path)})");

            string pattern = @"_(\d+)\._(\d+)\._(\d+)";
            var match = Regex.Match(path, pattern);

            if (match.Success)
            {
                return match.Value.Replace("_", "");
            }
            else
                throw new FormatException($"Can't extract Version from Path: {path}");
        }
        internal static bool getIsSchema(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.ToLower().EndsWith(".json"))
                throw new ArgumentException($"The given Paths should end with .json ({nameof(path)})");

            return path.ToLower().EndsWith("schema.json");
        }
        internal static string getName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            string pattern = @"[^\.]+\.[json]+$";
            var match = Regex.Match(path, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            else
                throw new FormatException($"The given Paths should end with .json ({nameof(path)})");
        }
        public override string ToString()
        {
            return $"{Name} [{Version}]";
        }
    }
}
