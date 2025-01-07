﻿using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;
using RDMSharp.RDM;
using OneOf = RDMSharp.Metadata.JSON.OneOfTypes.OneOfTypes;

namespace RDMSharp.Metadata.JSON
{
    [JsonConverter(typeof(CommandConverter))]
    public readonly struct Command
    {
        [JsonConverter(typeof(CustomEnumConverter<ECommandDublicte>))]
        public enum ECommandDublicte
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
        public readonly ECommandDublicte? EnumValue { get; }
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
        public Command(ECommandDublicte enumValue)
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
            if(GetIsEmpty())
                return new PDL();
            if (SingleField.HasValue)
                return SingleField.Value.GetDataLength();
            if (ListOfFields != null)
                return new PDL(ListOfFields.Select(f => f.GetDataLength()).ToArray());

            throw new System.NotSupportedException();
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
