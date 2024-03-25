using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class LockStateDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMLockStateDescription>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public LockStateDescriptionParameterWrapper() : base(ERDM_Parameter.LOCK_STATE_DESCRIPTION)
        {
        }
        public override string Name => "Lock State Description";
        public override string Description =>
            "This parameter is used to get a descriptive ASCII text label for a given lock state. " +
            "The label may be up to 32 characters.";
        public ERDM_Parameter ValueParameterID => ERDM_Parameter.LOCK_STATE;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.LOCK_STATE };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte lockStateId)
        {
            return Tools.ValueToData(lockStateId);
        }

        protected override RDMLockStateDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMLockStateDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMLockStateDescription value)
        {
            return value.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            if (value is GetLockStateResponse lockState)
                return new RequestRange<byte>(1, (byte)(lockState.Count));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}