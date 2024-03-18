﻿using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetIPv4StaticAddress : AbstractRDMPayloadObject
    {
        public GetSetIPv4StaticAddress(
            uint interfaceId = 0,
            IPv4Address ipAddress = default,
            byte netmask = 24)
        {
            this.InterfaceId = interfaceId;
            this.IPAddress = ipAddress;
            if (netmask > 32)
                throw new Exception($"The valid range of {nameof(netmask)} is from 0 to 32");

            this.Netmask = netmask;
        }

        public uint InterfaceId { get; private set; }
        public IPv4Address IPAddress { get; private set; }
        public byte Netmask { get; private set; }
        public const int PDL = 0x09;

        public override string ToString()
        {
            return $"{InterfaceId} - {IPAddress}/{Netmask}";
        }

        public static GetSetIPv4StaticAddress FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.IPV4_STATIC_ADDRESS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4StaticAddress FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetIPv4StaticAddress(
                interfaceId: Tools.DataToUInt(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data),
                netmask: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.IPAddress));
            data.AddRange(Tools.ValueToData(this.Netmask));
            return data.ToArray();
        }
    }
}