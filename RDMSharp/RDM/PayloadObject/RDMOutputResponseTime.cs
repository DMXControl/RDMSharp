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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.OUTPUT_RESPONSE_TIME, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMOutputResponseTime FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
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
