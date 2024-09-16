﻿using System.IO;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataBag
    {
        public readonly string Version;
        public readonly string Path;
        public readonly string Name;
        public readonly string Content;
        public readonly bool IsSchema;
        public MetadataBag(MetadataVersion metadataVersion) : this(metadataVersion.Version,metadataVersion.Name,metadataVersion.IsSchema, getContent(metadataVersion.Path), metadataVersion.Path)
        {
        }
        public MetadataBag(string version, string name, bool isSchema, string content, string path)
        {
            Version = version;
            Name= name;
            IsSchema = isSchema;
            Path = path;
            Content = content;
        }
        private static string getContent(string path)
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
