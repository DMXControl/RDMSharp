using RDMSharp.Metadata.JSON;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public readonly struct ReferenceType
    {
        [JsonPropertyName("$ref")]
        public readonly string URI { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public readonly Command.ECommandDublicte Command { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public readonly ushort Pointer { get; }

        [JsonConstructor]
        public ReferenceType(string uri)
        {
            URI = uri;
            if (uri.StartsWith("#"))
            {
                string fragment = uri.Substring(1);

                string[] segments = fragment.Split('/', StringSplitOptions.RemoveEmptyEntries);

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
