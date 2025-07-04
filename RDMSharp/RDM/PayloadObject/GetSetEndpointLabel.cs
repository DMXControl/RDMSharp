﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.ENDPOINT_LABEL, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.ENDPOINT_LABEL, Command.ECommandDublicate.SetRequest)]
    public class GetSetEndpointLabel : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetEndpointLabel(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("label")] string endpointLabel = default)
        {
            this.EndpointId = endpointId;

            if (string.IsNullOrWhiteSpace(endpointLabel))
                return;

            if (endpointLabel.Length > 32)
                endpointLabel = endpointLabel.Substring(0, 32);

            this.EndpointLabel = endpointLabel;
        }

        [DataTreeObjectProperty("endpoint_id", 0)]
        public ushort EndpointId { get; private set; }
        [DataTreeObjectProperty("label", 1)]
        public string EndpointLabel { get; private set; }
        public const int PDL_MIN = 0x02;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - EndpointLabel: {EndpointLabel}";
        }

        public static GetSetEndpointLabel FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_LABEL, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointLabel FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new GetSetEndpointLabel(
                endpointId: Tools.DataToUShort(ref data),
                endpointLabel: Tools.DataToString(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.EndpointLabel));
            return data.ToArray();
        }
    }
}