using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class LanguageCapabilitiesParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string[]>
    {
        public LanguageCapabilitiesParameterWrapper() : base(ERDM_Parameter.LANGUAGE_CAPABILITIES)
        {
        }
        public override string Name => "Language Capabilities";
        public override string Description => 
            "This parameter is used to identify languages that the device supports for using the Language" +
            "parameter. The response contains a packed message of 2 character Language Codes as defined" +
            "by ISO 639 - 1. International Standard ISO 639 - 1, Code for the representation of names of" +
            "languages - Part 1: Alpha 2 code.";
        
        protected override string[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<string> languages = new List<string>();
            int pdl = 2;
            while (parameterData.Length >= pdl)
            {
                var bytes = parameterData.Take(pdl).ToArray();
                languages.Add(Tools.DataToString(ref bytes));
                parameterData = parameterData.Skip(pdl).ToArray();
            }
            return languages.ToArray();
        }

        protected override byte[] getResponseValueToParameterData(string[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
                bytes.AddRange(Tools.ValueToData(item, trim: 2));

            return bytes.ToArray();
        }
    }
}