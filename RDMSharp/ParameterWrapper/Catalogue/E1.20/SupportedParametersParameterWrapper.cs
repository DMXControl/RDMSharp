using System.Collections.Generic;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SupportedParametersParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<ERDM_Parameter[]>, IRDMBlueprintParameterWrapper
    {
        public SupportedParametersParameterWrapper() : base(ERDM_Parameter.SUPPORTED_PARAMETERS)
        {
        }
        public override string Name => "Supported Parameters";
        public override string Description => "The Supported Parameters message is used to retrieve a packed list of supported PIDs.";

        protected override ERDM_Parameter[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<ERDM_Parameter> parameterIDs = new List<ERDM_Parameter>();
            while (parameterData.Length >= 2)
                parameterIDs.Add((ERDM_Parameter)Tools.DataToUShort(ref parameterData));

            return parameterIDs.ToArray();
        }
        protected override byte[] getResponseValueToParameterData(ERDM_Parameter[] supportedParameters)
        {
            List<byte> bytes = new List<byte>();
            foreach (var parameter in supportedParameters)
                bytes.AddRange(Tools.ValueToData(parameter));

            return bytes.ToArray();
        }
    }
}