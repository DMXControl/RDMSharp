namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceUnitNumberParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<uint, uint>
    {
        public DeviceUnitNumberParameterWrapper() : base(ERDM_Parameter.DEVICE_UNIT_NUMBER)
        {
        }
        public override string Name => "Device Unit Number";
        public override string Description => "A DMX512 address is only unique across a single DMX connection, and may change if certain parameters, such as personality, are altered.\r\nThe Device Unit Number parameter attempts to apply a unique identifier by allowing a Controller to store a 32-bit unit number in the Responder.\r\nUnit numbers are often used in lighting to identify individual fixtures within a rig. It is beneficial for all Controllers in a system to see a common unit number in order to allow the user to identify specific fixtures. Because of this, unit numbers should be unique across the entire rig.";

        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }

        protected override uint setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(uint state)
        {
            return Tools.ValueToData(state);
        }
    }
}