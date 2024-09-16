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

        [JsonConstructor]
        public MetadataJSONObjectDefine(string name, ushort manufacturerID, ushort pID, ushort version, SubdevicesForRequests[] getRequestSubdeviceRange, SubdevicesForResponses[] getResponseSubdeviceRange, SubdevicesForRequests[] setReequestsSubdeviceRange, SubdevicesForResponses[] setResponseSubdeviceRange)
        {
            Name = name;
            ManufacturerID = manufacturerID;
            PID = pID;
            Version = version;
            GetRequestSubdeviceRange = getRequestSubdeviceRange;
            GetResponseSubdeviceRange = getResponseSubdeviceRange;
            SetReequestsSubdeviceRange = setReequestsSubdeviceRange;
            SetResponseSubdeviceRange = setResponseSubdeviceRange;
        }

        public override string ToString()
        {
            return $"{ManufacturerID:X4} {PID:X4} {Name}";
        }
    }
}
