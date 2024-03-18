using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetIPv4DefaultRoute : AbstractRDMPayloadObject
    {
        public GetSetIPv4DefaultRoute(
            uint interfaceId = 0,
            IPv4Address ipAddress = default)
        {
            this.InterfaceId = interfaceId;
            this.IPAddress = ipAddress;
        }

        public uint InterfaceId { get; private set; }
        public IPv4Address IPAddress { get; private set; }
        public const int PDL = 0x08;

        public override string ToString()
        {
            return $"{InterfaceId} - {IPAddress}";
        }

        public static GetSetIPv4DefaultRoute FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.IPV4_DEFAULT_ROUTE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4DefaultRoute FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetIPv4DefaultRoute(
                interfaceId: Tools.DataToUInt(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data));

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