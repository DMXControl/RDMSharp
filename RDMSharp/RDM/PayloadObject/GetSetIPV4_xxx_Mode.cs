﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.IPV4_DHCP_MODE, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(ERDM_Parameter.IPV4_DHCP_MODE, Command.ECommandDublicte.SetRequest)]
    [DataTreeObject(ERDM_Parameter.IPV4_ZEROCONF_MODE, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(ERDM_Parameter.IPV4_ZEROCONF_MODE, Command.ECommandDublicte.SetRequest)]
    public class GetSetIPV4_xxx_Mode : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetIPV4_xxx_Mode(
            [DataTreeObjectParameter("id")] uint interfaceId = 0,
            [DataTreeObjectParameter("mode")] bool enabled = false)
        {
            this.InterfaceId = interfaceId;
            this.Enabled = enabled;
        }

        public uint InterfaceId { get; private set; }
        public bool Enabled { get; private set; }
        public const int PDL = 5;

        public override string ToString()
        {
            return $"GetSetDHCPMode: {InterfaceId} - {Enabled}";
        }

        public static GetSetIPV4_xxx_Mode FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.IPV4_DHCP_MODE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPV4_xxx_Mode FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetIPV4_xxx_Mode(
                interfaceId: Tools.DataToUInt(ref data),
                enabled: Tools.DataToBool(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.Enabled));
            return data.ToArray();
        }
    }
}
