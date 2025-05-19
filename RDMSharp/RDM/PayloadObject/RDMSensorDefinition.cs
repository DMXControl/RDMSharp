using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.SENSOR_DEFINITION, Command.ECommandDublicte.GetResponse)]
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

        [DataTreeObjectConstructor]
        public RDMSensorDefinition(
            [DataTreeObjectParameter("sensor")] byte sensorId,
            [DataTreeObjectParameter("type")] byte type,
            [DataTreeObjectParameter("unit")] byte unit,
            [DataTreeObjectParameter("unit_prefix")] byte prefix,
            [DataTreeObjectParameter("range_min_value")] short rangeMinimum,
            [DataTreeObjectParameter("range_max_value")] short rangeMaximum,
            [DataTreeObjectParameter("normal_min_value")] short normalMinimum,
            [DataTreeObjectParameter("normal_max_value")] short normalMaximum,
            [DataTreeObjectParameter("recorded_value_support/recorded_value_supported")] bool recordedValueSupported,
            [DataTreeObjectParameter("recorded_value_support/low_high_detected_values_supported")] bool lowestHighestValueSupported,
            [DataTreeObjectParameter("description")] string description)
            : this(sensorId,
                   (ERDM_SensorType)type,
                   (ERDM_SensorUnit)unit,
                   (ERDM_UnitPrefix)prefix,
                   rangeMinimum,
                   rangeMaximum,
                   normalMinimum,
                   normalMaximum,
                   lowestHighestValueSupported,
                   recordedValueSupported,
                   description)
        {
        }

        [DataTreeObjectProperty("sensor", 0)]
        public byte SensorId { get; private set; }
        [DataTreeObjectProperty("type", 1)]
        public ERDM_SensorType Type { get; private set; }
        [DataTreeObjectProperty("unit", 2)]
        public ERDM_SensorUnit Unit { get; private set; }
        [DataTreeObjectProperty("unit_prefix", 3)]
        public ERDM_UnitPrefix Prefix { get; private set; }
        [DataTreeObjectProperty("range_min_value", 4)]
        public short RangeMinimum { get; private set; }
        [DataTreeObjectProperty("range_max_value", 5)]
        public short RangeMaximum { get; private set; }
        [DataTreeObjectProperty("normal_min_value", 6)]
        public short NormalMinimum { get; private set; }
        [DataTreeObjectProperty("normal_max_value", 7)]
        public short NormalMaximum { get; private set; }
        [DataTreeObjectProperty("recorded_value_support/recorded_value_supported", 0)]
        public bool LowestHighestValueSupported { get; private set; }
        [DataTreeObjectProperty("recorded_value_support/low_high_detected_values_supported", 1)]
        public bool RecordedValueSupported { get; private set; }
        [DataTreeObjectProperty("description", 9)]
        public string Description { get; private set; }

        public object MinIndex => (byte)0;
        public object Index => SensorId;

        public const int PDL_MIN = 13;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            string _unit = Unit.GetUnitSymbol();
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSensorDefinition");
            b.AppendLine($"SensorId:                    {SensorId}");
            b.AppendLine($"Type:                        {Type}");
            b.AppendLine($"Unit:                        {Unit}({_unit})");
            b.AppendLine($"Prefix:                      {Prefix}");
            b.AppendLine($"RangeMinimum:                {Prefix.GetNormalizedValue(RangeMinimum)}{_unit}");
            b.AppendLine($"RangeMaximum:                {Prefix.GetNormalizedValue(RangeMaximum)}{_unit}");
            b.AppendLine($"NormalMinimum:               {Prefix.GetNormalizedValue(NormalMinimum)}{_unit}");
            b.AppendLine($"NormalMaximum:               {Prefix.GetNormalizedValue(NormalMaximum)}{_unit}");
            b.AppendLine($"LowestHighestValueSupported: {LowestHighestValueSupported}");
            b.AppendLine($"RecordedValueSupported:      {RecordedValueSupported}");
            b.AppendLine($"Description:                 {Description}");

            return b.ToString();
        }

        public static RDMSensorDefinition FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SENSOR_DEFINITION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSensorDefinition FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

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
