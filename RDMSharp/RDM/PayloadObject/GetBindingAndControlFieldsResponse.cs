using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BINDING_CONTROL_FIELDS, Command.ECommandDublicate.GetResponse)]
    public class GetBindingAndControlFieldsResponse : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetBindingAndControlFieldsResponse(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("uid")] UID uid = default,
            [DataTreeObjectParameter("control")] ushort controlField = default,
            [DataTreeObjectParameter("binding_uid")] UID bindingUID = default)
        {
            this.EndpointId = endpointId;
            this.UID = uid;
            this.ControlField = controlField;
            this.BindingUID = bindingUID;
        }

        public ushort EndpointId { get; private set; }
        public UID UID { get; private set; }
        public ushort ControlField { get; private set; }
        public UID BindingUID { get; private set; }
        public const int PDL = 0x10;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - UID: {UID} ControlField: {ControlField} BindingUID: {BindingUID}";
        }

        public static GetBindingAndControlFieldsResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.BINDING_CONTROL_FIELDS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBindingAndControlFieldsResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetBindingAndControlFieldsResponse(
                endpointId: Tools.DataToUShort(ref data),
                uid: Tools.DataToRDMUID(ref data),
                controlField: Tools.DataToUShort(ref data),
                bindingUID: Tools.DataToRDMUID(ref data));

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