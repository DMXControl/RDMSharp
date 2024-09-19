using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata
{
    public class MetadataJSONObjectDefine
    {
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
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            ManufacturerID = manufacturerID;
            PID = pID;
            Version = version;

            GetRequestSubdeviceRange = getRequestSubdeviceRange;
            GetResponseSubdeviceRange = getResponseSubdeviceRange;
            SetRequestsSubdeviceRange = setRequestsSubdeviceRange;
            SetResponseSubdeviceRange = setResponseSubdeviceRange;

            //GetRequest = getRequest;
            //GetResponse = getResponse;
            //SetRequest = setRequest;
            //SetResponse = setResponse;

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
                    var result= setReferenceObjects(command.SingleField.Value);
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
                        case Command.ECommandDublicte.GetRequest:
                            reference = new ReferenceType(reference.URI, getRequest?.ListOfFields[reference.Pointer].ObjectType);
                            break;
                        case Command.ECommandDublicte.GetResponse:
                            reference = new ReferenceType(reference.URI, getResponse?.ListOfFields[reference.Pointer].ObjectType);
                            break;
                        case Command.ECommandDublicte.SetRequest:
                            reference = new ReferenceType(reference.URI, setRequest?.ListOfFields[reference.Pointer].ObjectType);
                            break;
                        case Command.ECommandDublicte.SetResponse:
                            reference = new ReferenceType(reference.URI, setResponse?.ListOfFields[reference.Pointer].ObjectType);
                            break;
                    }
                    return new OneOfTypes(reference);
                }
            }
        }

        public void GetCommand(Command.ECommandDublicte eCommand,out Command? command)
        {
            command = null;
            switch (eCommand)
            {
                case Command.ECommandDublicte.GetRequest:
                    command = GetRequest.Value;
                    break;
                case Command.ECommandDublicte.GetResponse:
                    command = GetResponse.Value;
                    break;
                case Command.ECommandDublicte.SetRequest:
                    command = SetRequest.Value;
                    break;
                case Command.ECommandDublicte.SetResponse:
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
