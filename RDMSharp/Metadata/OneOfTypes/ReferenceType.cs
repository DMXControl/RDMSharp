using RDMSharp.Metadata.JSON;
using System;
using System.Data;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct ReferenceType
    {
        [JsonPropertyName("$ref")]
        public readonly string URI { get; }
        public readonly Command.ECommandDublicte Command { get; }
        public readonly ushort Pointer { get; }

        [JsonConstructor]
        public ReferenceType(string uri)
        {
            URI = uri;
            // Entferne das '#' und zerlege den Rest in Segmente
            if (uri.StartsWith("#"))
            {
                string fragment = uri.Substring(1); // Entfernt das '#'

                // Zerlege den Pfad in einzelne Teile
                string[] segments = fragment.Split('/', StringSplitOptions.RemoveEmptyEntries);

                // Ausgabe der einzelnen Teile
                switch (segments[0])
                {
                    case "get_request":
                        Command = JSON.Command.ECommandDublicte.GetRequest;
                        break;
                    case "get_response":
                        Command = JSON.Command.ECommandDublicte.GetResponse;
                        break;
                    case "set_request":
                        Command = JSON.Command.ECommandDublicte.SetRequest;
                        break;
                    case "set_response":
                        Command = JSON.Command.ECommandDublicte.SetResponse;
                        break;
                }
                Pointer = ushort.Parse(segments[1]);
            }
        }
        public override string ToString()
        {
            return URI;
        }
    }
}
