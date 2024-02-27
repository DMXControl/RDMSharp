using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace RDMSharp
{
    public readonly struct IPv4Address : IEquatable<IPv4Address>
    {
        public readonly byte B1;
        public readonly byte B2;
        public readonly byte B3;
        public readonly byte B4;

        public static IPv4Address LocalHost { get => new IPv4Address(127, 0, 0, 1); }

        public IPv4Address(in byte block1, in byte block2, in byte block3, in byte block4)
        {
            B1 = block1;
            B2 = block2;
            B3 = block3;
            B4 = block4;
        }
        public IPv4Address(in string ipAddress) : this()
        {
            Regex regex = new Regex(@"^([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})\.([0-9]{1,3})$");
            var match = regex.Match(ipAddress);
            if (!match.Success)
                throw new FormatException("The given string is not a IPv4Address");
            B1 = byte.Parse(match.Groups[1].Value);
            B2 = byte.Parse(match.Groups[2].Value);
            B3 = byte.Parse(match.Groups[3].Value);
            B4 = byte.Parse(match.Groups[4].Value);
        }

        public IPv4Address(byte[] bytes) : this()
        {
            if (bytes.Length != 4)
                throw new ArgumentOutOfRangeException("bytes should be an array with a length of 4");

            B1 = bytes[0];
            B2 = bytes[1];
            B3 = bytes[2];
            B4 = bytes[3];
        }

        public IPv4Address(IEnumerable<byte> enumerable) : this()
        {
            if (enumerable.Count() != 4)
                throw new ArgumentOutOfRangeException("bytes should be an array with a length of 4");

            B1 = enumerable.ElementAt(0);
            B2 = enumerable.ElementAt(1);
            B3 = enumerable.ElementAt(2);
            B4 = enumerable.ElementAt(3);
        }

        public static implicit operator IPAddress(IPv4Address address)
        {
            return new IPAddress(new byte[4] { address.B1, address.B2, address.B3, address.B4 });
        }
        public static implicit operator IPv4Address(IPAddress ip)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return new IPv4Address(ip.GetAddressBytes());
            throw new ArgumentException($"{ip} is not a Valid IPv4 and cant be converted");
        }
        public static implicit operator byte[](IPv4Address address)
        {
            return new byte[4] { address.B1, address.B2, address.B3, address.B4 };
        }
        public static implicit operator IPv4Address(byte[] bytes)
        {
            return new IPv4Address(bytes);
        }
        public override string ToString()
        {
            return $"{B1}.{B2}.{B3}.{B4}";
        }

        public static bool operator ==(IPv4Address a, IPv4Address b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IPv4Address a, IPv4Address b)
        {
            return !a.Equals(b);
        }

        public bool Equals(IPv4Address other)
        {
            if (this.B1 != other.B1)
                return false;
            if (this.B2 != other.B2)
                return false;
            if (this.B3 != other.B3)
                return false;
            if (this.B4 != other.B4)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is IPv4Address other &&
                   B1 == other.B1 &&
                   B2 == other.B2 &&
                   B3 == other.B3 &&
                   B4 == other.B4;
        }

        public override int GetHashCode()
        {
            int hashCode = 1916557166;
            hashCode = hashCode * -1521134295 + B1.GetHashCode();
            hashCode = hashCode * -1521134295 + B2.GetHashCode();
            hashCode = hashCode * -1521134295 + B3.GetHashCode();
            hashCode = hashCode * -1521134295 + B4.GetHashCode();
            return hashCode;
        }
    }
}