using Microsoft.Extensions.Logging;
using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class RDMMessage : IEquatable<RDMMessage>
    {
        private static ILogger Logger = null;
        private byte[] _parameterData = new byte[0];

        public RDMMessage()
        {

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
        internal RDMMessage(bool checksumValid)
        {
            ChecksumValid = checksumValid;
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

        public RDMUID SourceUID { get; set; }

        public RDMUID DestUID { get; set; }

        public byte TransactionCounter { get; set; }

        public byte PortID_or_Responsetype { get; set; }

        public byte MessageCounter { get; set; }

        public SubDevice SubDevice { get; set; }

        public ERDM_Command Command { get; set; }

        public ERDM_Parameter Parameter { get; set; }

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
                if (value == null)
                    _parameterData = new byte[0];
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
        public bool ChecksumValid { get; private set; }

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
            List<byte> ret = new List<byte>(MessageLength + 2)
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

            ushort cs = Checksum;
            ret.Add((byte)(cs >> 8));
            ret.Add((byte)cs);

            return ret.ToArray();
        }

        public object Value
        {
            get
            {
                try
                {
                    if (this.ResponseType == ERDM_ResponseType.ACK_TIMER)
                        return AcknowledgeTimer.FromPayloadData(this.ParameterData);

                    return RDMParameterWrapperCatalogueManager.GetInstance().ParameterDataObjectFromMessage(this);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Logger?.LogError(string.Empty, ex);
#endif
                    return null;
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
                Command == ERDM_Command.SET_COMMAND_RESPONSE)
            {
                var pm = RDMParameterWrapperCatalogueManager.GetInstance().GetRDMParameterWrapperByID(Parameter);
                switch (Command)
                {
                    case ERDM_Command.SET_COMMAND_RESPONSE
                        when !(pm is IRDMSetParameterWrapperWithEmptySetResponse):
                        b.AppendLine("Value: " + valueString());
                        break;
                    case ERDM_Command.GET_COMMAND_RESPONSE
                        when !(pm is IRDMGetParameterWrapperWithEmptyGetResponse):
                        b.AppendLine("Value: " + valueString());
                        break;
                    case ERDM_Command.SET_COMMAND
                        when !(pm is IRDMSetParameterWrapperWithEmptySetRequest):
                        b.AppendLine("Value: " + valueString());
                        break;
                }
                string valueString()
                {
                    string value = null;
                    if (Value is Array array)
                    {
                        List<string> list = new List<string>();
                        foreach (var a in array)
                            list.Add(a.ToString());
                        value = string.Join("," + Environment.NewLine, list);
                    }
                    else if (Value is string str)
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
            return other is not null &&
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
                   EqualityComparer<byte[]>.Default.Equals(ParameterData, other.ParameterData) &&
                   Checksum == other.Checksum &&
                   ResponseType == other.ResponseType &&
                   IsAck == other.IsAck &&
                   EqualityComparer<object>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            int hashCode = 1518318531;
            hashCode = hashCode * -1521134295 + MessageLength.GetHashCode();
            hashCode = hashCode * -1521134295 + SourceUID.GetHashCode();
            hashCode = hashCode * -1521134295 + DestUID.GetHashCode();
            hashCode = hashCode * -1521134295 + TransactionCounter.GetHashCode();
            hashCode = hashCode * -1521134295 + PortID_or_Responsetype.GetHashCode();
            hashCode = hashCode * -1521134295 + MessageCounter.GetHashCode();
            hashCode = hashCode * -1521134295 + SubDevice.GetHashCode();
            hashCode = hashCode * -1521134295 + Command.GetHashCode();
            hashCode = hashCode * -1521134295 + Parameter.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ERDM_NackReason[]>.Default.GetHashCode(NackReason);
            hashCode = hashCode * -1521134295 + PDL.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(ParameterData);
            hashCode = hashCode * -1521134295 + Checksum.GetHashCode();
            hashCode = hashCode * -1521134295 + IsAck.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}
