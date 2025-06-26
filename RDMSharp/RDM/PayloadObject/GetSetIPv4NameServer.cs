using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DNS_IPV4_NAME_SERVER, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.DNS_IPV4_NAME_SERVER, Command.ECommandDublicate.SetRequest)]
    public class GetSetIPv4NameServer : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetIPv4NameServer(
            [DataTreeObjectParameter("index")] byte nameServerIndex = 0,
            [DataTreeObjectParameter("address")] IPv4Address ipAddress = default)
        {
            this.NameServerIndex = nameServerIndex;
            this.IPAddress = ipAddress;
        }

        [DataTreeObjectProperty("index", 0)]
        public byte NameServerIndex { get; private set; }
        [DataTreeObjectProperty("address", 1)]
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