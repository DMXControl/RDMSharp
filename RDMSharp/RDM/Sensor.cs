using System;
using System.ComponentModel;
using System.Text;

namespace RDMSharp
{
    public class Sensor : INotifyPropertyChanged, IEquatable<Sensor>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly byte SensorId;

        private ERDM_SensorType type;
        public ERDM_SensorType Type
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

        private ERDM_SensorUnit unit;
        public ERDM_SensorUnit Unit
        {
            get { return unit; }
            private set
            {
                if (unit == value)
                    return;

                unit = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Unit)));
            }
        }

        private ERDM_UnitPrefix prefix;
        public ERDM_UnitPrefix Prefix
        {
            get { return prefix; }
            private set
            {
                if (prefix == value)
                    return;

                prefix = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Prefix)));
            }
        }

        private short rangeMinimum;
        public short RangeMinimum
        {
            get { return rangeMinimum; }
            private set
            {
                if (rangeMinimum == value)
                    return;

                rangeMinimum = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(RangeMinimum)));
            }
        }

        private short rangeMaximum;
        public short RangeMaximum
        {
            get { return rangeMaximum; }
            private set
            {
                if (rangeMaximum == value)
                    return;

                rangeMaximum = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(RangeMaximum)));
            }
        }

        private short normalMinimum;
        public short NormalMinimum
        {
            get { return normalMinimum; }
            private set
            {
                if (normalMinimum == value)
                    return;

                normalMinimum = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(NormalMinimum)));
            }
        }

        private short normalMaximum;
        public short NormalMaximum
        {
            get { return normalMaximum; }
            private set
            {
                if (normalMaximum == value)
                    return;

                normalMaximum = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(NormalMaximum)));
            }
        }

        private bool lowestHighestValueSupported;
        public bool LowestHighestValueSupported
        {
            get { return lowestHighestValueSupported; }
            private set
            {
                if (lowestHighestValueSupported == value)
                    return;

                lowestHighestValueSupported = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(LowestHighestValueSupported)));
            }
        }

        private bool recordedValueSupported;
        public bool RecordedValueSupported
        {
            get { return recordedValueSupported; }
            private set
            {
                if (recordedValueSupported == value)
                    return;

                recordedValueSupported = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(RecordedValueSupported)));
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

        private short presentValue;
        public short PresentValue
        {
            get { return presentValue; }
            private set
            {
                if (presentValue == value)
                    return;

                presentValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(PresentValue)));
            }
        }
        private short lowestValue;
        public short LowestValue
        {
            get { return lowestValue; }
            private set
            {
                if (lowestValue == value)
                    return;

                lowestValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(LowestValue)));
            }
        }
        private short highestValue;
        public short HighestValue
        {
            get { return highestValue; }
            private set
            {
                if (highestValue == value)
                    return;

                highestValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(HighestValue)));
            }
        }
        private short recordedValue;
        public short RecordedValue
        {
            get { return recordedValue; }
            private set
            {
                if (recordedValue == value)
                    return;

                recordedValue = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(RecordedValue)));
            }
        }
        public Sensor(in byte sensorId)
        {
            this.SensorId = sensorId;
        }
        protected Sensor(in byte sensorId,
            in ERDM_SensorType type,
            in ERDM_SensorUnit unit,
            in ERDM_UnitPrefix prefix,
            in string description,
            in short rangeMinimum,
            in short rangeMaximum,
            in short normalMinimum,
            in short normalMaximum,
            in bool lowestHighestValueSupported = false,
            in bool recordedValueSupported = false) : this(sensorId)
        {
            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            Type = type;
            Unit = unit;
            Prefix = prefix;
            Description = description;
            RangeMinimum = rangeMinimum;
            RangeMaximum = rangeMaximum;
            NormalMinimum = normalMinimum;
            NormalMaximum = normalMaximum;
            LowestHighestValueSupported = lowestHighestValueSupported;
            RecordedValueSupported = recordedValueSupported;
        }
        internal void UpdateDescription(RDMSensorDefinition sensorDescription)
        {
            if (this.SensorId != sensorDescription.SensorId)
                throw new InvalidOperationException($"The given {nameof(sensorDescription)} has not the expected id of {this.SensorId} but {sensorDescription.SensorId}");

            this.Description = sensorDescription.Description;
            this.Type = sensorDescription.Type;
            this.Unit = sensorDescription.Unit;
            this.Prefix = sensorDescription.Prefix;
            this.RangeMinimum = sensorDescription.RangeMinimum;
            this.RangeMaximum = sensorDescription.RangeMaximum;
            this.NormalMinimum = sensorDescription.NormalMinimum;
            this.NormalMaximum = sensorDescription.NormalMaximum;
            this.LowestHighestValueSupported = sensorDescription.LowestHighestValueSupported;
            this.RecordedValueSupported = sensorDescription.RecordedValueSupported;
        }
        internal void UpdateValue(RDMSensorValue sensorValue)
        {
            if (this.SensorId != sensorValue.SensorId)
                throw new InvalidOperationException($"The given {nameof(sensorValue)} has not the expected id of {this.SensorId} but {sensorValue.SensorId}");

            this.PresentValue = sensorValue.PresentValue;
            this.LowestValue = sensorValue.LowestValue;
            this.HighestValue = sensorValue.HighestValue;
            this.RecordedValue = sensorValue.RecordedValue;
        }

        protected virtual void UpdateValue(short value)
        {
            PresentValue = value;
            if (this.LowestHighestValueSupported)
                updateLowestHighestValue(value);
        }
        private void updateLowestHighestValue(short value)
        {
            LowestValue = Math.Min(LowestValue, value);
            HighestValue = Math.Max(HighestValue, value);
        }
        internal void RecordValue(short value)
        {
            if (this.RecordedValueSupported)
                RecordedValue = value;
        }
        internal void ResetValues()
        {
            LowestValue = PresentValue;
            HighestValue = PresentValue;
            RecordedValue = PresentValue;
        }

        public string GetFormatedString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Tools.GetFormatedSensorValue(PresentValue, Prefix, Unit));
            if (LowestHighestValueSupported)
            {
                sb.Append(" ");
                sb.Append($"Lowest: {Tools.GetFormatedSensorValue(LowestValue, Prefix, Unit)}");
                sb.Append(" ");
                sb.Append($"Highest: {Tools.GetFormatedSensorValue(HighestValue, Prefix, Unit)}");
            }
            if (RecordedValueSupported)
            {
                sb.Append(" ");
                sb.Append($"Recorded: {Tools.GetFormatedSensorValue(RecordedValue, Prefix, Unit)}");
            }

            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Sensor: {this.SensorId}");
            sb.AppendLine($"Formated: {this.GetFormatedString()}");
            sb.AppendLine($"Type: {this.Type}");
            sb.AppendLine($"Unit: {this.Unit}");
            sb.AppendLine($"Prefix: {this.Prefix}");
            sb.AppendLine($"Description: {this.Description}");
            sb.AppendLine($"RangeMinimum: {this.RangeMinimum}");
            sb.AppendLine($"RangeMaximum: {this.RangeMaximum}");
            sb.AppendLine($"NormalMinimum: {this.NormalMinimum}");
            sb.AppendLine($"NormalMaximum: {this.NormalMaximum}");
            sb.AppendLine($"LowestHighestValueSupported: {this.LowestHighestValueSupported}");
            sb.AppendLine($"RecordedValueSupported: {this.RecordedValueSupported}");
            sb.AppendLine($"PresentValue: {this.PresentValue}");
            if (LowestHighestValueSupported)
            {
                sb.AppendLine($"LowestValue: {this.LowestValue}");
                sb.AppendLine($"HighestValue: {this.HighestValue}");
            }
            if (RecordedValueSupported)
                sb.AppendLine($"RecordedValue: {this.RecordedValue}");

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Sensor);
        }

        public bool Equals(Sensor other)
        {
            return other is not null &&
                   SensorId == other.SensorId &&
                   Type == other.Type &&
                   Unit == other.Unit &&
                   Prefix == other.Prefix &&
                   RangeMinimum == other.RangeMinimum &&
                   RangeMaximum == other.RangeMaximum &&
                   NormalMinimum == other.NormalMinimum &&
                   NormalMaximum == other.NormalMaximum &&
                   LowestHighestValueSupported == other.LowestHighestValueSupported &&
                   RecordedValueSupported == other.RecordedValueSupported &&
                   Description == other.Description &&
                   PresentValue == other.PresentValue &&
                   LowestValue == other.LowestValue &&
                   HighestValue == other.HighestValue &&
                   RecordedValue == other.RecordedValue;
        }

        public override int GetHashCode()
        {
#if !NETSTANDARD
            HashCode hash = new HashCode();
            hash.Add(SensorId);
            hash.Add(Type);
            hash.Add(Unit);
            hash.Add(Prefix);
            hash.Add(RangeMinimum);
            hash.Add(RangeMaximum);
            hash.Add(NormalMinimum);
            hash.Add(NormalMaximum);
            hash.Add(LowestHighestValueSupported);
            hash.Add(RecordedValueSupported);
            hash.Add(Description);
            return hash.ToHashCode();
#else
            int hashCode = 1916557166;
            hashCode = hashCode * -1521134295 + SensorId.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Unit.GetHashCode();
            hashCode = hashCode * -1521134295 + Prefix.GetHashCode();
            hashCode = hashCode * -1521134295 + RangeMinimum.GetHashCode();
            hashCode = hashCode * -1521134295 + RangeMaximum.GetHashCode();
            hashCode = hashCode * -1521134295 + NormalMinimum.GetHashCode();
            hashCode = hashCode * -1521134295 + NormalMaximum.GetHashCode();
            hashCode = hashCode * -1521134295 + LowestHighestValueSupported.GetHashCode();
            hashCode = hashCode * -1521134295 + RecordedValueSupported.GetHashCode();
            hashCode = hashCode * -1521134295 + Description.GetHashCode();
            return hashCode;
#endif
        }


        public static implicit operator RDMSensorDefinition(Sensor _this)
        {
            return new RDMSensorDefinition(_this.SensorId,
                                           _this.Type,
                                           _this.Unit,
                                           _this.Prefix,
                                           _this.RangeMinimum,
                                           _this.RangeMaximum,
                                           _this.NormalMinimum,
                                           _this.NormalMaximum,
                                           _this.LowestHighestValueSupported,
                                           _this.RecordedValueSupported,
                                           _this.Description);
        }
        public static implicit operator RDMSensorValue(Sensor _this)
        {
            return new RDMSensorValue(_this.SensorId,
                                           _this.PresentValue,
                                           _this.LowestValue,
                                           _this.HighestValue,
                                           _this.RecordedValue);
        }
    }
}