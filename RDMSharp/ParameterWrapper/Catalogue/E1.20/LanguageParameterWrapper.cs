using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class LanguageParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<string, string>
    {
        public LanguageParameterWrapper() : base(ERDM_Parameter.LANGUAGE)
        {
        }
        public override string Name => "Language";
        public override string Description =>
            "This parameter is used to change the language of the messages from the device. Supported " +
            "languages of the device can be determined by the Language Capabilities.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(string iso639_1_LanguageCode)
        {
            if (string.IsNullOrEmpty(iso639_1_LanguageCode) || iso639_1_LanguageCode.Length != 2)
                throw new ArgumentException("Language code has to be International Standard ISO 639 - 1");

            return Tools.ValueToData(iso639_1_LanguageCode);
        }
    }
}