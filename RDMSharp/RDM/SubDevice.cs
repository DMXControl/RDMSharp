using System;

namespace RDMSharp
{
    public readonly struct SubDevice : IEquatable<SubDevice>
    {

        public static readonly SubDevice Root = new SubDevice(0);
        public static readonly SubDevice Broadcast = new SubDevice(0xFFFF);

        public readonly ushort ID;

        public SubDevice(in ushort id)
        {
            if (id > 0x0200 && id < 0xFFFF)
                throw new ArgumentOutOfRangeException($"A ID of {id.ToString("X4")} is not valid, it should be 0x0000, 0x0001-0x0200 ore 0xFFFF");

            this.ID = id;
        }

        public override string ToString()
        {
            if (this.IsRoot)
                return $"Root ({this.ID.ToString("X4")})";
            if (this.IsBroadcast)
                return $"Broadcast ({this.ID.ToString("X4")})";

            return this.ID.ToString("X4");
        }

        public static bool operator ==(in SubDevice a, in SubDevice b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(in SubDevice a, in SubDevice b)
        {
            return !a.Equals(b);
        }

        public static bool operator >(in SubDevice a, in SubDevice b)
        {
            if (a.IsRoot || b.IsRoot)
                return false;
            if (a.IsBroadcast || b.IsBroadcast)
                return false;

            return a.ID > b.ID;
        }

        public static bool operator <(in SubDevice a, in SubDevice b)
        {
            if (a.IsRoot || b.IsRoot)
                return false;
            if (a.IsBroadcast || b.IsBroadcast)
                return false;

            return a.ID < b.ID;
        }

        public static bool operator >=(in SubDevice a, in SubDevice b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(in SubDevice a, in SubDevice b)
        {
            return a < b || a == b;
        }

        public static explicit operator ushort(in SubDevice a)
        {
            return ((ushort)a.ID);
        }

        public bool Equals(SubDevice other)
        {
            return this.ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return obj is SubDevice subDevice && Equals(subDevice);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public bool IsRoot
        {
            get
            {
                return this.ID == 0x0000;
            }
        }
        public bool IsBroadcast
        {
            get
            {
                return this.ID == 0xFFFF;
            }
        }
    }
}