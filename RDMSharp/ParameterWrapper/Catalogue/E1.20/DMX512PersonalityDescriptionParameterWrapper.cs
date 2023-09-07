using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512PersonalityDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMDMXPersonalityDescription>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public DMX512PersonalityDescriptionParameterWrapper() : base(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION)
        {
        }
        public override string Name => "DMX512 Personality Description";
        public override string Description => "This parameter is used to get a descriptive ASCII text label for a given DMX512 Personality. The label may be up to 32 characters.";

        public ERDM_Parameter ValueParameterID => ERDM_Parameter.DMX_PERSONALITY;


        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.DMX_PERSONALITY };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(byte personalityID)
        {
            return Tools.ValueToData(personalityID);
        }
        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override RDMDMXPersonalityDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDMXPersonalityDescription.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMDMXPersonalityDescription personalityDescription)
        {
            return personalityDescription.ToPayloadData();
        }

        public override RequestRange<byte> GetRequestRange(object value)
        {
            if (value is RDMDMXPersonality perso)
                return new RequestRange<byte>((byte)perso.MinIndex, (byte)perso.Count);
            if (value is RDMDeviceInfo deviceInfo)
                return new RequestRange<byte>(1, (byte)deviceInfo.Dmx512NumberOfPersonalities);
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue - 1);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}