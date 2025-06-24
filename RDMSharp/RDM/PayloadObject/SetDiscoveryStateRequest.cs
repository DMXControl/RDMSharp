﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DISCOVERY_STATE, Command.ECommandDublicate.SetResponse)]
    public class SetDiscoveryStateRequest : AbstractRDMPayloadObject
    {
        public SetDiscoveryStateRequest(
            ushort endpointId = default,
            ERDM_DiscoveryState discoveryState = default)
        {
            this.EndpointId = endpointId;
            this.DiscoveryState = discoveryState;
        }
        [DataTreeObjectConstructor]
        public SetDiscoveryStateRequest(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId,
            [DataTreeObjectParameter("state")] byte discoveryState)
            : this(endpointId, (ERDM_DiscoveryState)discoveryState)
        {
        }

        [DataTreeObjectProperty("endpoint_id", 0)]
        public ushort EndpointId { get; private set; }
        [DataTreeObjectProperty("state", 1)]
        public ERDM_DiscoveryState DiscoveryState { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - DiscoveryState: {DiscoveryState}";
        }

        public static SetDiscoveryStateRequest FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DISCOVERY_STATE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static SetDiscoveryStateRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new SetDiscoveryStateRequest(
                endpointId: Tools.DataToUShort(ref data),
                discoveryState: Tools.DataToEnum<ERDM_DiscoveryState>(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.DiscoveryState));
            return data.ToArray();
        }
    }
}