using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMSensorValue : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMSensorValue(
            byte sensorId = 0,
            short presentvalue = 0,
            short lowestValue = 0,
            short highestValue = 0,
            short recordedValue = 0)
        {
            this.SensorId = sensorId;
            this.PresentValue = presentvalue;
            this.LowestValue = lowestValue;
            this.HighestValue = highestValue;
            this.RecordedValue = recordedValue;
        }

        public byte SensorId { get; private set; }
        public short PresentValue { get; private set; }
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
            b.AppendLine($"PresentValue:  {PresentValue}");
            b.AppendLine($"LowestValue:   {LowestValue}");
            b.AppendLine($"HighestValue:  {HighestValue}");
            b.AppendLine($"RecordedValue: {RecordedValue}");

            return b.ToString();
        }

        public static RDMSensorValue FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SENSOR_VALUE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSensorValue FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMSensorValue(
                sensorId: Tools.DataToByte(ref data),
                presentvalue: Tools.DataToShort(ref data),
                lowestValue: Tools.DataToShort(ref data),
                highestValue: Tools.DataToShort(ref data),
                recordedValue: Tools.DataToShort(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SensorId));
            data.AddRange(Tools.ValueToData(this.PresentValue));
            data.AddRange(Tools.ValueToData(this.LowestValue));
            data.AddRange(Tools.ValueToData(this.HighestValue));
            data.AddRange(Tools.ValueToData(this.RecordedValue));
            return data.ToArray();
        }
    }
}