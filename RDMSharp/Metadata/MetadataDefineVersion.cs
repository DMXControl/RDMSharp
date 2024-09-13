using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataDefineVersion
    {
        public readonly string Version;
        public readonly string Define;
        public readonly string Path;
        public readonly string Name;
        public MetadataDefineVersion(string path) : this(getVersion(path), getDefine(path), path)
        {
        }
        public MetadataDefineVersion(string version, string define, string path)
        {
            Version = version;
            Define = define;
            Path = path;
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
        private static string getDefine(string path)
        {
            var assembly = typeof(MetadataFactory).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(path);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        public override string ToString()
        {
            return $"{Name} [{Version}]";
        }
    }
}
