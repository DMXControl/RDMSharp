using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMSensorValue : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMSensorValue(
            byte sensorId = 0,
            short value = 0,
            short lowestValue = 0,
            short highestValue = 0,
            short recordedValue = 0)
        {
            this.SensorId = sensorId;
            this.Value = value;
            this.LowestValue = lowestValue;
            this.HighestValue = highestValue;
            this.RecordedValue = recordedValue;
        }

        public byte SensorId { get; private set; }
        public short Value { get; private set; }
        public short LowestValue { get; private set; }
        public short HighestValue { get; private set; }
        public short RecordedValue { get; private set; }

        public object MinIndex => (byte)0;

        public object Index => SensorId;

        public const int PDL = 9;


        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSensorValue");
            b.AppendLine($"SensorId:      {SensorId}");
            b.AppendLine($"Value:         {Value}");
            b.AppendLine($"LowestValue:   {LowestValue}");
            b.AppendLine($"HighestValue:  {HighestValue}");
            b.AppendLine($"RecordedValue: {RecordedValue}");

            return b.ToString();
        }

        public static RDMSensorValue FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.SENSOR_VALUE) return null;
            if (msg.PDL < PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSensorValue FromPayloadData(byte[] data)
        {
            if (data.Length < PDL) return null;

            var i = new RDMSensorValue(
                sensorId: Tools.DataToByte(ref data),
                value: Tools.DataToShort(ref data),
                lowestValue: Tools.DataToShort(ref data),
                highestValue: Tools.DataToShort(ref data),
                recordedValue: Tools.DataToShort(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SensorId));
            data.AddRange(Tools.ValueToData(this.Value));
            data.AddRange(Tools.ValueToData(this.LowestValue));
            data.AddRange(Tools.ValueToData(this.HighestValue));
            data.AddRange(Tools.ValueToData(this.RecordedValue));
            return data.ToArray();
        }
    }
}