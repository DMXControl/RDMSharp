﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.INTERFACE_LABEL, Command.ECommandDublicate.GetResponse)]
    public class GetInterfaceNameResponse : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetInterfaceNameResponse(
            [DataTreeObjectParameter("id")] uint interfaceId = 0,
            [DataTreeObjectParameter("label")] string label = "")
        {
            this.InterfaceId = interfaceId;

            if (string.IsNullOrWhiteSpace(label))
                return;

            if (label.Length > 32)
                label = label.Substring(0, 32);

            this.Label = label;
        }

        [DataTreeObjectProperty("id", 0)]
        public uint InterfaceId { get; private set; }
        [DataTreeObjectProperty("label", 1)]
        public string Label { get; private set; }
        public const int PDL_MIN = 4;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"GetInterfaceNameResponse: {InterfaceId} - {Label}";
        }

        public static GetInterfaceNameResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.INTERFACE_LABEL, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetInterfaceNameResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new GetInterfaceNameResponse(
                interfaceId: Tools.DataToUInt(ref data),
                label: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.Label, 32));
            return data.ToArray();
        }
    }
}
