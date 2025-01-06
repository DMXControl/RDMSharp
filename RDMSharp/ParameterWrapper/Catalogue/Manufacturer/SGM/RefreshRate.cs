using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.REFRESH_RATE, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.REFRESH_RATE, Command.ECommandDublicte.SetRequest)]

    public readonly struct RefreshRate : IEquatable<RefreshRate>
    {
        public const uint FREQUENCY_MULTIPLYER = 197647;
        public readonly byte RawValue;
        public readonly uint Frequency;

        public RefreshRate(in byte rawValue)
        {
            if (rawValue == 0)
                return;

            this.RawValue = rawValue;
            this.Frequency = FREQUENCY_MULTIPLYER / this.RawValue;
        }
        public RefreshRate(in uint frequency)
        {
            this.Frequency = frequency;
            this.RawValue = (byte)Math.Max(0, Math.Min(byte.MaxValue, FREQUENCY_MULTIPLYER / this.Frequency));
        }

        public bool Equals(RefreshRate other)
        {
            if (this.RawValue != other.RawValue)
                return false;
            if (this.Frequency != other.Frequency)
                return false;

            return true;
        }
        public override bool Equals(object obj)
        {
            return obj is RefreshRate refreshRate && Equals(refreshRate);
        }

        public override int GetHashCode()
        {
            return this.RawValue.GetHashCode();
        }

        public override string ToString()
        {
            return $"RefreshRate: {this.Frequency}";
        }
    }
}