using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RDMSharp
{
    public sealed class RDMPersonalityModel : AbstractRDMCache
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly ushort ManufacturerID;
        public readonly EManufacturer Manufacturer;

        public readonly ushort DeviceModelID;

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


        private RDMPersonalityModel(UID uid, SubDevice sudevice, Func<RDMMessage, Task> sendRdmFunktion)
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRdmFunktion);

            SubDevice = sudevice;
            ManufacturerID = uid.ManufacturerID;
            Manufacturer = (EManufacturer)uid.ManufacturerID;
        }
        internal RDMPersonalityModel(UID uid, SubDevice sudevice, ushort deviceModelID, uint softwareVersionID, byte personalityID, Func<RDMMessage, Task> sendRdmFunktion)
            :this(uid, sudevice, sendRdmFunktion)
        {
            DeviceModelID = deviceModelID;
            SoftwareVersionID = softwareVersionID;
            PersonalityID = personalityID;
        }
        internal RDMPersonalityModel(UID uid, SubDevice sudevice, ushort majorPersonalityID, ushort minorPersonalityID, Func<RDMMessage, Task> sendRdmFunktion)
            : this(uid, sudevice, sendRdmFunktion)
        {
            MajorPersonalityID = majorPersonalityID;
            MinorPersonalityID = minorPersonalityID;
        }
        internal void RDMDeviceModel_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
        {
            var cache = sender as AbstractRDMCache;
            var bag = new ParameterDataCacheBag(e.Parameter, e.Index);
            cache.parameterValuesDataTreeBranch.TryGetValue(bag, out var value);
            updateParameterValuesDataTreeBranch(bag, value);

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
                getOrCreate(Convert.ToUInt16(e.Index)).UpdateSlotDescription(slotDescription);

            Slot getOrCreate(ushort id)
            {
                if (!slots.TryGetValue(id, out Slot slot1))
                {
                    slot1 = new Slot(id);
                    slots.TryAdd(id, slot1);
                }
                return slot1;
            }
        }

        internal AsyncRDMRequestHelper GetAsyncRDMRequestHelper()
        {
            return asyncRDMRequestHelper;
        }
        internal void DisposeAsyncRDMRequestHelper()
        {
            asyncRDMRequestHelper?.Dispose();
            asyncRDMRequestHelper = null;
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