using RDMSharp.Metadata.JSON.Converter;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OneOf = RDMSharp.Metadata.JSON.OneOfTypes.OneOfTypes;

namespace RDMSharp.Metadata.JSON
{
#pragma warning disable CS8632
    [JsonConverter(typeof(CommandConverter))]
    public readonly struct Command
    {
        [JsonConverter(typeof(CustomEnumConverter<ECommandDublicate>))]
        public enum ECommandDublicate
        {
            [JsonPropertyName("get_request")]
            GetRequest,
            [JsonPropertyName("get_response")]
            GetResponse,
            [JsonPropertyName("set_request")]
            SetRequest,
            [JsonPropertyName("set_response")]
            SetResponse,
            [JsonPropertyName("different_pid")]
            DifferentDid
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly ECommandDublicate? EnumValue { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly OneOf? SingleField { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly OneOf[]? ListOfFields { get; }

        public bool GetIsEmpty()
        {
            if (EnumValue != null)
                return false;
            if (SingleField != null)
                return false;
            if (ListOfFields != null)
                return ListOfFields.Length == 0;

            return true;
        }
        public Command(ECommandDublicate enumValue)
        {
            EnumValue = enumValue;
        }
        public Command(OneOf singleField)
        {
            SingleField = singleField;
        }
        public Command(OneOf[] listOfFields)
        {
            ListOfFields = listOfFields;
        }
        public PDL GetDataLength()
        {
            if (GetIsEmpty())
                return new PDL();
            if (SingleField.HasValue)
                return SingleField.Value.GetDataLength();
            if (ListOfFields != null)
                return new PDL(ListOfFields.Select(f => f.GetDataLength()).ToArray());

            throw new NotSupportedException();
        }

        public CommonPropertiesForNamed[] GetRequiredProperties()
        {
            if (SingleField.HasValue)
                return new CommonPropertiesForNamed[] { SingleField.Value.ObjectType };
            if (ListOfFields != null)
            {
                List<CommonPropertiesForNamed> names = new();
                foreach (var field in ListOfFields)
                    names.Add(field.ObjectType);
                return names.ToArray();
            }
            throw new NotImplementedException();
        }
        public bool TryGetLabeledIntegerTypes(out LabeledIntegerType[] labeledIntegerTypes)
        {
            labeledIntegerTypes = null;
            if (EnumValue.HasValue)
                return false;
            if (SingleField.HasValue)
                return SingleField.Value.TryGetLabeledIntegerTypes(out labeledIntegerTypes);
            if (ListOfFields != null)
                foreach (var field in ListOfFields)
                    if (field.TryGetLabeledIntegerTypes(out labeledIntegerTypes))
                        return true;
            return false;
        }
        public override string ToString()
        {
            if (EnumValue.HasValue)
                return EnumValue.Value.ToString();
            if (SingleField.HasValue)
                return SingleField.Value.ToString();
            if (ListOfFields != null)
                return $"[ {string.Join("; ", ListOfFields.Select(f => f.ToString()))} ]";
            return base.ToString();
        }
    }
}
#pragma warning restore CS8632