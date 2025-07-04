﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.COMPONENT_SCOPE, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.COMPONENT_SCOPE, Command.ECommandDublicate.SetRequest)]
    public class GetSetComponentScope : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetComponentScope(
            [DataTreeObjectParameter("scope_slot")] ushort scopeSlot = default,
            [DataTreeObjectParameter("scope_string")] string scopeString = default,
            [DataTreeObjectParameter("static_config_type")] ERDM_StaticConfig staticConfigType = default,
            [DataTreeObjectParameter("static_broker_ipv4")] IPAddress staticBrokerIPv4 = default,
            [DataTreeObjectParameter("static_broker_ipv6")] IPAddress staticBrokerIPv6 = default,
            [DataTreeObjectParameter("static_broker_port")] ushort staticBrokerPort = default)
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

        [DataTreeObjectProperty("scope_slot", 0)]
        public ushort ScopeSlot { get; private set; }
        [DataTreeObjectProperty("scope_string", 1)]
        public string ScopeString { get; private set; }
        [DataTreeObjectProperty("static_config_type", 3)]
        public ERDM_StaticConfig StaticConfigType { get; private set; }
        [DataTreeObjectProperty("static_broker_ipv4", 4)]
        public IPv4Address StaticBrokerIPv4 { get; private set; }
        [DataTreeObjectProperty("static_broker_ipv6", 5)]
        public IPAddress StaticBrokerIPv6 { get; private set; }
        [DataTreeObjectProperty("static_broker_port", 6)]
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.COMPONENT_SCOPE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetComponentScope FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var scopeSlot = Tools.DataToUShort(ref data);
            var scopeString = Tools.DataToString(ref data, 63).Replace("\u0000", "");
            var staticConfigType = Tools.DataToEnum<ERDM_StaticConfig>(ref data);
            IPv4Address? staticBrokerIPv4 = null;
            IPAddress staticBrokerIPv6 = null;
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

            ushort staticBrokerPort = Tools.DataToUShort(ref data);
            var i = new GetSetComponentScope(
                scopeSlot: scopeSlot,
                scopeString: scopeString,
                staticConfigType: staticConfigType,
                staticBrokerIPv4: staticBrokerIPv4,
                staticBrokerIPv6: staticBrokerIPv6,
                staticBrokerPort: staticBrokerPort);

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