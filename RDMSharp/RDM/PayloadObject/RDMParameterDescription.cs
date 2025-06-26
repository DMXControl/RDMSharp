using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.PARAMETER_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class RDMParameterDescription : AbstractRDMPayloadObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060")]
        public RDMParameterDescription(
            ushort parameterId = 0,
            byte pdlSize = 0,
            ERDM_DataType dataType = ERDM_DataType.NOT_DEFINED,
            ERDM_CommandClass commandClass = ERDM_CommandClass.GET,
            byte type = 0, //Obsolete
            ERDM_SensorUnit unit = ERDM_SensorUnit.NONE,
            ERDM_UnitPrefix prefix = ERDM_UnitPrefix.NONE,
            uint minValidValue = 0,
            uint maxValidValue = 0,
            uint defaultValue = 0,
            string description = "")
        {
            this.ParameterId = parameterId;
            this.PDLSize = pdlSize;
            this.DataType = dataType;
            this.CommandClass = commandClass;
#pragma warning disable CS0612
            this.Type = type;
#pragma warning restore CS0612
            this.Unit = unit;
            this.Prefix = prefix;
            this.MinValidValue = minValidValue;
            this.MaxValidValue = maxValidValue;
            this.DefaultValue = defaultValue;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }
        [DataTreeObjectConstructor]
        public RDMParameterDescription(
            [DataTreeObjectParameter("pid")] ushort parameterId,
            [DataTreeObjectParameter("pdl")] byte pdlSize,
            [DataTreeObjectParameter("data_type")] byte dataType,
            [DataTreeObjectParameter("command_class")] byte commandClass,
            [DataTreeObjectParameter("type")] byte type, //Obsolete
            [DataTreeObjectParameter("unit")] byte unit,
            [DataTreeObjectParameter("unit_prefix")] byte prefix,
            [DataTreeObjectParameter("min_valid_value")] uint minValidValue,
            [DataTreeObjectParameter("max_valid_value")] uint maxValidValue,
            [DataTreeObjectParameter("default_value")] uint defaultValue,
            [DataTreeObjectParameter("description")] string description)
            : this(parameterId, pdlSize, (ERDM_DataType)dataType, (ERDM_CommandClass)commandClass, type, (ERDM_SensorUnit)unit, (ERDM_UnitPrefix)prefix, minValidValue, maxValidValue, defaultValue, description)
        {
        }

        [DataTreeObjectProperty("pid", 0)]
        public ushort ParameterId { get; private set; }
        [DataTreeObjectProperty("pdl", 1)]
        public byte PDLSize { get; private set; }
        [DataTreeObjectProperty("data_type", 2)]
        public ERDM_DataType DataType { get; private set; }
        [DataTreeObjectProperty("command_class", 3)]
        public ERDM_CommandClass CommandClass { get; private set; }

        [DataTreeObjectProperty("type", 4), Obsolete]
        public byte Type { get; private set; } //Obsolete
        [DataTreeObjectProperty("unit", 5)]
        public ERDM_SensorUnit Unit { get; private set; }
        [DataTreeObjectProperty("unit_prefix", 6)]
        public ERDM_UnitPrefix Prefix { get; private set; }
        [DataTreeObjectProperty("min_valid_value", 7)]
        public uint MinValidValue { get; private set; }
        [DataTreeObjectProperty("max_valid_value", 8)]
        public uint MaxValidValue { get; private set; }
        [DataTreeObjectProperty("default_value", 9)]
        public uint DefaultValue { get; private set; }
        [DataTreeObjectProperty("description", 10)]
        public string Description { get; private set; }
        public const int PDL_MIN = 20;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"{Description}:");
            b.AppendLine($"DataType: {DataType}");
            b.AppendLine($"CommandClass: {CommandClass}");
            b.AppendLine($"MinValid: {this.Prefix.GetNormalizedValue(this.MinValidValue)}");
            b.AppendLine($"MaxValid: {this.Prefix.GetNormalizedValue(this.MaxValidValue)}");
            b.AppendLine($"Default: {this.Prefix.GetNormalizedValue(this.DefaultValue)}");
            b.AppendLine($"Unit: {Unit}");
            return b.ToString();
        }

        public static RDMParameterDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.PARAMETER_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMParameterDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMParameterDescription(
                parameterId: Tools.DataToUShort(ref data),
                pdlSize: Tools.DataToByte(ref data),
                dataType: Tools.DataToEnum<ERDM_DataType>(ref data),
                commandClass: Tools.DataToEnum<ERDM_CommandClass>(ref data),
                type: Tools.DataToByte(ref data),//Obsolet, but need it in constructor to remove the byte from Data
                unit: Tools.DataToEnum<ERDM_SensorUnit>(ref data),
                prefix: Tools.DataToEnum<ERDM_UnitPrefix>(ref data),
                minValidValue: Tools.DataToUInt(ref data),
                maxValidValue: Tools.DataToUInt(ref data),
                defaultValue: Tools.DataToUInt(ref data),
                description: Tools.DataToString(ref data)
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ParameterId));
            data.AddRange(Tools.ValueToData(this.PDLSize));
            data.AddRange(Tools.ValueToData(this.DataType));
            data.AddRange(Tools.ValueToData(this.CommandClass));
            data.AddRange(Tools.ValueToData((byte)0x00));
            data.AddRange(Tools.ValueToData(this.Unit));
            data.AddRange(Tools.ValueToData(this.Prefix));
            data.AddRange(Tools.ValueToData(this.MinValidValue));
            data.AddRange(Tools.ValueToData(this.MaxValidValue));
            data.AddRange(Tools.ValueToData(this.DefaultValue));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
