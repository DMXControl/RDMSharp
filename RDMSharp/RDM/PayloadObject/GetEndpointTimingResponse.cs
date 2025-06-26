using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.ENDPOINT_TIMING, Command.ECommandDublicate.GetResponse)]
    public class GetEndpointTimingResponse : AbstractRDMPayloadObjectOneOf
    {
        [DataTreeObjectConstructor]
        public GetEndpointTimingResponse(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("setting")] byte timingId = 1,
            [DataTreeObjectParameter("setting_count")] byte timings = default)
        {
            this.EndpointId = endpointId;
            this.TimingId = timingId;
            this.Timings = timings;
        }

        [DataTreeObjectProperty("endpoint_id", 0)]
        public ushort EndpointId { get; private set; }
        [DataTreeObjectProperty("setting", 1)]
        public byte TimingId { get; private set; }
        [DataTreeObjectProperty("setting_count", 2)]
        public byte Timings { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => TimingId;

        public override object Count => Timings;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION;

        public const int PDL = 4;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} TimingId: {TimingId} of {Timings}";
        }
        public static GetEndpointTimingResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_TIMING, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointTimingResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new GetEndpointTimingResponse(
                endpointId: Tools.DataToUShort(ref data),
                timingId: Tools.DataToByte(ref data),
                timings: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.TimingId));
            data.AddRange(Tools.ValueToData(this.Timings));
            return data.ToArray();
        }
    }
}
