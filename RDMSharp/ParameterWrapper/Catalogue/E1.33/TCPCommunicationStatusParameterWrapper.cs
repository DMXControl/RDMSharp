using System.Collections.Generic;

namespace RDMSharp.ParameterWrapper
{
    public sealed class TCPCommunicationStatusParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<TCPCommsEntry, string>
    {
        public TCPCommunicationStatusParameterWrapper() : base(ERDM_Parameter.TCP_COMMS_STATUS)
        {
        }
        public override string Name => "TCP Communication Status";
        public override string Description => 
            "This parameter is used to collect information that may be useful in analyzing the performance and " +
            "system behavior for the active TCP communication channels.";

        protected override TCPCommsEntry getResponseParameterDataToValue(byte[] parameterData)
        {
            return TCPCommsEntry.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(TCPCommsEntry value)
        {
            return value.ToPayloadData();
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData, 63).Replace("\u0000", "");
        }

        protected override byte[] setRequestValueToParameterData(string scopeString)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Tools.ValueToData(scopeString, 62));
            while (bytes.Count < 63)
                bytes.Add(0);

            return bytes.ToArray();
        }
    }
}