using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMParameterDescription : AbstractRDMPayloadObject
    {
        public RDMParameterDescription(
            ushort parameterId = 0,
            byte pdlSize = 0,
            ERDM_DataType dataType = ERDM_DataType.NOT_DEFINED,
            ERDM_CommandClass commandClass = ERDM_CommandClass.GET,
            byte type = 0, //Obsolete
            ERDM_SensorUnit unit = ERDM_SensorUnit.NONE,
            ERDM_UnitPrefix prefix = ERDM_UnitPrefix.NONE,
            int minValidValue = 0,
            int maxValidValue = 0,
            int defaultValue = 0,
            string description = "")
        {
            this.ParameterId = parameterId;
            this.PDLSize = pdlSize;
            this.DataType = dataType;
            this.CommandClass = commandClass;
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

        public ushort ParameterId { get; private set; }
        public byte PDLSize { get; private set; }
        public ERDM_DataType DataType { get; private set; }
        public ERDM_CommandClass CommandClass { get; private set; }
        public ERDM_SensorUnit Unit { get; private set; }
        public ERDM_UnitPrefix Prefix { get; private set; }
        public int MinValidValue { get; private set; }
        public int MaxValidValue { get; private set; }
        public int DefaultValue { get; private set; }
        public string Description { get; private set; }
        public const int PDL_MIN = 20;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"{Description}:");
            b.AppendLine($"DataType: {DataType}");
            b.AppendLine($"CommandClass: {CommandClass}");
            b.AppendLine($"MinValid: {Tools.GetNormalizedValue(this.Prefix, this.MinValidValue)}");
            b.AppendLine($"MaxValid: {Tools.GetNormalizedValue(this.Prefix, this.MaxValidValue)}");
            b.AppendLine($"Default: {Tools.GetNormalizedValue(this.Prefix, this.DefaultValue)}");
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
                minValidValue: Tools.DataToInt(ref data),
                maxValidValue: Tools.DataToInt(ref data),
                defaultValue: Tools.DataToInt(ref data),
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
