using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class RDMMessage
    {
        private byte[] _parameterData = new byte[0];

        public RDMMessage()
        {

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

        public ERDM_ResponseType ResponseType
        {
            get
            {
                return (ERDM_ResponseType)PortID_or_Responsetype;
            }
        }

        public bool IsAck
        {
            get
            {
                ERDM_ResponseType resp = ResponseType;
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
                return RDMParameterWrapperCatalogueManager.GetInstance().ParameterDataObjectFromMessage(this);
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(128);
            b.AppendLine("DestUID: " + DestUID);
            b.AppendLine("SourceUID: " + SourceUID);
            b.AppendLine("MessageCounter: " + MessageCounter);
            b.AppendLine("PortID / Responsetype: " + ResponseType);
            b.AppendLine("SubDevice: " + SubDevice);
            b.AppendLine("Command: " + Command);
            b.Append("Parameter: " + ((ERDM_Parameter)Parameter).ToString());
            //Add More if required

            return b.ToString();
        }
    }
}
