using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetBindingAndControlFieldsRequest : AbstractRDMPayloadObject
    {
        public GetBindingAndControlFieldsRequest(
            ushort endpointId = default,
            RDMUID uid = default)
        {
            this.EndpointId = endpointId;
            this.UID = uid;
        }

        public ushort EndpointId { get; private set; }
        public RDMUID UID { get; private set; }
        public const int PDL = 0x08;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - UID: {UID}";
        }

        public static GetBindingAndControlFieldsRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.BINDING_CONTROL_FIELDS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBindingAndControlFieldsRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetBindingAndControlFieldsRequest(
                endpointId: Tools.DataToUShort(ref data),
                uid: Tools.DataToRDMUID(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.UID));
            return data.ToArray();
        }
    }
}