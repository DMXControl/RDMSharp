using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMSensorDefinition : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMSensorDefinition(
            byte sensorId = 0,
            ERDM_SensorType type = (ERDM_SensorType)0,
            ERDM_SensorUnit unit = (ERDM_SensorUnit)0,
            ERDM_UnitPrefix prefix = (ERDM_UnitPrefix)0,
            short rangeMinimum = 0,
            short rangeMaximum = 0,
            short normalMinimum = 0,
            short normalMaximum = 0,
            bool lowestHighestValueSupported = false,
            bool recordedValueSupported = false,
            string description = "")
        {
            this.SensorId = sensorId;
            this.Type = type;
            this.Unit = unit;
            this.Prefix = prefix;
            this.RangeMaximum = rangeMaximum;
            this.RangeMinimum = rangeMinimum;
            this.NormalMaximum = normalMaximum;
            this.NormalMinimum = normalMinimum;
            this.LowestHighestValueSupported = lowestHighestValueSupported;
            this.RecordedValueSupported = recordedValueSupported;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte SensorId { get; private set; }
        public ERDM_SensorType Type { get; private set; }
        public ERDM_SensorUnit Unit { get; private set; }
        public ERDM_UnitPrefix Prefix { get; private set; }
        public short RangeMinimum { get; private set; }
        public short RangeMaximum { get; private set; }
        public short NormalMinimum { get; private set; }
        public short NormalMaximum { get; private set; }
        public bool LowestHighestValueSupported { get; private set; }
        public bool RecordedValueSupported { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)0;
        public object Index => SensorId;

        public const int PDL_MIN = 13;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            string _unit = Tools.GetUnitSymbol(Unit);
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSensorDefinition");
            b.AppendLine($"SensorId:                    {SensorId}");
            b.AppendLine($"Type:                        {Type}");
            b.AppendLine($"Unit:                        {Unit}({_unit})");
            b.AppendLine($"Prefix:                      {Prefix}");
            b.AppendLine($"RangeMinimum:                {Tools.GetNormalizedValue(Prefix, RangeMinimum)}{_unit}");
            b.AppendLine($"RangeMaximum:                {Tools.GetNormalizedValue(Prefix, RangeMaximum)}{_unit}");
            b.AppendLine($"NormalMinimum:               {Tools.GetNormalizedValue(Prefix, NormalMinimum)}{_unit}");
            b.AppendLine($"NormalMaximum:               {Tools.GetNormalizedValue(Prefix, NormalMaximum)}{_unit}");
            b.AppendLine($"LowestHighestValueSupported: {LowestHighestValueSupported}");
            b.AppendLine($"RecordedValueSupported:      {RecordedValueSupported}");
            b.AppendLine($"Description:                 {Description}");

            return b.ToString();
        }

        public static RDMSensorDefinition FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.SENSOR_DEFINITION) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSensorDefinition FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var sensorId = Tools.DataToByte(ref data);
            var type = Tools.DataToEnum<ERDM_SensorType>(ref data);
            var unit = Tools.DataToEnum<ERDM_SensorUnit>(ref data);
            var prefix = Tools.DataToEnum<ERDM_UnitPrefix>(ref data);
            var rangeMinimum = Tools.DataToShort(ref data);
            var rangeMaximum = Tools.DataToShort(ref data);
            var normalMinimum = Tools.DataToShort(ref data);
            var normalMaximum = Tools.DataToShort(ref data);
            var @boolArray = Tools.DataToBoolArray(ref data, 2);
            var description = Tools.DataToString(ref data);

            var i = new RDMSensorDefinition(
                sensorId: sensorId,
                type: type,
                unit: unit,
                prefix: prefix,
                rangeMinimum: rangeMinimum,
                rangeMaximum: rangeMaximum,
                normalMinimum: normalMinimum,
                normalMaximum: normalMaximum,
                lowestHighestValueSupported: @boolArray[1],
                recordedValueSupported: @boolArray[0],
                description: description
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SensorId));
            data.AddRange(Tools.ValueToData(this.Type));
            data.AddRange(Tools.ValueToData(this.Unit));
            data.AddRange(Tools.ValueToData(this.Prefix));
            data.AddRange(Tools.ValueToData(this.RangeMinimum));
            data.AddRange(Tools.ValueToData(this.RangeMaximum));
            data.AddRange(Tools.ValueToData(this.NormalMinimum));
            data.AddRange(Tools.ValueToData(this.NormalMaximum));
            data.AddRange(Tools.ValueToData(this.RecordedValueSupported, this.LowestHighestValueSupported));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
