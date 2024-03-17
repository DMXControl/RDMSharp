using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RDMSharp
{
    public class Slot : INotifyPropertyChanged, IEquatable<Slot>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly ushort SlotId;

        private ERDM_SlotType type;
        public ERDM_SlotType Type
        {
            get { return type; }
            private set
            {
                if (type == value)
                    return;

                type = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }

        private ERDM_SlotCategory category;
        public ERDM_SlotCategory Category
        {
            get { return category; }
            private set
            {
                if (category == value)
                    return;

                category = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Category)));
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            private set
            {
                if (description == value)
                    return;

                description = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        private byte defaultValue;
        public byte DefaultValue
        {
            get { return defaultValue; }
            private set
            {
                if (defaultValue == value)
                    return;

                defaultValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DefaultValue)));
            }
        }

        public Slot(ushort slotId)
        {
            this.SlotId = slotId;
        }
        public Slot(ushort slotId, ERDM_SlotCategory category, ERDM_SlotType type, string description = null, byte defaultValue = 0) : this(slotId)
        {
            this.Category = category;
            this.Type = type;
            this.Description = description;
            this.DefaultValue = defaultValue;
        }
        public Slot(ushort slotId, ERDM_SlotCategory category, string description = null, byte defaultValue = 0) : this(slotId, category, ERDM_SlotType.PRIMARY, description: description, defaultValue: defaultValue)
        {
        }

        public void UpdateSlotInfo(RDMSlotInfo slotInfo)
        {
            if (this.SlotId != slotInfo.SlotOffset)
                throw new InvalidOperationException($"The given {nameof(slotInfo)} has not the expected id of {this.SlotId} but {slotInfo.SlotOffset}");

            this.Type = slotInfo.SlotType;
            this.Category = slotInfo.SlotLabelId;
        }
        public void UpdateSlotDescription(RDMSlotDescription slotDescription)
        {
            if (this.SlotId != slotDescription.SlotId)
                throw new InvalidOperationException($"The given {nameof(slotDescription)} has not the expected id of {this.SlotId} but {slotDescription.SlotId}");

            this.Description = slotDescription.Description;
        }
        public void UpdateSlotDefaultValue(RDMDefaultSlotValue defaultSlotValue)
        {
            if (this.SlotId != defaultSlotValue.SlotOffset)
                throw new InvalidOperationException($"The given {nameof(defaultSlotValue)} has not the expected id of {this.SlotId} but {defaultSlotValue.SlotOffset}");

            this.DefaultValue = defaultSlotValue.DefaultSlotValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Slot: {this.SlotId}");
            sb.AppendLine($"Category: {this.Category}");
            sb.AppendLine($"Type: {this.Type}");
            sb.AppendLine($"Description: {this.Description}");
            sb.AppendLine($"DefaultValue: {this.DefaultValue}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Slot);
        }

        public bool Equals(Slot other)
        {
            return other is not null &&
                   SlotId == other.SlotId &&
                   Type == other.Type &&
                   Category == other.Category &&
                   Description == other.Description &&
                   DefaultValue == other.DefaultValue;
        }

        public override int GetHashCode()
        {
#if !NETSTANDARD
            return HashCode.Combine(SlotId, Type, Category, Description, DefaultValue);
#else
            int hashCode = 1916557166;
            hashCode = hashCode * -1521134295 + SlotId.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Category.GetHashCode();
            hashCode = hashCode * -1521134295 + Description.GetHashCode();
            hashCode = hashCode * -1521134295 + DefaultValue.GetHashCode();
            return hashCode;
#endif
        }

        public static bool operator ==(Slot left, Slot right)
        {
            return EqualityComparer<Slot>.Default.Equals(left, right);
        }

        public static bool operator !=(Slot left, Slot right)
        {
            return !(left == right);
        }
    }
}