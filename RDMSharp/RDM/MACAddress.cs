//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;

//namespace RDMSharp
//{
//    public readonly struct MACAddress : IEquatable<MACAddress>
//    {
//        public static readonly MACAddress Empty = new MACAddress(0, 0, 0, 0, 0, 0);

//        public readonly byte B1;
//        public readonly byte B2;
//        public readonly byte B3;
//        public readonly byte B4;
//        public readonly byte B5;
//        public readonly byte B6;

//        public MACAddress(in byte b1, in byte b2, in byte b3, in byte b4, in byte b5, in byte b6)
//        {
//            this.B1 = b1;
//            this.B2 = b2;
//            this.B3 = b3;
//            this.B4 = b4;
//            this.B5 = b5;
//            this.B6 = b6;
//        }
//        public MACAddress(params byte[] bytes)
//        {
//            if (bytes.Length != 6)
//                throw new ArgumentOutOfRangeException();

//            this.B1 = bytes[0];
//            this.B2 = bytes[1];
//            this.B3 = bytes[2];
//            this.B4 = bytes[3];
//            this.B5 = bytes[4];
//            this.B6 = bytes[5];
//        }
//        public MACAddress(in string macAddress) : this()
//        {
//            Regex regex6g = new Regex(@"^([A-Fa-f0-9]{1,2})[\:\-\s]([A-Fa-f0-9]{1,2})[\:\-\s]([A-Fa-f0-9]{1,2})[\:\-\s]([A-Fa-f0-9]{1,2})[\:\-\s]([A-Fa-f0-9]{1,2})[\:\-\s]([A-Fa-f0-9]{1,2})$");
//            var match = regex6g.Match(macAddress);
//            if (!match.Success)
//            {
//                Regex regex3g = new Regex(@"^([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})\.([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})\.([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})$");
//                match = regex3g.Match(macAddress);
//            }
//            if (!match.Success)
//            {
//                Regex regex0g = new Regex(@"^([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})([0-9A-Fa-f]{2})$");
//                match = regex0g.Match(macAddress);
//            }
//            if (match.Success)
//            {
//                B1 = byte.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
//                B2 = byte.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
//                B3 = byte.Parse(match.Groups[3].Value, System.Globalization.NumberStyles.HexNumber);
//                B4 = byte.Parse(match.Groups[4].Value, System.Globalization.NumberStyles.HexNumber);
//                B5 = byte.Parse(match.Groups[5].Value, System.Globalization.NumberStyles.HexNumber);
//                B6 = byte.Parse(match.Groups[6].Value, System.Globalization.NumberStyles.HexNumber);
//            }
//            else
//                throw new FormatException($"The given string\"{macAddress}\" is not matchable to any known MAC-Address format");
//        }

//        public MACAddress(IEnumerable<byte> enumerable)
//        {
//            if (enumerable.Count() != 6)
//            {
//                throw new ArgumentOutOfRangeException("bytes should be an array with a length of 6");
//            }

//            B1 = enumerable.ElementAt(0);
//            B2 = enumerable.ElementAt(1);
//            B3 = enumerable.ElementAt(2);
//            B4 = enumerable.ElementAt(3);
//            B5 = enumerable.ElementAt(4);
//            B6 = enumerable.ElementAt(5);
//        }


//        public override string ToString()
//        {
//            return $"{B1:X2}:{B2:X2}:{B3:X2}:{B4:X2}:{B5:X2}:{B6:X2}";
//        }
//        public static implicit operator byte[](MACAddress mac)
//        {
//            return new byte[6] { mac.B1, mac.B2, mac.B3, mac.B4, mac.B5, mac.B6 };
//        }
//        public static implicit operator MACAddress(byte[] bytes)
//        {
//            return new MACAddress(bytes);
//        }

//        public static bool operator ==(MACAddress a, MACAddress b)
//        {
//            return a.Equals(b);
//        }

//        public static bool operator !=(MACAddress a, MACAddress b)
//        {
//            return !a.Equals(b);
//        }

//        public bool Equals(MACAddress other)
//        {
//            if (this.B1 != other.B1)
//                return false;
//            if (this.B2 != other.B2)
//                return false;
//            if (this.B3 != other.B3)
//                return false;
//            if (this.B4 != other.B4)
//                return false;
//            if (this.B5 != other.B5)
//                return false;
//            if (this.B6 != other.B6)
//                return false;

//            return true;
//        }

//        public override bool Equals(object obj)
//        {
//            return obj is MACAddress && Equals((MACAddress)obj);
//        }

//        public override int GetHashCode()
//        {
//            int hashCode = -1756596593;
//            hashCode = hashCode * -1521134295 + B1.GetHashCode();
//            hashCode = hashCode * -1521134295 + B2.GetHashCode();
//            hashCode = hashCode * -1521134295 + B3.GetHashCode();
//            hashCode = hashCode * -1521134295 + B4.GetHashCode();
//            hashCode = hashCode * -1521134295 + B5.GetHashCode();
//            hashCode = hashCode * -1521134295 + B6.GetHashCode();
//            return hashCode;
//        }
//    }
//}