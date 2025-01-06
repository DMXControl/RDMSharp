﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Net;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.IPV4_CURRENT_ADDRESS, Command.ECommandDublicte.GetResponse)]
    public class GetIPv4CurrentAddressResponse : AbstractRDMPayloadObject
    {
        public GetIPv4CurrentAddressResponse(
            uint interfaceId = 0,
            IPv4Address ipAddress = default,
            byte netmask = 24,
            ERDM_DHCPStatusMode dhcpStatus = ERDM_DHCPStatusMode.UNKNOWN)
        {
            this.InterfaceId = interfaceId;
            this.IPAddress = ipAddress;
            if (netmask > 32)
                throw new Exception($"The valid range of {nameof(netmask)} is from 0 to 32");

            this.Netmask = netmask;
            this.DHCPStatus = dhcpStatus;
        }
        [DataTreeObjectConstructor]
        public GetIPv4CurrentAddressResponse(
            [DataTreeObjectParameter("id")] uint interfaceId,
            [DataTreeObjectParameter("address")] IPv4Address ipAddress,
            [DataTreeObjectParameter("netmask")] byte netmask,
            [DataTreeObjectParameter("dhcp_status")] byte dhcpStatus)
            :this(interfaceId, ipAddress, netmask, (ERDM_DHCPStatusMode)dhcpStatus)
        {
        }

        public uint InterfaceId { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public byte Netmask { get; private set; }
        public ERDM_DHCPStatusMode DHCPStatus { get; private set; }
        public const int PDL = 0x0A;

        public override string ToString()
        {
            return $"{InterfaceId} - {IPAddress}/{Netmask} \tDHCP: {DHCPStatus}";
        }

        public static GetIPv4CurrentAddressResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.IPV4_CURRENT_ADDRESS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetIPv4CurrentAddressResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetIPv4CurrentAddressResponse(
                interfaceId: Tools.DataToUInt(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data),
                netmask: Tools.DataToByte(ref data),
                dhcpStatus: Tools.DataToEnum<ERDM_DHCPStatusMode>(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.IPAddress));
            data.AddRange(Tools.ValueToData(this.Netmask));
            data.AddRange(Tools.ValueToData(this.DHCPStatus));
            return data.ToArray();
        }
    }
}