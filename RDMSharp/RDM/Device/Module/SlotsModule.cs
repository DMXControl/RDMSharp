using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class SlotsModule : AbstractModule
    {
        public IReadOnlyDictionary<ushort, Slot> Slots
        {
            get
            {
                return dmxPersonalityModule?.Personalities?.FirstOrDefault(p=>p.ID == dmxPersonalityModule.CurrentPersonality)?.Slots;
            }
        }

        private DMX_PersonalityModule dmxPersonalityModule;

        public SlotsModule() : base(
            "Slots",
            ERDM_Parameter.SLOT_INFO,
            ERDM_Parameter.SLOT_DESCRIPTION,
            ERDM_Parameter.DEFAULT_SLOT_VALUE)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            dmxPersonalityModule = device.Modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
            dmxPersonalityModule.PropertyChanged += DmxPersonalityModule_PropertyChanged;
            updateParameterValues();
            OnPropertyChanged(nameof(Slots));
        }

        private void DmxPersonalityModule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(DMX_PersonalityModule.CurrentPersonality))
                return;

            updateParameterValues();
            OnPropertyChanged(nameof(Slots));
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DMX_PERSONALITY:
                    updateParameterValues();
                    OnPropertyChanged(nameof(Slots));
                    break;
            }
        }
        private void updateParameterValues()
        {
            var slots = this.Slots;
            var slotsCount = slots.Count;
            var slotInfos = new RDMSlotInfo[slotsCount];
            var slotDesc = new ConcurrentDictionary<object, object>();
            var slotDefault = new RDMDefaultSlotValue[slotsCount];
            foreach (var s in slots)
            {
                Slot slot = s.Value;
                slotInfos[slot.SlotId] = new RDMSlotInfo(slot.SlotId, slot.Type, slot.Category);
                slotDesc.TryAdd(slot.SlotId, new RDMSlotDescription(slot.SlotId, slot.Description));
                slotDefault[slot.SlotId] = new RDMDefaultSlotValue(slot.SlotId, slot.DefaultValue);
            }
            ParentDevice.setParameterValue(ERDM_Parameter.SLOT_INFO, slotInfos);
            ParentDevice.setParameterValue(ERDM_Parameter.SLOT_DESCRIPTION, slotDesc);
            ParentDevice.setParameterValue(ERDM_Parameter.DEFAULT_SLOT_VALUE, slotDefault);
        }
    }
}