using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMOutputResponseTime : AbstractRDMPayloadObjectOneOf
    {
        public RDMOutputResponseTime(
            byte currentResponseTimeId = 1,
            byte responseTimes = 0)
        {
            this.CurrentResponseTimeId = currentResponseTimeId;
            this.ResponseTimes = responseTimes;
        }

        public byte CurrentResponseTimeId { get; private set; }
        public byte ResponseTimes { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => CurrentResponseTimeId;

        public override object Count => ResponseTimes;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"RDMOutputResponseTime: {CurrentResponseTimeId} of {ResponseTimes}";
        }
        public static RDMOutputResponseTime FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.OUTPUT_RESPONSE_TIME) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMOutputResponseTime FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new RDMOutputResponseTime(
                currentResponseTimeId: Tools.DataToByte(ref data),
                responseTimes: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.CurrentResponseTimeId));
            data.AddRange(Tools.ValueToData(this.ResponseTimes));
            return data.ToArray();
        }
    }
}
