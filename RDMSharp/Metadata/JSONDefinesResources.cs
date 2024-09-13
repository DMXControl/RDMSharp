namespace RDMSharp
{
    public static class JSONDefinesResources
    {
        public static string[] GetResources()
        {
            var assembly = typeof(JSONDefinesResources).Assembly;
            return assembly.GetManifestResourceNames();
        }
    }
}
