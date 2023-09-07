using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RDMSharp
{
    public class TCPCommsEntry : AbstractRDMPayloadObject
    {
        public TCPCommsEntry(
            string scopeString = default,
            IPAddress brokerAddress = default,
            ushort brokerPort = default,
            ushort unhealthyTCPEvents = default)
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

        public string ScopeString { get; private set; }
        public IPAddress BrokerAdress { get; private set; }
        public ushort BrokerPort { get; private set; }
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.TCP_COMMS_STATUS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static TCPCommsEntry FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var scopeString = Tools.DataToString(ref data, 63).Replace("\u0000","");
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

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        { 
            List<byte> scopeStringBytes = new List<byte>();
            scopeStringBytes.AddRange(Tools.ValueToData(this.ScopeString, 62));
            while (scopeStringBytes.Count < 63)
                scopeStringBytes.Add(0);
            var ipv4Bytes = this.BrokerAdress.AddressFamily == AddressFamily.InterNetwork ? Tools.ValueToData(this.BrokerAdress) : new byte[4];
            var ipv6Bytes = this.BrokerAdress.AddressFamily == AddressFamily.InterNetworkV6 ? Tools.ValueToData(this.BrokerAdress) : new byte[16];

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