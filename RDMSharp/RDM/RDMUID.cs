using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RDMSharp
{
    public readonly struct RDMUID : IEquatable<RDMUID>, IComparer<RDMUID>, IComparable<RDMUID>
    {
        public static readonly RDMUID Empty = new RDMUID((ushort)0, 0);
        public static readonly RDMUID Broadcast = CreateManufacturerBroadcast(0xFFFF);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045")]
        private static readonly Regex regex6g = new Regex(@"^([A-Fa-f0-9]{1,4})[\:\.\-\s]([A-Fa-f0-9]{1,8})$");
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045")]
        private static readonly Regex regex0g = new Regex(@"^([0-9A-Fa-f]{4})([0-9A-Fa-f]{8})$");

        public readonly ushort ManufacturerID;
        public EManufacturer Manufacturer => (EManufacturer)ManufacturerID;
        public readonly uint DeviceID;
        public RDMUID(in string uid)
        {
            var match = regex6g.Match(uid);
            if (!match.Success)
                match = regex0g.Match(uid);

            if (match.Success)
            {
                ManufacturerID = Convert.ToUInt16(match.Groups[1].Value, 16);
                DeviceID = Convert.ToUInt32(match.Groups[2].Value, 16);
            }
            else
                throw new FormatException($"The given string\"{uid}\" is not matchable to any known RDM-UID format");
        }
        public RDMUID(in EManufacturer manufacturer, in uint deviceId) : this((ushort)manufacturer, deviceId)
        {
        }
        public RDMUID(in ulong uid) : this((ushort)(uid >> 32), (uint)uid)
        {
        }
        public RDMUID(in ushort manId, in uint deviceId)
        {
            ManufacturerID = manId;
            DeviceID = deviceId;
        }

        public static RDMUID CreateManufacturerBroadcast(in ushort manId)
        {
            return new RDMUID(manId, 0xFFFFFFFF);
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
                   || (a.ManufacturerID == b.ManufacturerID
                   && a.DeviceID > b.DeviceID);
        }

        public static bool operator <(in RDMUID a, in RDMUID b)
        {
            return a.ManufacturerID < b.ManufacturerID
                   || (a.ManufacturerID == b.ManufacturerID
                   && a.DeviceID < b.DeviceID);
        }

        public static bool operator >=(in RDMUID a, in RDMUID b)
        {
            return a > b
                || a == b;
        }

        public static bool operator <=(in RDMUID a, in RDMUID b)
        {
            return a < b
                || a == b;
        }
        public static RDMUID operator *(RDMUID left, double right)
        {
            return new RDMUID((ulong)(((ulong)left) * right));
        }
        public static RDMUID operator /(RDMUID left, double right)
        {
            return new RDMUID((ulong)(((ulong)left) / right));
        }
        public static RDMUID operator +(RDMUID left, int right)
        {
            return new RDMUID((ulong)left + (ulong)right);
        }
        public static RDMUID operator -(RDMUID left, int right)
        {
            return new RDMUID((ulong)left - (ulong)right);
        }
        public static RDMUID operator +(RDMUID left, RDMUID right)
        {
            return new RDMUID((ulong)left + (ulong)right);
        }
        public static RDMUID operator -(RDMUID left, RDMUID right)
        {
            return new RDMUID((ulong)left - (ulong)right);
        }

        public static explicit operator ulong(in RDMUID a)
        {
            return ((ulong)a.ManufacturerID) << 32
                   | a.DeviceID;
        }
        public static explicit operator byte[](in RDMUID a)
        {
            return a.ToBytes().ToArray();
        }

        public bool Equals(RDMUID other)
        {
            return ManufacturerID == other.ManufacturerID
                   && DeviceID == other.DeviceID;
        }

        public override bool Equals(object obj)
        {
            return obj is RDMUID uid
                && Equals(uid);
        }

        public override int GetHashCode()
        {
            return ManufacturerID.GetHashCode() + 17 * DeviceID.GetHashCode();
        }


        public int CompareTo(RDMUID other)
        {
            return Compare(this, other);
        }
        public int Compare(RDMUID x, RDMUID y)
        {
            return ((ulong)x).CompareTo((ulong)y);
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