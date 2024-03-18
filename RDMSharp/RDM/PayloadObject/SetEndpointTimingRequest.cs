using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class SetEndpointTimingRequest : AbstractRDMPayloadObject
    {
        public SetEndpointTimingRequest(
            ushort endpointId = default,
            byte timingId = default)
        {
            this.EndpointId = endpointId;
            this.TimingId = timingId;
        }

        public ushort EndpointId { get; private set; }
        public byte TimingId { get; private set; }
        public byte Timings { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} TimingId: {TimingId}";
        }
        public static SetEndpointTimingRequest FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_TIMING, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static SetEndpointTimingRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new SetEndpointTimingRequest(
                endpointId: Tools.DataToUShort(ref data),
                timingId: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.TimingId));
            return data.ToArray();
        }
    }
}
