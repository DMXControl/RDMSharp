using System.Text.Json;
using System;
using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataJSONObjectDefine
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("manufacturer_id")]
        public readonly ushort ManufacturerID { get; }
        [JsonPropertyName("pid")]
        public readonly ushort PID { get; }
        [JsonPropertyName("version")]
        public readonly ushort Version { get; }
        [JsonPropertyName("get_request_subdevice_range")]
        public readonly SubdevicesForRequests[] GetRequestSubdeviceRange { get; }
        [JsonPropertyName("get_response_subdevice_range")]
        public readonly SubdevicesForResponses[] GetResponseSubdeviceRange { get; }
        [JsonPropertyName("set_request_subdevice_range")]
        public readonly SubdevicesForRequests[] SetReequestsSubdeviceRange { get; }
        [JsonPropertyName("set_response_subdevice_range")]
        public readonly SubdevicesForResponses[] SetResponseSubdeviceRange { get; }

        [JsonPropertyName("get_request")]
        public readonly Command? GetRequest { get; }
        [JsonPropertyName("get_response")]
        public readonly Command? GetResponse { get; }
        [JsonPropertyName("set_request")]
        public readonly Command? SetRequest { get; }
        [JsonPropertyName("set_response")]
        public readonly Command? SetResponse { get; }

        [JsonConstructor]
        public MetadataJSONObjectDefine(
            string name,
            ushort manufacturerID,
            ushort pID,
            ushort version,
            SubdevicesForRequests[] getRequestSubdeviceRange,
            SubdevicesForResponses[] getResponseSubdeviceRange,
            SubdevicesForRequests[] setReequestsSubdeviceRange,
            SubdevicesForResponses[] setResponseSubdeviceRange,
            Command? getRequest,
            Command? getResponse,
            Command? setRequest,
            Command? setResponse)
        {
            Name = name;
            ManufacturerID = manufacturerID;
            PID = pID;
            Version = version;

            GetRequestSubdeviceRange = getRequestSubdeviceRange;
            GetResponseSubdeviceRange = getResponseSubdeviceRange;
            SetReequestsSubdeviceRange = setReequestsSubdeviceRange;
            SetResponseSubdeviceRange = setResponseSubdeviceRange;

            GetRequest = getRequest;
            GetResponse = getResponse;
            SetRequest = setRequest;
            SetResponse = setResponse;
        }

        public override string ToString()
        {
            return $"{ManufacturerID:X4} {PID:X4} {Name}";
        }
    }
}
