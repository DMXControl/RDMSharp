using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp
{
    public readonly struct RDMUID : IEquatable<RDMUID>, IComparer<RDMUID>, IComparable<RDMUID>
    {
        public static readonly RDMUID Empty = new RDMUID(0, 0);
        public static readonly RDMUID Broadcast = new RDMUID(0xFFFF, 0xFFFFFFFF);

        public readonly ushort ManufacturerID;
        public EManufacturer Manufacturer => (EManufacturer)ManufacturerID;
        public readonly uint DeviceID;

        public RDMUID(in ushort manId, in uint deviceId)
        {
            ManufacturerID = manId;
            DeviceID = deviceId;
        }

        public RDMUID CreateManufacturerBroadcast(in ushort manId)
        {
            return new RDMUID(manId, 0xFFFFFFFF);
        }

        public static RDMUID FromULong(in ulong uid)
        {
            return new RDMUID((ushort)(uid >> 32), (uint)uid);
        }

        public IEnumerable<byte> ToBytes()
        {
            yield return (byte)(ManufacturerID >> 8);
            yield return (byte)(ManufacturerID);
            yield return (byte)(DeviceID >> 24);
            yield return (byte)(DeviceID >> 16);
            yield return (byte)(DeviceID >> 8);
            yield return (byte)(DeviceID);
        }

        public override string ToString()
        {
            return String.Format("{0:X4}:{1:X8}", ManufacturerID, DeviceID);
        }

        public static RDMUID Parse(in string uid)
        {
            if (!uid.Contains(':')) return RDMUID.Empty;
            string[] parts = uid.Split(':');
            if (parts.Length != 2) return RDMUID.Empty;
            if (parts[0].Length != 4) return RDMUID.Empty;
            if (parts[1].Length != 8) return RDMUID.Empty;
            ushort mfg;
            uint dev;
            try
            {
                mfg = Convert.ToUInt16(parts[0], 16);
                dev = Convert.ToUInt32(parts[1], 16);
            }
            catch
            {
                return RDMUID.Empty;
            }
            return new RDMUID(mfg, dev);
        }

        public static bool operator ==(in RDMUID a, in RDMUID b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(in RDMUID a, in RDMUID b)
        {
            return !a.Equals(b);
        }

        public static bool operator >(in RDMUID a, in RDMUID b)
        {
            return a.ManufacturerID > b.ManufacturerID
                   || (a.ManufacturerID == b.ManufacturerID && a.DeviceID > b.DeviceID);
        }

        public static bool operator <(in RDMUID a, in RDMUID b)
        {
            return a.ManufacturerID < b.ManufacturerID
                   || (a.ManufacturerID == b.ManufacturerID && a.DeviceID < b.DeviceID);
        }

        public static bool operator >=(in RDMUID a, in RDMUID b)
        {
            return a > b || a == b;
        }

        public static bool operator <=(in RDMUID a, in RDMUID b)
        {
            return a < b || a == b;
        }

        public static explicit operator ulong(in RDMUID a)
        {
            return ((ulong)a.ManufacturerID) << 32
                   | a.DeviceID;
        }

        public bool Equals(RDMUID other)
        {
            return ManufacturerID == other.ManufacturerID
                   && DeviceID == other.DeviceID;
        }

        public override bool Equals(object obj)
        {
            return obj is RDMUID uid && Equals(uid);
        }

        public override int GetHashCode()
        {
            return ManufacturerID.GetHashCode() + 17 * DeviceID.GetHashCode();
        }


        public int CompareTo(RDMUID other)
        {
            return ((ulong)this).CompareTo((ulong)other);
        }
        public int Compare(RDMUID x, RDMUID y)
        {
            return x.CompareTo(y);
        }

        public bool IsBroadcast
        {
            get
            {
                return DeviceID == 0xFFFFFFFF;
            }
        }
        public bool IsValidDeviceUID
        {
            get
            {
                if (this.Equals(Empty))
                    return false;

                if (this.ManufacturerID == 0)
                    return false;

                if (this.IsBroadcast)
                    return false;

                return true;
            }
        }
    }
}