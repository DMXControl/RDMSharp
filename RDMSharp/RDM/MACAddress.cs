using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RDMSharp
{
    public readonly struct MACAddress : IEquatable<MACAddress>
    {
        public static readonly MACAddress Empty = new MACAddress(0, 0, 0, 0, 0, 0);

        public readonly byte B1;
        public readonly byte B2;
        public readonly byte B3;
        public readonly byte B4;
        public readonly byte B5;
        public readonly byte B6;

        public MACAddress(in byte b1, in byte b2, in byte b3, in byte b4, in byte b5, in byte b6)
        {
            this.B1 = b1;
            this.B2 = b2;
            this.B3 = b3;
            this.B4 = b4;
            this.B5 = b5;
            this.B6 = b6;
        }
        public MACAddress(params byte[] bytes)
        {
            if (bytes.Length != 6)
                throw new ArgumentOutOfRangeException();

            this.B1 = bytes[0];
            this.B2 = bytes[1];
            this.B3 = bytes[2];
            this.B4 = bytes[3];
            this.B5 = bytes[4];
            this.B6 = bytes[5];
        }
        public MACAddress(in string macAddress) : this()
        {
            Regex regex = new Regex(@"^([A-Fa-f0-9]{1,2})\:([A-Fa-f0-9]{1,2})\:([A-Fa-f0-9]{1,2})\:([A-Fa-f0-9]{1,2})\:([A-Fa-f0-9]{1,2})\:([A-Fa-f0-9]{1,2})$");
            var match = regex.Match(macAddress);
            B1 = byte.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
            B2 = byte.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
            B3 = byte.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.HexNumber);
            B4 = byte.Parse(match.Groups[4].Value, System.Globalization.NumberStyles.HexNumber);
            B5 = byte.Parse(match.Groups[5].Value, System.Globalization.NumberStyles.HexNumber);
            B6 = byte.Parse(match.Groups[6].Value, System.Globalization.NumberStyles.HexNumber);
        }


        public override string ToString()
        {
            return $"{B1:X2}:{B2:X2}:{B3:X2}:{B4:X2}:{B5:X2}:{B6:X2}";
        }
        public static implicit operator byte[](MACAddress mac)
        {
            return new byte[6] { mac.B1, mac.B2, mac.B3, mac.B4, mac.B5, mac.B6 };
        }
        public static implicit operator MACAddress(byte[] bytes)
        {
            return new MACAddress(bytes);
        }

        public static MACAddress Parse(string uid)
        {
            if (!uid.Contains(':')) return MACAddress.Empty;
            string[] parts = uid.Split(':');
            if (parts.Length != 6) return MACAddress.Empty;
            foreach (var part in parts)
                if (part.Length != 2) return MACAddress.Empty;

            byte b1;
            byte b2;
            byte b3;
            byte b4;
            byte b5;
            byte b6;
            try
            {
                b1 = Convert.ToByte(parts[0], 16);
                b2 = Convert.ToByte(parts[1], 16);
                b3 = Convert.ToByte(parts[2], 16);
                b4 = Convert.ToByte(parts[3], 16);
                b5 = Convert.ToByte(parts[4], 16);
                b6 = Convert.ToByte(parts[5], 16);
            }
            catch
            {
                return MACAddress.Empty;
            }
            return new MACAddress(b1, b2, b3, b4, b5, b6);
        }
        public IEnumerable<byte> ToBytes()
        {
            yield return this.B1;
            yield return this.B2;
            yield return this.B3;
            yield return this.B4;
            yield return this.B5;
            yield return this.B6;
        }

        public static bool operator ==(MACAddress a, MACAddress b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MACAddress a, MACAddress b)
        {
            return !a.Equals(b);
        }

        public bool Equals(MACAddress other)
        {
            if (this.B1 != other.B1)
                return false;
            if (this.B2 != other.B2)
                return false;
            if (this.B3 != other.B3)
                return false;
            if (this.B4 != other.B4)
                return false;
            if (this.B5 != other.B5)
                return false;
            if (this.B6 != other.B6)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is MACAddress && Equals((MACAddress)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = -1756596593;
            hashCode = hashCode * -1521134295 + B1.GetHashCode();
            hashCode = hashCode * -1521134295 + B2.GetHashCode();
            hashCode = hashCode * -1521134295 + B3.GetHashCode();
            hashCode = hashCode * -1521134295 + B4.GetHashCode();
            hashCode = hashCode * -1521134295 + B5.GetHashCode();
            hashCode = hashCode * -1521134295 + B6.GetHashCode();
            return hashCode;
        }
    }
}