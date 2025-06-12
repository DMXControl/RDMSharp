﻿using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class RDMMessage : IEquatable<RDMMessage>
    {
        private static readonly ILogger Logger = Logging.CreateLogger<RDMMessage>();
        private byte[] _parameterData = Array.Empty<byte>();
        private byte? preambleCount = null;

        public RDMMessage()
        {

        }
        public RDMMessage(in byte[] data)
        {
#if NETSTANDARD
            if (data == null)
                throw new ArgumentNullException(nameof(data));
#else
            ArgumentNullException.ThrowIfNull(data);
#endif

            if (data.Length < 26)
            {
                if (data.Length >= 17 && (data[0] == 0xFE || data[0] == 0xAA))
                {
                    Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE;
                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH;

                    //Parse Byte[] According to Spec
                    //data could have a Preample of 0 - 7 bytes, search for Preample seperator
                    int dataIndex = Array.IndexOf(data, (byte)0xAA);
                    if (dataIndex == -1) //No Preamble seperator found, corrupt
                        throw new Exception("No Preamble seperator found, corrupt");
                    if (data.Length - dataIndex < 17) //Data Missing, corrupt
                        throw new Exception("Data Missing, corrupt");
                    if (dataIndex >= 8) //Data Missing, corrupt
                        throw new Exception("Data Missing, corrupt");

                    preambleCount = (byte)dataIndex;

                    //Calc Checksum
                    DeserializedChecksum1 = (ushort)(((data[dataIndex + 13] & data[dataIndex + 14]) << 8) |
                                            (data[dataIndex + 15] & data[dataIndex + 16]));

                    DeserializedChecksum2 = (ushort)data.Skip(dataIndex + 1).Take(12).Sum(c => (int)c);

                    ushort manId = (ushort)(((data[dataIndex + 1] & data[dataIndex + 2]) << 8) |
                                             (data[dataIndex + 3] & data[dataIndex + 4]));

                    uint devId = (uint)(((data[dataIndex + 5] & data[dataIndex + 6]) << 24) |
                                            ((data[dataIndex + 7] & data[dataIndex + 8]) << 16) |
                                            ((data[dataIndex + 9] & data[dataIndex + 10]) << 8) |
                                             (data[dataIndex + 11] & data[dataIndex + 12]));

                    SourceUID = new UID(manId, devId);
                    return;
                }
                else
                    throw new IndexOutOfRangeException($"{nameof(data)} Length is {data.Length} but has to be at least 26");
            }

            //Check startcode and sub-startcode
            if (data[0] != 0xCC || data[1] != 0x01)
                throw new Exception("Start-Code invalid");

            byte length = data[2];

            if (data.Length < length + 2)
                throw new Exception("DataLength not fit Length in Header");

            //Calc Checksum
            DeserializedChecksum1 = (ushort)((data[length] << 8) | data[length + 1]);
            DeserializedChecksum2 = (ushort)data.Take(length).Sum(c => (int)c);

            ushort manIdDest = (ushort)((data[3] << 8) | data[4]);
            uint devIdDest = (uint)((data[5] << 24) | (data[6] << 16) | (data[7] << 8) | data[8]);
            ushort manIdSource = (ushort)((data[9] << 8) | data[10]);
            uint devIdSource = (uint)((data[11] << 24) | (data[12] << 16) | (data[13] << 8) | data[14]);

            byte paramLength = data[23];

            SourceUID = new UID(manIdSource, devIdSource);
            DestUID = new UID(manIdDest, devIdDest);
            TransactionCounter = data[15];
            PortID_or_Responsetype = data[16];
            MessageCounter = data[17];
            SubDevice = new SubDevice((ushort)((data[18] << 8) | data[19]));
            Command = (ERDM_Command)data[20];
            Parameter = (ERDM_Parameter)((data[21] << 8) | data[22]);
            ParameterData = data.Skip(24).Take(paramLength).ToArray();
        }
        public RDMMessage(params ERDM_NackReason[] nackReasons)
        {
            if (nackReasons == null)
                return;
            if (nackReasons.Length == 0)
                return;

            nackReason = nackReasons;
            PortID_or_Responsetype = (byte)ERDM_ResponseType.NACK_REASON;
        }

        public byte MessageLength
        {
            get
            {
                byte b = 24; //The 1st 24 bytes are always the same
                b += PDL;
                return b;
            }
        }

        public UID SourceUID { get; set; }

        public UID DestUID { get; set; }

        public byte TransactionCounter { get; set; }

        private byte portID_or_Responsetype;
        public byte PortID_or_Responsetype
        {
            get
            {
                return portID_or_Responsetype;
            }
            set
            {
                if (value == portID_or_Responsetype)
                    return;

                valueCache = null;

                portID_or_Responsetype = value;
            }
        }

        public byte MessageCounter { get; set; }
        public byte? PreambleCount
        {
            get
            {
                if (this.Command != ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                    return null;

                if (this.Parameter != ERDM_Parameter.DISC_UNIQUE_BRANCH)
                    return null;

                return preambleCount;
            }
            set
            {
                if (this.Command != ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                    return;

                if (this.Parameter != ERDM_Parameter.DISC_UNIQUE_BRANCH)
                    return;

                if (preambleCount == value)
                    return;

                preambleCount = value;
            }
        }

        public SubDevice SubDevice { get; set; }

        private ERDM_Command command;
        public ERDM_Command Command
        {
            get
            {
                return command;
            }
            set
            {
                if (value == command)
                    return;
                valueCache = null;
                command = value;
            }
        }

        private ERDM_Parameter parameter;
        public ERDM_Parameter Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                if (value == parameter)
                    return;
                valueCache = null;
                parameter = value;
            }
        }

        private ERDM_NackReason[] nackReason = null;

        public ERDM_NackReason[] NackReason
        {
            get
            {
                if (ResponseType != ERDM_ResponseType.NACK_REASON)
                    return null;

                if (nackReason != null)
                    return nackReason;


                List<ERDM_NackReason> reasons = new List<ERDM_NackReason>();
                while (_parameterData.Length > 1)
                {
                    var res = Tools.DataToEnum<ERDM_NackReason>(ref _parameterData);
                    reasons.Add(res);
                }
                nackReason = reasons.ToArray();
                return nackReason;
            }
        }

        public byte PDL
        {
            get { return (byte)ParameterData.Length; }
        }

        public byte[] ParameterData
        {
            get { return _parameterData; }
            set
            {
                if (_parameterData == value)
                    return;

                valueCache = null;

                if (value == null)
                    _parameterData = Array.Empty<byte>();
                else
                {
                    if (value.Length > 255 - 24) throw new ArgumentException("ParameterData to large!");
                    _parameterData = value;
                }
            }
        }

        public ushort Checksum
        {
            get
            {
                int sum = 0xCC + 0x01; //Start Code and Sub Start Code
                sum += MessageLength;

                sum += SourceUID.ToBytes().Sum(c => (int)c);
                sum += DestUID.ToBytes().Sum(c => (int)c);

                sum += TransactionCounter;
                sum += PortID_or_Responsetype;
                sum += MessageCounter;
                sum += ((ushort)SubDevice) & 0xFF;
                sum += (((ushort)SubDevice) >> 8) & 0xFF;
                sum += (byte)Command;
                ushort para = (ushort)Parameter;
                sum += para & 0xFF;
                sum += (para >> 8) & 0xFF;
                sum += PDL;
                foreach (byte b in ParameterData)
                    sum += b;

                return (ushort)(sum % 0x10000);
            }
        }
        public ushort? DeserializedChecksum1
        {
            get;
            private set;
        }
        public ushort? DeserializedChecksum2
        {
            get;
            private set;
        }
        public bool ChecksumValid
        {
            get
            {
                return DeserializedChecksum1 == DeserializedChecksum2;
            }
        }

        public ERDM_ResponseType? ResponseType
        {
            get
            {
                if (this.Command.HasFlag(ERDM_Command.RESPONSE))
                    return (ERDM_ResponseType)PortID_or_Responsetype;
                return null;
            }
        }
        public byte? PortID
        {
            get
            {
                if (!this.Command.HasFlag(ERDM_Command.RESPONSE))
                    return PortID_or_Responsetype;
                return null;
            }
        }

        public bool IsAck
        {
            get
            {
                if (!ResponseType.HasValue)
                    return false;

                ERDM_ResponseType resp = ResponseType.Value;
                return resp == ERDM_ResponseType.ACK || resp == ERDM_ResponseType.ACK_OVERFLOW || resp == ERDM_ResponseType.ACK_TIMER;
            }
        }

        public byte[] BuildMessage()
        {
            List<byte> ret = null;
            switch (Command)
            {
                case ERDM_Command.DISCOVERY_COMMAND_RESPONSE when Parameter == ERDM_Parameter.DISC_UNIQUE_BRANCH:

                    byte preamble = preambleCount ?? 7;
                    if (preamble > 7) throw new ArgumentOutOfRangeException("preambleCount has to be within 0 and 7");
                    ret = new List<byte>(preamble + 17);
                    for (int i = 0; i < preamble; i++)
                        ret.Add(0xFE);
                    ret.Add(0xAA);
                    var uidBytes = ((byte[])SourceUID);
                    for (int b = 0; b < uidBytes.Length; b++)
                    {
                        ret.Add((byte)(uidBytes[b] | 0xAA));
                        ret.Add((byte)(uidBytes[b] | 0x55));
                    }
                    ushort cs = (ushort)ret.Skip(preamble + 1).Take(12).Sum(c => c);
                    ret.Add((byte)((cs >> 8) | 0xAA));
                    ret.Add((byte)((cs >> 8) | 0x55));
                    ret.Add((byte)((cs & 0xFF) | 0xAA));
                    ret.Add((byte)((cs & 0xFF) | 0x55));
                    return ret.ToArray();
                default:
                    ret = new List<byte>(MessageLength + 2)
                    {
                        0xcc, 0x01, MessageLength
                    };
                    ret.AddRange(DestUID.ToBytes());
                    ret.AddRange(SourceUID.ToBytes());
                    ret.Add(TransactionCounter);
                    ret.Add(PortID_or_Responsetype);
                    ret.Add(MessageCounter);
                    ret.Add((byte)(((ushort)SubDevice) >> 8));
                    ret.Add((byte)SubDevice);
                    ret.Add((byte)Command);

                    ushort para = (ushort)Parameter;
                    ret.Add((byte)(para >> 8));
                    ret.Add((byte)para);
                    ret.Add(PDL);
                    ret.AddRange(ParameterData);

                    ret.Add((byte)(Checksum >> 8));
                    ret.Add((byte)Checksum);

                    return ret.ToArray();
            }
        }

        private object valueCache = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2200:Erneut ausführen, um Stapeldetails beizubehalten", Justification = "<Ausstehend>")]
        public object Value
        {
            get
            {
                try
                {
                    if (this.ResponseType == ERDM_ResponseType.ACK_TIMER)
                        return valueCache = AcknowledgeTimer.FromPayloadData(this.ParameterData);
                    if (this.Parameter == ERDM_Parameter.DISC_UNIQUE_BRANCH && this.Command == ERDM_Command.DISCOVERY_COMMAND)
                        return valueCache = DiscUniqueBranchRequest.FromPayloadData(this.ParameterData);
                    if (this.Parameter == ERDM_Parameter.DISC_MUTE && this.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                        return valueCache = DiscMuteUnmuteResponse.FromPayloadData(this.ParameterData);
                    if (this.Parameter == ERDM_Parameter.DISC_UN_MUTE && this.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                        return valueCache = DiscMuteUnmuteResponse.FromPayloadData(this.ParameterData);

                    if (this.ParameterData?.Length == 0)
                        return valueCache = null;

                    ushort manufacturer = 0;
                    if (this.Command.HasFlag(ERDM_Command.RESPONSE))
                        manufacturer = SourceUID.ManufacturerID;
                    else
                        manufacturer = DestUID.ManufacturerID;

                    var parameterBag = new ParameterBag(this.Parameter, manufacturer);
                    var define = MetadataFactory.GetDefine(parameterBag);
                    var cd = Tools.ConvertCommandDublicateToCommand(Command);
                    Metadata.JSON.Command? cmd = null;
                    if ((this.ParameterData?.Length ?? 0) == 0)
                        return null;

                    if (define is null)
                        return null;

                    try
                    {
                        define.GetCommand(cd, out cmd);
                        if (cmd is null || !cmd.HasValue)
                            return null;
                        if(cmd.Value.GetIsEmpty())
                            return null;
                        return valueCache = MetadataFactory.ParseDataToPayload(define, cd, this.ParameterData).ParsedObject;
                    }
                    catch (Exception ex)
                    {
                        StringBuilder b = new StringBuilder(128);
                        b.AppendLine("Dest: " + DestUID);
                        b.AppendLine("Source: " + SourceUID);
                        b.AppendLine("Transaction: " + TransactionCounter);
                        b.AppendLine("MessageCounter: " + MessageCounter);
                        if (ResponseType.HasValue)
                            b.AppendLine("Responsetype: " + ResponseType);
                        if (PortID.HasValue)
                            b.AppendLine("PortID: " + PortID);
                        b.AppendLine("SubDevice: " + SubDevice);
                        b.AppendLine("Command: " + Command);
                        b.AppendLine("Parameter: " + ((ERDM_Parameter)Parameter).ToString());
                        Logger?.LogError(ex,$"Message: {b.ToString()}");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex);
                    throw ex;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(128);
            b.AppendLine("Dest: " + DestUID);
            b.AppendLine("Source: " + SourceUID);
            b.AppendLine("Transaction: " + TransactionCounter);
            b.AppendLine("MessageCounter: " + MessageCounter);
            if (ResponseType.HasValue)
                b.AppendLine("Responsetype: " + ResponseType);
            if (PortID.HasValue)
                b.AppendLine("PortID: " + PortID);
            b.AppendLine("SubDevice: " + SubDevice);
            b.AppendLine("Command: " + Command);
            b.AppendLine("Parameter: " + ((ERDM_Parameter)Parameter).ToString());
            if (
                Command == ERDM_Command.GET_COMMAND_RESPONSE ||
                Command == ERDM_Command.SET_COMMAND ||
                Command == ERDM_Command.SET_COMMAND_RESPONSE ||
                Command.HasFlag(ERDM_Command.DISCOVERY_COMMAND))
            {
                object val = null;
                try
                {
                    val = this.Value;
                }
                catch
                {

                }
                if (val != null)
                    b.AppendLine("Value: " + valueString());

                string valueString()
                {
                    string value;
                    if (val is Array array)
                    {
                        List<string> list = new List<string>();
                        foreach (var a in array)
                            list.Add(a.ToString());
                        value = string.Join("," + Environment.NewLine, list);
                    }
                    else if (val is string str)
                        value = $"\"{str}\"";
                    else
                        value = Value?.ToString() ?? "[NULL]";
                    if (value.Contains(Environment.NewLine) && !value.StartsWith(Environment.NewLine))
                    {
                        value = Environment.NewLine + value;
                        value = value.Replace(Environment.NewLine, Environment.NewLine + "\t");
                    }
                    return value;
                }
            }
            //Add More if required

            return b.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RDMMessage);
        }

        public bool Equals(RDMMessage other)
        {
            bool res = other is not null &&
                   MessageLength == other.MessageLength &&
                   SourceUID.Equals(other.SourceUID) &&
                   DestUID.Equals(other.DestUID) &&
                   TransactionCounter == other.TransactionCounter &&
                   PortID_or_Responsetype == other.PortID_or_Responsetype &&
                   MessageCounter == other.MessageCounter &&
                   SubDevice.Equals(other.SubDevice) &&
                   Command == other.Command &&
                   Parameter == other.Parameter &&
                   EqualityComparer<ERDM_NackReason[]>.Default.Equals(NackReason, other.NackReason) &&
                   PDL == other.PDL &&
                   ParameterData.SequenceEqual(other.ParameterData) &&
                   Checksum == other.Checksum &&
                   ChecksumValid == other.ChecksumValid &&
                   ResponseType == other.ResponseType &&
                   IsAck == other.IsAck &&
                   preambleCount == other.preambleCount;
            return res;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var bytes = BuildMessage();
                var result = 0;
                foreach (byte b in bytes)
                    result = (result * 31) ^ b;
                return result;
            }
        }
    }
}
