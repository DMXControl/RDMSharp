using RDMSharp.Metadata;
using System.Collections.Concurrent;
using System.Reflection;

namespace RDMSharpTests
{
    public class MetadataJSONObjectDefineTestSubject
    {
        public static readonly object[] TestSubjects = getTestSubjects();
        internal static string[] GetResources()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceNames();
        }
        private static object[] getTestSubjects()
        {
            List<MetadataJSONObjectDefineTestSubject> instances = new List<MetadataJSONObjectDefineTestSubject>();
            List<MetadataVersion> metadataVersionList= new List<MetadataVersion>();
            metadataVersionList.AddRange(MetadataFactory.GetResources().Select(r => new MetadataVersion(r)));
            var schemaList = MetadataFactory.GetMetadataSchemaVersions();
            ConcurrentDictionary<string, MetadataBag> versionSchemas = new ConcurrentDictionary<string, MetadataBag>();

            foreach (var mv in metadataVersionList.Where(_mv => !_mv.IsSchema))
            {
                var _schema = schemaList.First(s => s.Version.Equals(mv.Version));
                if (!versionSchemas.TryGetValue(_schema.Version, out MetadataBag schema))
                {
                    schema = new MetadataBag(_schema);
                    versionSchemas.TryAdd(_schema.Version, schema);
                }
                instances.Add(new MetadataJSONObjectDefineTestSubject(schema, new MetadataBag(mv)));
            }
            foreach (var mv in GetResources().Select(r => new MetadataVersion(r)))
            {
                var _schema = schemaList.First(s => s.Version.Equals(mv.Version));
                if (!versionSchemas.TryGetValue(_schema.Version, out MetadataBag schema))
                {
                    schema = new MetadataBag(_schema);
                    versionSchemas.TryAdd(_schema.Version, schema);
                }
                instances.Add(new MetadataJSONObjectDefineTestSubject(schema, new MetadataBag(mv.Version,mv.Name,mv.IsSchema,getContent(mv.Path),mv.Path)));
            }
            return instances.ToArray();
        }
        private static string getContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            var assembly = typeof(MetadataJSONObjectDefineTestSubject).Assembly;
            using Stream stream = assembly.GetManifestResourceStream(path);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public readonly MetadataBag Schema;
        public readonly MetadataBag Define;

        public override string ToString() => Define.Name;

        public MetadataJSONObjectDefineTestSubject(MetadataBag schema, MetadataBag define)
        {
            Schema = schema;
            Define = define;
        }
    }
}