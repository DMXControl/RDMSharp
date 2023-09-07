using System;
using System.ComponentModel;
using System.Text;

namespace RDMSharp
{
    [Serializable]
    public class Slot : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly ushort SlotId;

        private ERDM_SlotType type;
        public ERDM_SlotType Type
        {
            get { return type; }
            set
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
            set
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
            set
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
            set
            {
                if (defaultValue == value)
                    return;

                defaultValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DefaultValue)));
            }
        }

        public string DisplayName
        {
            get => $"({this.SlotId}) - {this.Category} {this.Type}";
        }

        public Slot(ushort slotId)
        {
            this.SlotId = slotId;
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
            if (obj is Slot other)
            {
                if (this.SlotId != other.SlotId)
                    return false;
                if (this.Type != other.Type)
                    return false;
                if (this.Category != other.Category)
                    return false;
                if (this.DefaultValue != other.DefaultValue)
                    return false;
                if (!string.Equals(this.Description, other.Description))
                    return false;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int hashCode = this.SlotId.GetHashCode();
            hashCode += this.Category.GetHashCode() * 29;
            hashCode += this.Type.GetHashCode() * 31;
            hashCode += this.DefaultValue.GetHashCode() * 13;
            if (!string.IsNullOrWhiteSpace(this.Description))
                hashCode += this.Description.GetHashCode() * 17;

            return hashCode;
        }
    }
}
