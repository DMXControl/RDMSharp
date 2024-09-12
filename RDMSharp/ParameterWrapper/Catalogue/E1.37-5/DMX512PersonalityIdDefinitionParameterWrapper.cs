using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512PersonalityIdDefinitionParameterWrapper : AbstractRDMGetParameterWrapperRanged<byte, RDMPersonalityId>, IRDMBlueprintParameterWrapper
    {
        public DMX512PersonalityIdDefinitionParameterWrapper() : base(ERDM_Parameter.DMX_PERSONALITY_ID)
        {
        }
        public override string Name => "DMX512 Personality ID";
        public override string Description => "A DMX512 Personality uniquely identifies a personality across different models and software versions. This allows Controllers to select the correct personality profile, even if the personality indices have changed due to additions / removals. Equivalent personalities across products from a manufacturer are required to have the same personality identifier.Any change to the DMX512 behavior of a Responder shall result in a new DMX_PERSONALITY_ID(see the major / minor descriptions below). Any manufacturers with Responders that support this parameter should publish the DMX personality identifier in the personality documentation on their website.If a Responder supports PRODUCT_URL, manufacturers are strongly encouraged to link to the personality documentation from the URL returned with PRODUCT_URL. A personality of zero means that the personality is not defined. This may mean that a custom personality is in use. For both the major and minor numbers, the values 0x0000 to 0x7FFF are manufacturer-defined, values 0x8000 to 0xFFFE are reserved. A value of 0xFFFF in both the Major Personality ID and Minor Personality ID fields indicates the personality is user-defined.For example, a media server may allow the user to define a DMX512 profile. A value of 0xFFFF in only one of the fields, but not both, is undefined, and shall be ignored.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.DEVICE_INFO };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(byte parameterID)
        {
            return Tools.ValueToData(parameterID);
        }
        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override RDMPersonalityId getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMPersonalityId.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMPersonalityId personalityId)
        {
            return personalityId.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            return DMX512PersonalityIdDefinitionParameterWrapper.GetRequestRangeInternal(value);
        }
        internal static IRequestRange GetRequestRangeInternal(object value)
        {
            if (value is RDMDeviceInfo deviceInfo)
                return new RequestRange<byte>(1, (byte)(deviceInfo.Dmx512NumberOfPersonalities));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType()}");
        }
    }
}