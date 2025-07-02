using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp
{
    public sealed class RDMPersonalityModel : AbstractRDMCache
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Slot> SlotAdded;
        public event EventHandler Initialized;

        public readonly ushort ManufacturerID;
        public readonly EManufacturer Manufacturer;

        public readonly ushort DeviceModelID;
        public bool IsInitialized { get; private set; } = false;
        public bool IsInitializing { get; private set; } = false;

        //Id given from PID DMX_PERSONALITY = 0x00E0 & DMX_PERSONALITY_DESCRIPTION E1.20
        public byte PersonalityID { get; private set; }
        public uint SoftwareVersionID { get; private set; }

        //Id given from PID DMX_PERSONALITY_ID E1.37-5 - 2024
        public ushort? MajorPersonalityID { get; private set; }
        public ushort? MinorPersonalityID { get; private set; }

        private SubDevice subDevice;
        public SubDevice SubDevice
        {
            get
            {
                return subDevice;
            }
            private set
            {
                if (subDevice == value)
                    return;
                subDevice = value;
                PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(SubDevice)));
            }
        }

        private ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();
        public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();
        public readonly IReadOnlyCollection<ERDM_Parameter> SupportedPersonalityBlueprintParameters;

        private UID currentUsedUid;

        public string Description { get; private set; }
        public ushort SlotCount { get; private set; }

        internal RDMPersonalityModel(IRDMRemoteDevice remoteRDMDevice, byte personalityId)
        {
            currentUsedUid = remoteRDMDevice.UID;
            SubDevice = remoteRDMDevice.Subdevice;
            ManufacturerID = remoteRDMDevice.UID.ManufacturerID;
            Manufacturer = (EManufacturer)ManufacturerID;
            DeviceModelID = remoteRDMDevice.DeviceInfo.DeviceModelId;
            SoftwareVersionID = remoteRDMDevice.DeviceInfo.SoftwareVersionId;
            PersonalityID = personalityId;
            this.ParameterValueAdded += RDMDeviceModel_ParameterValueAdded;
            if (remoteRDMDevice is AbstractRemoteRDMDevice rd)
            {
                SupportedPersonalityBlueprintParameters = rd.DeviceModel.SupportedPersonalityBlueprintParameters;
                if (remoteRDMDevice.ParameterValues.TryGetValue(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, out object obj) && obj is ConcurrentDictionary<object, object> dict)
                {
                    if (dict.TryGetValue(personalityId, out object disc) && disc is RDMDMXPersonalityDescription personalityDescription)
                    {
                        this.Description = personalityDescription.Description;
                        SlotCount = personalityDescription.Slots;
                        var rpl=DataTreeBranch.FromObject(personalityDescription, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION);
                        updateParameterValuesDependeciePropertyBag(ERDM_Parameter.SLOT_DESCRIPTION, rpl);
                    }
                }
            }
            _ = requestPersonalityBlueprintParameters();
        }
        private SemaphoreSlim initializeSemaphoreSlim = new SemaphoreSlim(1);
        internal async Task Initialize()
        {
            if (IsInitialized)
                return;
            if (initializeSemaphoreSlim.CurrentCount == 0)
                return;
            IsInitializing = true;

            await initializeSemaphoreSlim.WaitAsync();
            try
            {
                await requestPersonalityBlueprintParameters();

                IsInitialized = true;
            }
            finally
            {
                initializeSemaphoreSlim.Release();
                IsInitializing = false;
            }
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        internal async Task requestPersonalityBlueprintParameters()
        {
            try
            {
                foreach (ERDM_Parameter parameter in this.SupportedPersonalityBlueprintParameters)
                {
                    ParameterBag parameterBag = new ParameterBag(parameter, this.ManufacturerID, this.DeviceModelID, this.SoftwareVersionID);
                    var define = MetadataFactory.GetDefine(parameterBag);
                    if (define.GetRequest.HasValue)
                    {
                        if (define.GetRequest.Value.GetIsEmpty())
                            await requestGetParameterWithEmptyPayload(parameterBag, define, currentUsedUid, SubDevice);
                        else
                            await requestGetParameterWithPayload(parameterBag, define, currentUsedUid, SubDevice);
                    }
                    await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex);
            }
        }
        internal void RDMDeviceModel_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
        {
            if (!Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS.Contains(e.Parameter))
                return;

            //var cache = sender as AbstractRDMCache;
            //var bag = new ParameterDataCacheBag(e.Parameter, e.Index);
            //cache.parameterValuesDataTreeBranch.TryGetValue(bag, out var value);
            //updateParameterValuesDataTreeBranch(bag, value);

            if (e.Value is RDMSlotInfo[] slotInfos)
            {
                foreach (var slotInfo in slotInfos)
                    getOrCreate(slotInfo.SlotOffset).UpdateSlotInfo(slotInfo);
                return;
            }
            if (e.Value is RDMDefaultSlotValue[] defaultSlotValues)
            {
                foreach (var defaultSlotValue in defaultSlotValues)
                    getOrCreate(defaultSlotValue.SlotOffset).UpdateSlotDefaultValue(defaultSlotValue);
                return;
            }
            if (e.Value is RDMSlotDescription slotDescription)
            {
                var slot = getOrCreate(Convert.ToUInt16(e.Index));
                if (slot.SlotId == slotDescription.SlotId)
                    slot.UpdateSlotDescription(slotDescription);
            }

            Slot getOrCreate(ushort id)
            {
                if (!slots.TryGetValue(id, out Slot slot1))
                {
                    slot1 = new Slot(id);
                    if (slots.TryAdd(id, slot1))
                        SlotAdded?.InvokeFailSafe(this, slot1);
                }
                return slot1;
            }
        }

        public bool IsModelOf(UID uid, ushort deviceModelID, uint softwareVersionID, byte personalityID)
        {
            if (this.ManufacturerID != uid.ManufacturerID)
                return false;
            if (this.DeviceModelID != deviceModelID)
                return false;
            if (this.SoftwareVersionID != softwareVersionID)
                return false;
            if (this.PersonalityID != personalityID)
                return false;

            return true;
        }
        public bool IsModelOf(UID uid, ushort majorPersonalityID, ushort minorPersonalityID)
        {
            if (this.ManufacturerID != uid.ManufacturerID)
                return false;
            if (this.MajorPersonalityID != majorPersonalityID)
                return false;
            if (this.MinorPersonalityID != minorPersonalityID)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"[{PersonalityID}] {Enum.GetName(typeof(EManufacturer), Manufacturer)}";
        }
    }
}