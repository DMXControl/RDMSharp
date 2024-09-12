using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class StatusMessageParameterWrapper : AbstractRDMGetParameterWrapperRanged<ERDM_Status, RDMStatusMessage[]>
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public StatusMessageParameterWrapper() : base(ERDM_Parameter.STATUS_MESSAGES)
        {
        }
        public override string Name => "Status Messages";
        public override string Description => "This parameter is used to collect Status or Error information from a device.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ERDM_Status getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_Status>(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ERDM_Status status)
        {
            return Tools.ValueToData(status);
        }

        protected override RDMStatusMessage[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<RDMStatusMessage> statusMessages = new List<RDMStatusMessage>();
            int pdl = 9;
            while (parameterData.Length >= pdl)
            {
                var bytes = parameterData.Take(pdl).ToArray();
                statusMessages.Add(RDMStatusMessage.FromPayloadData(bytes));
                parameterData = parameterData.Skip(pdl).ToArray();
            }
            return statusMessages.ToArray();
        }

        protected override byte[] getResponseValueToParameterData(RDMStatusMessage[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
                bytes.AddRange(item.ToPayloadData());

            return bytes.ToArray();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return new RequestRange<ERDM_Status>(0, (ERDM_Status)0x04);
        }
    }
}