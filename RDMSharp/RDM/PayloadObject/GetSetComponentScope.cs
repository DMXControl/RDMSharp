using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RDMSharp
{
    public class GetSetComponentScope : AbstractRDMPayloadObject
    {
        internal GetSetComponentScope(
            ushort scopeSlot = default,
            string scopeString = default,
            ERDM_StaticConfig staticConfigType = default,
            IPAddress staticBrokerIPv4 = default,
            IPAddress staticBrokerIPv6 = default,
            ushort staticBrokerPort = default)
        {

            if (string.IsNullOrWhiteSpace(scopeString))
                return;

            if (scopeString.Length > 62)
                scopeString = scopeString.Substring(0, 62);


            this.ScopeSlot = scopeSlot;
            this.ScopeString = scopeString;
            this.StaticConfigType = staticConfigType;
            switch (staticConfigType)
            {
                case ERDM_StaticConfig.IPv4:
                    if (staticBrokerIPv4 != null)
                        if (staticBrokerIPv4.AddressFamily != AddressFamily.InterNetwork)
                            throw new ArgumentException($"{nameof(staticBrokerIPv4)} should be IPv4 ;)");
                    this.StaticBrokerIPv4 = staticBrokerIPv4;
                    break;
                case ERDM_StaticConfig.IPv6:
                    if (staticBrokerIPv6 != null)
                        if (staticBrokerIPv6.AddressFamily != AddressFamily.InterNetworkV6)
                            throw new ArgumentException($"{nameof(staticBrokerIPv6)} should be IPv6 ;)");
                    this.StaticBrokerIPv6 = staticBrokerIPv6;
                    break;
            }
            this.StaticBrokerPort = staticBrokerPort;
        }

        public GetSetComponentScope(
            ushort scopeSlot = default,
            string scopeString = default,
            IPAddress staticBroker = default,
            ushort staticBrokerPort = default) : this(
                scopeSlot: scopeSlot,
                scopeString: scopeString,
                staticConfigType: getStaticConfig(staticBroker),
                staticBrokerIPv4: staticBroker?.AddressFamily == AddressFamily.InterNetwork ? staticBroker : null,
                staticBrokerIPv6: staticBroker?.AddressFamily == AddressFamily.InterNetworkV6 ? staticBroker : null,
                staticBrokerPort: staticBrokerPort)
        {
        }
        private static ERDM_StaticConfig getStaticConfig(IPAddress ipaddress)
        {
            switch (ipaddress?.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return ERDM_StaticConfig.IPv4;
                case AddressFamily.InterNetworkV6:
                    return ERDM_StaticConfig.IPv6;
            }
            return ERDM_StaticConfig.NO;
        }

        public ushort ScopeSlot { get; private set; }
        public string ScopeString { get; private set; }
        public ERDM_StaticConfig StaticConfigType { get; private set; }
        public IPAddress StaticBrokerIPv4 { get; private set; }
        public IPAddress StaticBrokerIPv6 { get; private set; }
        public ushort StaticBrokerPort { get; private set; }

        public const int PDL = 0x54;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("GetSetComponentScope");
            b.AppendLine($"ScopeSlot:        {ScopeSlot}");
            b.AppendLine($"ScopeString:      {ScopeString}");
            b.AppendLine($"StaticConfigType: {StaticConfigType}");
            b.AppendLine($"StaticBrokerIPv4: {StaticBrokerIPv4}");
            b.AppendLine($"StaticBrokerIPv6: {StaticBrokerIPv6}");
            b.AppendLine($"StaticBrokerPort: {StaticBrokerPort}");

            return b.ToString();
        }

        public static GetSetComponentScope FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.COMPONENT_SCOPE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetComponentScope FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var scopeSlot = Tools.DataToUShort(ref data);
            var scopeString = Tools.DataToString(ref data, 63).Replace("\u0000", "");
            var staticConfigType = Tools.DataToEnum<ERDM_StaticConfig>(ref data);
            IPAddress staticBrokerIPv4 = null;
            IPAddress staticBrokerIPv6 = null;
            ushort staticBrokerPort = 0;
            switch (staticConfigType)
            {
                case ERDM_StaticConfig.IPv4:
                    staticBrokerIPv4 = Tools.DataToIPAddressIPv4(ref data);
                    data = data.Skip(12).ToArray();
                    break;
                case ERDM_StaticConfig.IPv6:
                    staticBrokerIPv6 = Tools.DataToIPAddressIPv6(ref data);
                    break;
                case ERDM_StaticConfig.NO:
                default:
                    data = data.Skip(16).ToArray();
                    break;
            }

            staticBrokerPort = Tools.DataToUShort(ref data);
            var i = new GetSetComponentScope(
                scopeSlot: scopeSlot,
                scopeString: scopeString,
                staticConfigType: staticConfigType,
                staticBrokerIPv4: staticBrokerIPv4,
                staticBrokerIPv6: staticBrokerIPv6,
                staticBrokerPort: staticBrokerPort);

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

            var slotBytes = Tools.ValueToData(this.ScopeSlot);
            var configTypeBytes = Tools.ValueToData(this.StaticConfigType);
            var brokerPortBytes = Tools.ValueToData(this.StaticBrokerPort);

            List<byte> data = new List<byte>();
            data.AddRange(slotBytes);
            data.AddRange(scopeStringBytes);
            data.AddRange(configTypeBytes);
            switch (this.StaticConfigType)
            {
                case ERDM_StaticConfig.IPv4:
                    data.AddRange(Tools.ValueToData(this.StaticBrokerIPv4));
                    data.AddRange(new byte[12]);//Fill the rest with zeros
                    break;
                case ERDM_StaticConfig.IPv6:
                    data.AddRange(Tools.ValueToData(this.StaticBrokerIPv6));
                    break;
                case ERDM_StaticConfig.NO:
                default:
                    data.AddRange(new byte[16]);//Fill the rest with zeros
                    break;
            }
            data.AddRange(brokerPortBytes);
            return data.ToArray();
        }
    }
}