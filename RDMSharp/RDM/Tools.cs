using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RDMSharp
{
    public static class Tools
    {
        public static int GenerateHashCode<T>(this IEnumerable<T> enumerable)
        {
            int hash = 0;
            if (enumerable == null) return hash;

            int mul = 1;
            foreach (var a in enumerable)
                hash += (a?.GetHashCode() * mul++) ?? 0;

            return hash;
        }
        public static string FormatNumber(double number)
        {
            string[] prefixes = { "", "k", "M", "G", "T", "P", "E", "Z", "Y" };
            string[] suffixes = { "", "m", "µ", "n", "p", "f", "a", "z", "y" };

            int prefixIndex = 0;
            int suffixIndex = 0;

            while (Math.Abs(number) >= 1000 && prefixIndex < prefixes.Length - 1)
            {
                number /= 1000;
                prefixIndex++;
            }

            while (Math.Abs(number) < 1 && suffixIndex < suffixes.Length - 1)
            {
                number *= 1000;
                suffixIndex++;
            }

            string prefix = prefixes[prefixIndex];
            string suffix = suffixes[suffixIndex];

            return $"{number}{prefix}{suffix}";
        }

        public static string GetFormatedSensorValue(in int value,in ERDM_UnitPrefix prefix, in ERDM_SensorUnit unit)
        {
            return $"{FormatNumber(prefix.GetNormalizedValue(value))}{unit.GetUnitSymbol()}";
        }

        public static double GetNormalizedValue(this ERDM_UnitPrefix prefix, in int value)
        {
            return value * (prefix.GetAttribute<UnitPrefixAttribute>().Multiplyer);
        }

        public static string GetUnitSymbol(this ERDM_SensorUnit unit)
        {
            return unit.GetAttribute<SensorUnitAttribute>().Unit;
        }

        public static string GetStatusMessage(this ERDM_StatusMessage status, in short dataValue1 = 0, in short dataValue2 = 0)
        {
            return status.GetAttribute<StatusMessageAttribute>().GetFormatedString(dataValue1, dataValue2) ?? string.Empty;
        }
        public static string GetEnumDescription(Enum value)
        {
            return value.GetAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static byte[] ValueToData(params bool[] bits)
        {
            return ValueToData(bits, trim: 0);
        }
        public static byte[] ValueToData(object value, int trim = 32)
        {
            if (value == null)
                return Array.Empty<byte>();

            if (value is Enum @enum)
            {
                switch (@enum.GetTypeCode())
                {
                    default:
                    case TypeCode.Byte:
                        value = (byte)value;
                        break;
                    case TypeCode.SByte:
                        value = (sbyte)value;
                        break;
                    case TypeCode.Int16:
                        value = (short)value;
                        break;
                    case TypeCode.UInt16:
                        value = (ushort)value;
                        break;
                    case TypeCode.Int32:
                        value = (int)value;
                        break;
                    case TypeCode.UInt32:
                        value = (uint)value;
                        break;
                    case TypeCode.Int64:
                        value = (long)value;
                        break;
                    case TypeCode.UInt64:
                        value = (ulong)value;
                        break;
                }
            }

            switch (value)
            {
                case AbstractRDMPayloadObject[] @payloadObjectArray:
                    List<byte> l1Bytes = new List<byte>();
                    foreach (AbstractRDMPayloadObject payloadObject in @payloadObjectArray)
                        l1Bytes.AddRange(payloadObject.ToPayloadData());
                    return l1Bytes.ToArray();
                case AbstractRDMPayloadObject @payloadObject:
                    return payloadObject.ToPayloadData();

                case byte[] @byteArray:
                    return @byteArray;
                case bool[] @boolArray:
                    int byteCount = (int)Math.Ceiling(@boolArray.Length / 8.0);
                    byte[] _resArray = new byte[byteCount];
                    for (int bitIndex = 0; bitIndex < @boolArray.Length; bitIndex++)
                    {
                        int byteIndex = bitIndex / 8;
                        int bitInByteIndex = bitIndex % 8;
                        byte mask = (byte)(1 << bitInByteIndex);
                        if (boolArray[bitIndex])
                            _resArray[byteIndex] |= mask;
                    }
                    return _resArray;
                case UID[] @uidArray:
                    List<byte> lBytes = new List<byte>();
                    foreach (UID uid in @uidArray)
                        lBytes.AddRange(uid.ToBytes());
                    return lBytes.ToArray();
                case UID @uid:
                    return @uid.ToBytes().ToArray();

                case IPv4Address @ipv4Address:
                    return (byte[])@ipv4Address;
                case IPAddress @ipAddress:
                    if (@ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //AL 2022-09-11: Commented out as ipAddress.Address is deprecated
                        //List<byte> ipBytes = new List<byte>();
                        //byte[] ipAddressBytes = Tools.ValueToData((uint)@ipAddress.Address);
                        //for (int i = 3; i >= 0; i--)
                        //    ipBytes.Add(ipAddressBytes[i]);
                        //return ipBytes.ToArray();

                        return ipAddress.GetAddressBytes(); //In case the Bytes are in reverse order use .Reverse().ToArray();
                    }
                    else if (@ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        return @ipAddress.GetAddressBytes();
                    }
                    throw new NotSupportedException();

                case byte @byte:
                    return DoOneByte(@byte);
                case sbyte @sbyte:
                    return DoOneByte((byte)@sbyte);
                case ushort @ushort:
                    return DoTwoByte(@ushort);
                case short @short:
                    return DoTwoByte((ushort)@short);
                case int @int:
                    return DoFourByte((uint)@int);
                case uint @uint:
                    return DoFourByte(@uint);
                case long @long:
                    return DoEightByte((ulong)@long);
                case ulong @ulong:
                    return DoEightByte(@ulong);

                case string @string:
                    if (trim >= 1)
                        if (@string.Length > trim)
                            @string = @string.Substring(0, trim);

                    return Encoding.UTF8.GetBytes(@string);

                case bool @bool:
                    return new byte[] { (byte)(@bool ? 1 : 0) };

                default:
                    throw new NotSupportedException(value.GetType().Name);
            }

            //Local Functions

            byte[] DoOneByte(byte b) => new byte[] { b };
            byte[] DoTwoByte(ushort s) => new byte[] { (byte)(s >> 8), (byte)s };
            byte[] DoFourByte(uint i) => new byte[] { (byte)(i >> 24), (byte)(i >> 16), (byte)(i >> 8), (byte)i };
            byte[] DoEightByte(ulong i) => new byte[] { (byte)(i >> 56), (byte)(i >> 48), (byte)(i >> 40), (byte)(i >> 32), (byte)(i >> 24), (byte)(i >> 16), (byte)(i >> 8), (byte)i };
        }

        public static byte DataToByte(ref byte[] data)
        {
            const int length = 1;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            byte res = (byte)data[0];
            data = data.Skip(length).ToArray();
            return res;
        }
        public static sbyte DataToSByte(ref byte[] data)
        {
            const int length = 1;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            sbyte res = (sbyte)data[0];
            data = data.Skip(length).ToArray();
            return res;
        }
        public static short DataToShort(ref byte[] data)
        {
            const int length = 2;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            short res = (short)((data[0] << 8) | data[1]);
            data = data.Skip(length).ToArray();
            return res;
        }
        public static ushort DataToUShort(ref byte[] data)
        {
            const int length = 2;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            ushort res = (ushort)((data[0] << 8) | data[1]);
            data = data.Skip(length).ToArray();
            return res;
        }
        public static int DataToInt(ref byte[] data)
        {
            const int length = 4;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            int res = (int)((data[0] << 24) | (data[1] << 16) | data[2] << 8 | data[3]);
            data = data.Skip(length).ToArray();
            return res;
        }
        public static uint DataToUInt(ref byte[] data)
        {
            const int length = 4;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            uint res = (uint)((data[0] << 24) | (data[1] << 16) | data[2] << 8 | data[3]);
            data = data.Skip(length).ToArray();
            return res;
        }
        public static long DataToLong(ref byte[] data)
        {
            const int length = 8;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            long res = (long)((((long)data[0]) << 56) | (((long)data[1]) << 48) | (((long)data[2]) << 40) | (((long)data[3]) << 32) | (((long)data[4]) << 24) | (((long)data[5]) << 16) | ((long)data[6]) << 8 | ((long)data[7]));
            data = data.Skip(length).ToArray();
            return res;
        }
        public static ulong DataToULong(ref byte[] data)
        {
            const int length = 8;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            ulong res = (ulong)((((ulong)data[0]) << 56) | (((ulong)data[1]) << 48) | (((ulong)data[2]) << 40) | (((ulong)data[3]) << 32) | (((ulong)data[4]) << 24) | (((ulong)data[5]) << 16) | ((ulong)data[6]) << 8 | ((ulong)data[7]));
            data = data.Skip(length).ToArray();
            return res;
        }
        public static UID DataToRDMUID(ref byte[] data)
        {
            const int length = 6;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            return new UID(DataToUShort(ref data), DataToUInt(ref data)); ;
        }

        public static string DataToString(ref byte[] data, int take = 0)
        {
            string res;
            if (take != 0)
            {
                res = Encoding.UTF8.GetString(data.Take(take).ToArray());
                data = data.Skip(take).ToArray();
            }
            else
            {
                res = Encoding.UTF8.GetString(data);
                data = Array.Empty<byte>();
            }
            return res;
        }
        public static bool DataToBool(ref byte[] data)
        {
            const int length = 1;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            bool res = false;
            if (data[0] == (byte)0x01)
                res = true;

            data = data.Skip(length).ToArray();
            return res;
        }
        public static bool[] DataToBoolArray(ref byte[] data, int bitCount)
        {
            List<bool> res = new List<bool>();
            int length = (int)Math.Ceiling(bitCount / 8.0);
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            for (int bitIndex = 0; bitIndex < bitCount; bitIndex++)
            {
                int byteIndex = bitIndex / 8;
                int bitInByteIndex = bitIndex % 8;
                byte mask = (byte)(1 << bitInByteIndex);
                res.Add((data[byteIndex] & mask) != 0);
            }

            data = data.Skip(length).ToArray();
            return res.ToArray();
        }
        public static T DataToEnum<T>(ref byte[] data) where T : Enum
        {
            var en = Enum.GetValues(typeof(T));
            var enums = en.Cast<T>();

            switch (enums.FirstOrDefault()?.GetTypeCode())
            {
                default:
                case TypeCode.Byte:
                    return (T)Enum.ToObject(typeof(T), DataToByte(ref data));
                case TypeCode.SByte:
                    return (T)Enum.ToObject(typeof(T), DataToSByte(ref data));

                case TypeCode.Int16:
                    return (T)Enum.ToObject(typeof(T), DataToShort(ref data));
                case TypeCode.UInt16:
                    return (T)Enum.ToObject(typeof(T), DataToUShort(ref data));

                case TypeCode.Int32:
                    return (T)Enum.ToObject(typeof(T), DataToInt(ref data));
                case TypeCode.UInt32:
                    return (T)Enum.ToObject(typeof(T), DataToUInt(ref data));


                case TypeCode.Int64:
                    return (T)Enum.ToObject(typeof(T), DataToLong(ref data));
                case TypeCode.UInt64:
                    return (T)Enum.ToObject(typeof(T), DataToULong(ref data));
            }
        }
        public static IPv4Address DataToIPAddressIPv4(ref byte[] @data)
        {
            const int length = 4;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            byte[] bytes = new byte[length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Tools.DataToByte(ref data);

            return new IPv4Address(bytes);
        }
        public static IPAddress DataToIPAddressIPv6(ref byte[] @data)
        {
            const int length = 16;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            byte[] bytes = new byte[length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Tools.DataToByte(ref data);

            return new IPAddress(bytes);
        }
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
#if NETSTANDARD
            if (list == null)
                throw new ArgumentNullException(nameof(list));
#else
            ArgumentNullException.ThrowIfNull(list);
#endif

            if (list is List<T> instance)
                return instance.AsReadOnly();
            return new ReadOnlyCollection<T>(list);
        }
        public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> source)
        {
#if NETSTANDARD
            if (source == null)
                throw new ArgumentNullException(nameof(source));
#else
            ArgumentNullException.ThrowIfNull(source);
#endif

            return new ReadOnlyDictionary<K, V>(source);
        }
    }
}
