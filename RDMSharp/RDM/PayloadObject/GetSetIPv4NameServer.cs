using System;
using System.Collections.Generic;
using System.Net;

namespace RDMSharp
{
    public class GetSetIPv4NameServer : AbstractRDMPayloadObject
    {
        public GetSetIPv4NameServer(
            byte nameServerIndex = 0,
            IPAddress ipAddress = default)
        {
            this.NameServerIndex = nameServerIndex;
            this.IPAddress = ipAddress;
        }

        public byte NameServerIndex { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public const int PDL = 0x05;

        public override string ToString()
        {
            return $"{NameServerIndex} - {IPAddress}";
        }

        public static GetSetIPv4NameServer FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DNS_IPV4_NAME_SERVER) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIPv4NameServer FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetIPv4NameServer(
                nameServerIndex: Tools.DataToByte(ref data),
                ipAddress: Tools.DataToIPAddressIPv4(ref data));            

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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