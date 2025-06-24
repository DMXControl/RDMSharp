using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.TCP_COMMS_STATUS, Command.ECommandDublicate.GetResponse)]
    public class TCPCommsEntry : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public TCPCommsEntry(
            [DataTreeObjectParameter("scopeString")] string scopeString = default,
            [DataTreeObjectParameter("brokerAddress")] IPAddress brokerAddress = default,
            [DataTreeObjectParameter("brokerPort")] ushort brokerPort = default,
            [DataTreeObjectParameter("unhealthyTCPEvents")] ushort unhealthyTCPEvents = default)
        {

            if (string.IsNullOrWhiteSpace(scopeString))
                return;

            if (scopeString.Length > 62)
                scopeString = scopeString.Substring(0, 62);


            this.ScopeString = scopeString;
            this.BrokerAdress = brokerAddress;
            this.BrokerPort = brokerPort;
            this.UnhealthyTCPEvents = unhealthyTCPEvents;
        }

        [DataTreeObjectProperty("scopeString", 0)]
        public string ScopeString { get; private set; }
        [DataTreeObjectProperty("brokerAddress", 1)]
        public IPAddress BrokerAdress { get; private set; }
        [DataTreeObjectProperty("brokerPort", 2)]
        public ushort BrokerPort { get; private set; }
        [DataTreeObjectProperty("unhealthyTCPEvents", 3)]
        public ushort UnhealthyTCPEvents { get; private set; }

        public const int PDL = 0x57;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("TCPCommsEntry");
            b.AppendLine($"ScopeString:        {ScopeString}");
            b.AppendLine($"BrokerAdress:       {BrokerAdress}");
            b.AppendLine($"BrokerPort:         {BrokerPort}");
            b.AppendLine($"UnhealthyTCPEvents: {UnhealthyTCPEvents}");

            return b.ToString();
        }

        public static TCPCommsEntry FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.TCP_COMMS_STATUS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static TCPCommsEntry FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var scopeString = Tools.DataToString(ref data, 63).Replace("\u0000", "");
            byte[] ipv4Bytes = data.Take(4).ToArray();
            byte[] ipv6Bytes = data.Skip(4).Take(16).ToArray();
            data = data.Skip(20).ToArray();
            IPAddress brokerAddress = null;

            if (ipv4Bytes.Any(b => b != 0))
                brokerAddress = Tools.DataToIPAddressIPv4(ref ipv4Bytes);
            else if (ipv6Bytes.Any(b => b != 0))
                brokerAddress = Tools.DataToIPAddressIPv6(ref ipv6Bytes);

            ushort staticBrokerPort = Tools.DataToUShort(ref data);
            ushort unhealthyTCPEvents = Tools.DataToUShort(ref data);

            var i = new TCPCommsEntry(
                scopeString: scopeString,
                brokerAddress: brokerAddress,
                brokerPort: staticBrokerPort,
                unhealthyTCPEvents: unhealthyTCPEvents);

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> scopeStringBytes = new List<byte>();
            scopeStringBytes.AddRange(Tools.ValueToData(this.ScopeString, 62));
            while (scopeStringBytes.Count < 63)
                scopeStringBytes.Add(0);
            var ipv4Bytes = this.BrokerAdress?.AddressFamily == AddressFamily.InterNetwork ? Tools.ValueToData(this.BrokerAdress) : new byte[4];
            var ipv6Bytes = this.BrokerAdress?.AddressFamily == AddressFamily.InterNetworkV6 ? Tools.ValueToData(this.BrokerAdress) : new byte[16];

            var brokerPortBytes = Tools.ValueToData(this.BrokerPort);
            var unhealthyTCPEventsBytes = Tools.ValueToData(this.UnhealthyTCPEvents);

            List<byte> data = new List<byte>();
            data.AddRange(scopeStringBytes);
            data.AddRange(ipv4Bytes);
            data.AddRange(ipv6Bytes);
            data.AddRange(brokerPortBytes);
            data.AddRange(unhealthyTCPEventsBytes);
            return data.ToArray();
        }
    }
}