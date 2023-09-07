using System;
using System.Collections.Generic;
using System.Net;

namespace RDMSharp
{
    public class GetSetIPv4StaticAddress : AbstractRDMPayloadObject
    {
        public GetSetIPv4StaticAddress(
            uint interfaceId = 0,
            IPAddress ipAddress = default,
            byte netmask = 24)
        {
            this.InterfaceId = interfaceId;
            this.IPAddress = ipAddress;
            if(netmask > 32)
                throw new Exception($"The valid range of {nameof(netmask)} is from 0 to 32");

            this.Netmask = netmask;
        }

        public uint InterfaceId { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public byte Netmask { get; private set; }
        public const int PDL = 0x09;

        public override string ToString()
        {
            return $"{InterfaceId} - {IPAddress}/{Netmask}";
        }

        public static GetSetIPv4StaticAddress FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.IPV4_STATIC_ADDRESS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4StaticAddress FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetIPv4StaticAddress(
                interfaceId: Tools.DataToUInt(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data),
                netmask: Tools.DataToByte(ref data));            

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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