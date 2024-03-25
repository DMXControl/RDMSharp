using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class PresetStatusParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<ushort, RDMPresetStatus, RDMPresetStatus>
    {
        public PresetStatusParameterWrapper() : base(ERDM_Parameter.PRESET_STATUS)
        {
        }
        public override string Name => "Preset Status";
        public override string Description =>
            "This parameter is used to determine if a preset scene is programmed and to retrieve the timing information stored with that scene (Get). " +
            "It also allows a preset scene to be cleared or to change the timing information stored with that scene (Set).";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.PRESET_INFO };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort sceneId)
        {
            return Tools.ValueToData(sceneId);
        }

        protected override RDMPresetStatus getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetStatus.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMPresetStatus presetStatus)
        {
            return presetStatus.ToPayloadData();
        }

        protected override RDMPresetStatus setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetStatus.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMPresetStatus presetStatus)
        {
            return presetStatus.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            if (value is RDMPresetInfo presetInfo)
                return new RequestRange<ushort>(0x0001, presetInfo.MaximumSceneNumber);
            else if (value == null)
                return new RequestRange<ushort>(0x0001, 0xFFFE);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}