using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetBindingAndControlFieldsResponse : AbstractRDMPayloadObject
    {
        public GetBindingAndControlFieldsResponse(
            ushort endpointId = default,
            RDMUID uid = default,
            ushort controlField = default,
            RDMUID bindingUID = default)
        {
            this.EndpointId = endpointId;
            this.UID = uid;
            this.ControlField = controlField;
            this.BindingUID = bindingUID;
        }

        public ushort EndpointId { get; private set; }
        public RDMUID UID { get; private set; }
        public ushort ControlField { get; private set; }
        public RDMUID BindingUID { get; private set; }
        public const int PDL = 0x10;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - UID: {UID} ControlField: {ControlField} BindingUID: {BindingUID}";
        }

        public static GetBindingAndControlFieldsResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.BINDING_CONTROL_FIELDS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBindingAndControlFieldsResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetBindingAndControlFieldsResponse(
                endpointId: Tools.DataToUShort(ref data),
                uid: Tools.DataToRDMUID(ref data),
                controlField: Tools.DataToUShort(ref data),
                bindingUID: Tools.DataToRDMUID(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.UID));
            data.AddRange(Tools.ValueToData(this.ControlField));
            data.AddRange(Tools.ValueToData(this.BindingUID));
            return data.ToArray();
        }
    }
}