using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, Command.ECommandDublicte.GetResponse)]
    public class GetEndpointResponderListChangeResponse : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetEndpointResponderListChangeResponse(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("list_change_number")] uint listChangeNumber = default)
        {
            this.EndpointId = endpointId;
            this.ListChangeNumber = listChangeNumber;
        }

        public ushort EndpointId { get; private set; }
        public uint ListChangeNumber { get; private set; }
        public const int PDL = 0x06;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} Responder ListChangeNumber: {ListChangeNumber:X}";
        }

        public static GetEndpointResponderListChangeResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointResponderListChangeResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetEndpointResponderListChangeResponse(
                endpointId: Tools.DataToUShort(ref data),
                listChangeNumber: Tools.DataToUInt(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.ListChangeNumber));
            return data.ToArray();
        }
    }
}