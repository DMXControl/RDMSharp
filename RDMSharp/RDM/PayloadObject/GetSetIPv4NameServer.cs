using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetIPv4NameServer : AbstractRDMPayloadObject
    {
        public GetSetIPv4NameServer(
            byte nameServerIndex = 0,
            IPv4Address ipAddress = default)
        {
            this.NameServerIndex = nameServerIndex;
            this.IPAddress = ipAddress;
        }

        public byte NameServerIndex { get; private set; }
        public IPv4Address IPAddress { get; private set; }
        public const int PDL = 0x05;

        public override string ToString()
        {
            return $"{NameServerIndex} - {IPAddress}";
        }

        public static GetSetIPv4NameServer FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DNS_IPV4_NAME_SERVER, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4NameServer FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetIPv4NameServer(
                nameServerIndex: Tools.DataToByte(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.NameServerIndex));
            data.AddRange(Tools.ValueToData(this.IPAddress));
            return data.ToArray();
        }
    }
}