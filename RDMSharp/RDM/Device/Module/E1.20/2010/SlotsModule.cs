using RDMSharp.PayloadObject;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module;

public sealed class SlotsModule : AbstractModule
{
    private const string _moduleName = "Slots";
    private const string _moduleDisplayName = "Slots";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.SLOT_INFO,
        ERDM_Parameter.SLOT_DESCRIPTION,
        ERDM_Parameter.DEFAULT_SLOT_VALUE
    };

    public override string DisplayName => _moduleDisplayName;

    public IPersonality CurrentPersonality
    {
        get
        {
            if (ParentGeneratedDevice is not null)
                return dmxPersonalityModule.CurrentPersonality;
            else if (ParentRemoteDevice is not null)
                return ParentRemoteDevice.PersonalityModel.Personality;
            return null;
        }
    }

    public IReadOnlyDictionary<ushort, Slot> Slots
    {
        get
        {
            return CurrentPersonality?.Slots;
        }
    }

    private DMX_PersonalityModule dmxPersonalityModule;

    public SlotsModule() : base(
        _moduleName,
        _moduleParameters)
    {
    }
    public SlotsModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        dmxPersonalityModule = device.Modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
        dmxPersonalityModule.PropertyChanged += DmxPersonalityModule_PropertyChanged;
        updateParameterValues();
    }
    protected override void OnRemoteParentDeviceChanged(AbstractRemoteRDMDevice device)
    {
        device.PropertyChanged += DmxPersonalityModule_PropertyChanged;
    }

    private void DmxPersonalityModule_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(DMX_PersonalityModule.CurrentPersonality))
            return;

        updateParameterValues();
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.DMX_PERSONALITY:
                updateParameterValues();
                break;
        }
    }
    private void updateParameterValues()
    {
        if (dmxPersonalityModule is null)
        {
            if (ParentRemoteDevice is not null)
            {
                dmxPersonalityModule = ParentRemoteDevice.Modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
                dmxPersonalityModule.PropertyChanged += DmxPersonalityModule_PropertyChanged;
            }
        }
        if (ParentRemoteDevice is not null)
        {
            var pers = (RemotePersonality)CurrentPersonality;
            if (!pers.AllDataPulled)
                pers.PropertyChanged += Pers_PropertyChanged;
            else
                OnPropertyChanged(nameof(Slots));
            return;
        }

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
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.SLOT_INFO, slotInfos);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.SLOT_DESCRIPTION, slotDesc);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DEFAULT_SLOT_VALUE, slotDefault);
        OnPropertyChanged(nameof(Slots));
    }

    private void Pers_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var pers = (RemotePersonality)sender;
        pers.PropertyChanged -= Pers_PropertyChanged;

        if (e.PropertyName == nameof(pers.AllDataPulled))
            OnPropertyChanged(nameof(Slots));

    }
}