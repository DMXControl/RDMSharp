using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata
{
    public class MetadataJSONObjectDefine
    {
#pragma warning disable CS8632
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("displayName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DisplayName { get; }

        [JsonPropertyName("notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Notes { get; }
        [JsonPropertyName("manufacturer_id")]
        public ushort ManufacturerID { get; }

        [JsonPropertyName("device_model_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ushort? DeviceModelID { get; }

        [JsonPropertyName("software_version_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? SoftwareVersionID { get; }

        [JsonPropertyName("pid")]
        public ushort PID { get; }

        [JsonPropertyName("version")]
        public ushort Version { get; }

        [JsonPropertyName("get_request_subdevice_range")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(11)]
        public SubdevicesForRequests[]? GetRequestSubdeviceRange { get; }
        [JsonPropertyName("get_response_subdevice_range")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(13)]
        public SubdevicesForResponses[]? GetResponseSubdeviceRange { get; }
        [JsonPropertyName("set_request_subdevice_range")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(15)]
        public SubdevicesForRequests[]? SetRequestsSubdeviceRange { get; }
        [JsonPropertyName("set_response_subdevice_range")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(17)]
        public SubdevicesForResponses[]? SetResponseSubdeviceRange { get; }

        [JsonPropertyName("get_request")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(12)]
        public Command? GetRequest { get; }
        [JsonPropertyName("get_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(14)]
        public Command? GetResponse { get; }
        [JsonPropertyName("set_request")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(16)]
        public Command? SetRequest { get; }
        [JsonPropertyName("set_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(18)]
        public Command? SetResponse { get; }

        [JsonConstructor]
        public MetadataJSONObjectDefine(
            string name,
            string? displayName,
            string? notes,
            ushort manufacturerID,
            ushort? deviceModelID,
            uint? softwareVersionID,
            ushort pID,
            ushort version,
            SubdevicesForRequests[]? getRequestSubdeviceRange,
            SubdevicesForResponses[]? getResponseSubdeviceRange,
            SubdevicesForRequests[]? setRequestsSubdeviceRange,
            SubdevicesForResponses[]? setResponseSubdeviceRange,
            Command? getRequest,
            Command? getResponse,
            Command? setRequest,
            Command? setResponse)
        {
            if (getRequest.HasValue ^ getResponse.HasValue)
                throw new JsonException($"Both {nameof(getRequest)} & {nameof(getResponse)} and have to be defined in {name}");

            if (setRequest.HasValue ^ setResponse.HasValue)
                throw new JsonException($"Both {nameof(setRequest)} & {nameof(setResponse)} and have to be defined in {name}");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            ManufacturerID = manufacturerID;
            DeviceModelID = deviceModelID;
            SoftwareVersionID = softwareVersionID;
            PID = pID;
            Version = version;

            GetRequestSubdeviceRange = getRequestSubdeviceRange;
            GetResponseSubdeviceRange = getResponseSubdeviceRange;
            SetRequestsSubdeviceRange = setRequestsSubdeviceRange;
            SetResponseSubdeviceRange = setResponseSubdeviceRange;

            GetRequest = getRequest;
            GetResponse = getResponse;
            SetRequest = setRequest;
            SetResponse = setResponse;

            if (getRequest.HasValue)
                GetRequest = setReferenceObjects(getRequest.Value);
            if (getResponse.HasValue)
                GetResponse = setReferenceObjects(getResponse.Value);
            if (setRequest.HasValue)
                SetRequest = setReferenceObjects(setRequest.Value);
            if (setResponse.HasValue)
                SetResponse = setReferenceObjects(setResponse.Value);


            Command setReferenceObjects(Command command)
            {
                if (command.SingleField.HasValue)
                {
                    var result = setReferenceObjects(command.SingleField.Value);
                    if (!result.Equals(command.SingleField.Value))
                        return new Command(result);
                }

                if (command.ListOfFields != null)
                {
                    List<OneOfTypes> listOfFields = new List<OneOfTypes>();
                    foreach (OneOfTypes oneOf in command.ListOfFields)
                    {
                        var result = setReferenceObjects(oneOf);
                        if (!result.Equals(oneOf))
                            listOfFields.Add(result);
                        else
                            listOfFields.Add(oneOf);
                    }
                    if (!listOfFields.SequenceEqual(command.ListOfFields))
                        return new Command(listOfFields.ToArray());
                }

                return command;

                OneOfTypes setReferenceObjects(OneOfTypes oneOf)
                {
                    if (!oneOf.ReferenceType.HasValue)
                        return oneOf;

                    ReferenceType reference = oneOf.ReferenceType.Value;

                    switch (reference.Command)
                    {
                        case Command.ECommandDublicate.GetRequest:
                            if (!getRequest.HasValue)
                                throw new JsonException($"The Referenced Command ({reference.Command.ToString()})is not defined");
                            reference = new ReferenceType(reference.URI, getRequest.Value.ListOfFields[reference.Pointer].ObjectType);
                            break;

                        case Command.ECommandDublicate.GetResponse:
                            if (!getResponse.HasValue)
                                throw new JsonException($"The Referenced Command ({reference.Command.ToString()})is not defined");
                            reference = new ReferenceType(reference.URI, getResponse.Value.ListOfFields[reference.Pointer].ObjectType);
                            break;

                        case Command.ECommandDublicate.SetRequest:
                            if (!setRequest.HasValue)
                                throw new JsonException($"The Referenced Command ({reference.Command.ToString()})is not defined");
                            reference = new ReferenceType(reference.URI, setRequest.Value.ListOfFields[reference.Pointer].ObjectType);
                            break;

                        case Command.ECommandDublicate.SetResponse:
                            if (!setResponse.HasValue)
                                throw new JsonException($"The Referenced Command ({reference.Command.ToString()})is not defined");
                            reference = new ReferenceType(reference.URI, setResponse.Value.ListOfFields[reference.Pointer].ObjectType);
                            break;
                    }
                    return new OneOfTypes(reference);
                }
            }
        }

        public void GetCommand(Command.ECommandDublicate eCommand, out Command? command)
        {
            command = null;
            switch (eCommand)
            {
                case Command.ECommandDublicate.GetRequest when GetRequest is not null:
                    command = GetRequest.Value;
                    break;
                case Command.ECommandDublicate.GetResponse when GetResponse is not null:
                    command = GetResponse.Value;
                    break;
                case Command.ECommandDublicate.SetRequest when SetRequest is not null:
                    command = SetRequest.Value;
                    break;
                case Command.ECommandDublicate.SetResponse when SetResponse is not null:
                    command = SetResponse.Value;
                    break;
            }
            if (command.HasValue)
                if (command.Value.EnumValue.HasValue)
                    GetCommand(command.Value.EnumValue.Value, out command);

        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Notes))
                return $"{ManufacturerID:X4} {PID:X4} {Name} ({Notes})";

            return $"{ManufacturerID:X4} {PID:X4} {Name}";
        }
    }
}

#pragma warning restore CS8632