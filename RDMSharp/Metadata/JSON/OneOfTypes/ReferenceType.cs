using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public readonly struct ReferenceType
    {
        [JsonPropertyName("$ref")]
        public readonly string URI { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public readonly Command.ECommandDublicate Command { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public readonly ushort Pointer { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
#pragma warning disable CS8632
        public readonly CommonPropertiesForNamed? ReferencedObject { get; }
#pragma warning restore CS8632

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
                        Command = JSON.Command.ECommandDublicate.GetRequest;
                        break;
                    case "get_response":
                        Command = JSON.Command.ECommandDublicate.GetResponse;
                        break;
                    case "set_request":
                        Command = JSON.Command.ECommandDublicate.SetRequest;
                        break;
                    case "set_response":
                        Command = JSON.Command.ECommandDublicate.SetResponse;
                        break;
                }
                Pointer = ushort.Parse(segments[1]);
            }
            else
                throw new ArgumentException($"{nameof(uri)} has to start with \'#\'");
        }
        public ReferenceType(string uri, CommonPropertiesForNamed referencedObject) : this(uri)
        {
            ReferencedObject = referencedObject;
        }
        public PDL GetDataLength()
        {
            return ReferencedObject?.GetDataLength() ?? new PDL();
        }
        public IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
        {
            return ReferencedObject.ParsePayloadToData(dataTree);
        }

        public DataTree ParseDataToPayload(ref byte[] data)
        {
            return ReferencedObject.ParseDataToPayload(ref data);
        }
        public override string ToString()
        {
            return URI;
        }
    }
}
