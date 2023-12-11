using System;
using System.Collections.Generic;
using System.Net;

namespace RDMSharp
{
    public class GetSetIPv4DefaultRoute : AbstractRDMPayloadObject
    {
        public GetSetIPv4DefaultRoute(
            uint interfaceId = 0,
            IPAddress ipAddress = default)
        {
            this.InterfaceId = interfaceId;
            this.IPAddress = ipAddress;
        }

        public uint InterfaceId { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public const int PDL = 0x08;

        public override string ToString()
        {
            return $"{InterfaceId} - {IPAddress}";
        }

        public static GetSetIPv4DefaultRoute FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.IPV4_DEFAULT_ROUTE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4DefaultRoute FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetIPv4DefaultRoute(
                interfaceId: Tools.DataToUInt(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.InterfaceId));
            data.AddRange(Tools.ValueToData(this.IPAddress));
            return data.ToArray();
        }
    }
}