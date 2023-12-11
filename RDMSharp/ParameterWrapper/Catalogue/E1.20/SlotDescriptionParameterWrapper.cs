using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SlotDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<ushort, RDMSlotDescription>, IRDMDescriptionParameterWrapper
    {
        public SlotDescriptionParameterWrapper() : base(ERDM_Parameter.SLOT_DESCRIPTION)
        {
        }
        public override string Name => "Slot Description";
        public override string Description => "This parameter is used for requesting an ASCII text description for DMX512 slot offsets.";

        public ERDM_Parameter ValueParameterID => ERDM_Parameter.DEFAULT_SLOT_VALUE;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.DEVICE_INFO, ERDM_Parameter.SLOT_INFO, ERDM_Parameter.DEFAULT_SLOT_VALUE };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort slotID)
        {
            return Tools.ValueToData(slotID);
        }

        protected override RDMSlotDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSlotDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMSlotDescription value)
        {
            return value.ToPayloadData();
        }
        public override RequestRange<ushort> GetRequestRange(object value)
        {
            if (value is RDMDeviceInfo deviceInfo)
                return new RequestRange<ushort>(0, (ushort)(deviceInfo.Dmx512Footprint));
            else if (value is RDMDMXPersonalityDescription personalityDescription)
                return new RequestRange<ushort>(0, (ushort)(personalityDescription.Slots));
            else if (value == null)
                return new RequestRange<ushort>(0, 511);

            ushort max = 0;
            if (value is IEnumerable<object> @enumerable)
            {
                var slotInfos = enumerable.OfType<RDMSlotInfo>();
                var defaultSlotValues = enumerable.OfType<RDMDefaultSlotValue>();
                if (slotInfos.Count() != 0)
                    max = slotInfos.Max(i => i.SlotOffset);
                else if (defaultSlotValues.Count() != 0)
                    max = defaultSlotValues.Max(d => d.SlotOffset);

                return new RequestRange<ushort>(0, max);
            }

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}