using RDMSharp.Metadata;
using System.Collections.Concurrent;

namespace RDMSharpTests
{
    public class MetadataJSONObjectDefineTestSubject
    {
        public static readonly object[] TestSubjects = getTestSubjects();
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
            return instances.ToArray();
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