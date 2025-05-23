using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BINDING_CONTROL_FIELDS, Command.ECommandDublicate.GetRequest)]
    public class GetBindingAndControlFieldsRequest : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetBindingAndControlFieldsRequest(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("uid")] UID uid = default)
        {
            this.EndpointId = endpointId;
            this.UID = uid;
        }

        public ushort EndpointId { get; private set; }
        public UID UID { get; private set; }
        public const int PDL = 0x08;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - UID: {UID}";
        }

        public static GetBindingAndControlFieldsRequest FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND, ERDM_Parameter.BINDING_CONTROL_FIELDS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBindingAndControlFieldsRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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