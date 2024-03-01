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

        public static double GetNormalizedValue(in ERDM_UnitPrefix prefix, in int value)
        {
            switch (prefix)
            {
                case ERDM_UnitPrefix.DECI: return value * 1E-1;
                case ERDM_UnitPrefix.CENTI: return value * 1E-2;
                case ERDM_UnitPrefix.MILLI: return value * 1E-3;
                case ERDM_UnitPrefix.MICRO: return value * 1E-6;
                case ERDM_UnitPrefix.NANO: return value * 1E-9;
                case ERDM_UnitPrefix.PICO: return value * 1E-12;
                case ERDM_UnitPrefix.FEMPTO: return value * 1E-15;
                case ERDM_UnitPrefix.ATTO: return value * 1E-18;
                case ERDM_UnitPrefix.ZEPTO: return value * 1E-21;
                case ERDM_UnitPrefix.YOCTO: return value * 1E-24;

                case ERDM_UnitPrefix.DECA: return value * 1E1;
                case ERDM_UnitPrefix.HECTO: return value * 1E2;
                case ERDM_UnitPrefix.KILO: return value * 1E3;
                case ERDM_UnitPrefix.MEGA: return value * 1E6;
                case ERDM_UnitPrefix.GIGA: return value * 1E9;
                case ERDM_UnitPrefix.TERRA: return value * 1E12;
                case ERDM_UnitPrefix.PETA: return value * 1E15;
                case ERDM_UnitPrefix.EXA: return value * 1E18;
                case ERDM_UnitPrefix.ZETTA: return value * 1E21;
                case ERDM_UnitPrefix.YOTTA: return value * 1E24;

                case ERDM_UnitPrefix.NONE:
                default:
                    return value;
            }
        }

        public static string GetUnitSymbol(in ERDM_SensorUnit unit)
        {
            switch (unit)
            {
                case ERDM_SensorUnit.CENTIGRADE: return "°C";

                case ERDM_SensorUnit.VOLTS_AC_RMS: return $"V{'\u1D63'}{'\u2098'}{'\u209B'}";
                case ERDM_SensorUnit.VOLTS_AC_PEAK: return $"V{'\u209A'}";
                case ERDM_SensorUnit.VOLTS_DC: return "V";

                case ERDM_SensorUnit.AMPERE_AC_RMS: return $"A{'\u1D63'}{'\u2098'}{'\u209B'}";
                case ERDM_SensorUnit.AMPERE_AC_PEAK: return $"A{'\u209A'}";
                case ERDM_SensorUnit.AMPERE_DC: return "A";

                case ERDM_SensorUnit.HERTZ: return "Hz";
                case ERDM_SensorUnit.WATT: return "W";
                case ERDM_SensorUnit.KILOGRAM: return "kg";
                case ERDM_SensorUnit.METERS: return "m";
                case ERDM_SensorUnit.METERS_SQUARED: return "m²";
                case ERDM_SensorUnit.LUMEN: return "lm";
                case ERDM_SensorUnit.CANDELA: return "cd";

                case ERDM_SensorUnit.OHM: return "Ω";
                case ERDM_SensorUnit.METERS_CUBED: return "m³";
                case ERDM_SensorUnit.KILOGRAMMES_PER_METER_CUBED: return "kg/m³";
                case ERDM_SensorUnit.METERS_PER_SECOND: return "m/s";
                case ERDM_SensorUnit.METERS_PER_SECOND_SQUARED: return "m/s²";
                case ERDM_SensorUnit.NEWTON: return "N";
                case ERDM_SensorUnit.JOULE: return "J";
                case ERDM_SensorUnit.PASCAL: return "Pa";
                case ERDM_SensorUnit.SECOND: return "s";
                case ERDM_SensorUnit.DEGREE: return "°";
                case ERDM_SensorUnit.STERADIAN: return "sr";
                case ERDM_SensorUnit.LUX: return "lx";
                case ERDM_SensorUnit.IRE: return "IRE";
                case ERDM_SensorUnit.BYTE: return "B";

                case ERDM_SensorUnit.NONE:
                default:
                    return "";
            }
            static string SubscriptString(string str)
            {
                StringBuilder result = new StringBuilder();

                foreach (char c in str)
                {
                    if (char.IsLetter(c))
                    {
                        result.Append(SubscriptChar(c));
                    }
                    else
                    {
                        result.Append(c);
                    }
                }

                return result.ToString();
            }
            static char SubscriptChar(char c)
            {
                // Unicode-Offset für tiefgestellte Buchstaben
                const int offset = 0x1D62; // Dieser Wert ist für den Unicode-Bereich U+1D62 bis U+1D6B

                // Überprüfen, ob das Zeichen ein Großbuchstabe ist und ihn in Kleinbuchstaben konvertieren
                if (char.IsUpper(c))
                {
                    c = char.ToLower(c);
                }

                // Tiefgestelltes Zeichen erstellen und zurückgeben
                return (char)(c + offset);
            }
        }

        public static string GetStatusMessage(in ERDM_StatusMessage status, in short dataValue1 = 0, in short dataValue2 = 0)
        {
            switch (status)
            {
                case ERDM_StatusMessage.CAL_FAIL:
                    return $"{SlotLabelCode(dataValue1)} failed calibration.";
                case ERDM_StatusMessage.SENS_NOT_FOUND:
                    return $"{SlotLabelCode(dataValue1)} sensor not found.";
                case ERDM_StatusMessage.SENS_ALWAYS_ON:
                    return $"{SlotLabelCode(dataValue1)} sensor always on.";
                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.FEEDBACK_ERROR:
                    return $"{SlotLabelCode(dataValue1)} feedback error.";
                case ERDM_StatusMessage.INDEX_ERROR:
                    return $"{SlotLabelCode(dataValue1)} index circuit error.";
                #endregion

                case ERDM_StatusMessage.LAMP_DOUSED:
                    return "Lamp doused.";
                case ERDM_StatusMessage.LAMP_STRIKE:
                    return "Lamp failed to strike.";
                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.LAMP_ACCESS_OPEN:
                    return "Lamp access open.";
                case ERDM_StatusMessage.LAMP_ALWAYS_ON:
                    return "Lamp on without command.";
                #endregion

                case ERDM_StatusMessage.OVERTEMP:
                    return $"Sensor {DecimalNumber(dataValue1)} over temp at {DecimalNumber(dataValue2)} °C.";
                case ERDM_StatusMessage.UNDERTEMP:
                    return $"Sensor {DecimalNumber(dataValue1)} under temp at {DecimalNumber(dataValue2)} °C.";
                case ERDM_StatusMessage.SENS_OUT_RANGE:
                    return $"Sensor {DecimalNumber(dataValue1)} out of range.";

                case ERDM_StatusMessage.OVERVOLTAGE_PHASE:
                    return $"Phase {DecimalNumber(dataValue1)} over voltage at {DecimalNumber(dataValue2)} V.";
                case ERDM_StatusMessage.UNDERVOLTAGE_PHASE:
                    return $"Phase {DecimalNumber(dataValue1)} under voltage at {DecimalNumber(dataValue2)} V.";
                case ERDM_StatusMessage.OVERCURRENT:
                    return $"Phase {DecimalNumber(dataValue1)} over currnet at {DecimalNumber(dataValue2)} A.";
                case ERDM_StatusMessage.UNDERCURRENT:
                    return $"Phase {DecimalNumber(dataValue1)} under current at {DecimalNumber(dataValue2)} A.";
                case ERDM_StatusMessage.PHASE:
                    return $"Phase {DecimalNumber(dataValue1)} is at {DecimalNumber(dataValue2)} degrees.";
                case ERDM_StatusMessage.PHASE_ERROR:
                    return $"Phase {DecimalNumber(dataValue1)} Error.";
                case ERDM_StatusMessage.AMPS:
                    return $"{DecimalNumber(dataValue1)} A.";
                case ERDM_StatusMessage.VOLTS:
                    return $"{DecimalNumber(dataValue1)} V.";

                case ERDM_StatusMessage.DIMSLOT_OCCUPIED:
                    return "No Dimmer.";
                case ERDM_StatusMessage.BREAKER_TRIP:
                    return "Tripped Breaker.";
                case ERDM_StatusMessage.WATTS:
                    return $"{DecimalNumber(dataValue1)} W.";
                case ERDM_StatusMessage.DIM_FAILURE:
                    return "Dimmer Failure.";
                case ERDM_StatusMessage.DIM_PANIC:
                    return "Panic Mode.";
                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.LOAD_FAILURE:
                    return "Lamp or cable failure.";
                #endregion

                case ERDM_StatusMessage.READY:
                    return $"{SlotLabelCode(dataValue1)} ready.";
                case ERDM_StatusMessage.NOT_READY:
                    return $"{SlotLabelCode(dataValue1)} not ready.";
                case ERDM_StatusMessage.LOW_FLUID:
                    return $"{SlotLabelCode(dataValue1)} low fluid.";

                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.EEPROM_ERROR:
                    return $"EEPROM error.";
                case ERDM_StatusMessage.RAM_ERROR:
                    return $"RAM error.";
                case ERDM_StatusMessage.FPGA_ERROR:
                    return $"FPGA programming error.";
                #endregion

                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.PROXY_BROADCAST_DROPPED:
                    ERDM_Parameter pid = (ERDM_Parameter)(ushort)DecimalNumber(dataValue1);
                    return $"Proxy Drop: PID 0x{(ushort)pid:X4}({pid}) at Transaction Number {DecimalNumber(dataValue2)}.";
                case ERDM_StatusMessage.ASC_RXOK:
                    return $"DMX ASC {DecimalNumber(dataValue1)} received OK.";
                case ERDM_StatusMessage.ASC_DROPPED:
                    return $"DMX ASC {DecimalNumber(dataValue1)} received dropped.";
                #endregion

                #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
                case ERDM_StatusMessage.DMXNSCNONE:
                    return $"DMX NSC never received.";
                case ERDM_StatusMessage.DMXNSCLOSS:
                    return $"DMX NSC received, now dropped.";
                case ERDM_StatusMessage.DMXNSCERROR:
                    return $"DMX NSC timing, or packet error.";
                case ERDM_StatusMessage.DMXNSC_OK:
                    return $"DMX NSC received OK.";
                #endregion

                default:
                    return string.Empty;
            }

            //Local Functions
            short DecimalNumber(short d) => d;
            string SlotLabelCode(short s)
            {
                ERDM_SlotCategory sc = (ERDM_SlotCategory)(ushort)s;
                return GetEnumDescription(sc);
            }
        }
        public static string GetEnumDescription(Enum value)
        {
            DescriptionAttribute attribute = null;
            
                if (value == null) { return ""; }
            try
            {
                attribute = value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .SingleOrDefault() as DescriptionAttribute;
            }
            catch { }
            return attribute?.Description ?? value.ToString();
        }

        public static byte[] ValueToData(params bool[] bits)
        {
            return ValueToData(bits, trim: 0);
        }
        public static byte[] ValueToData(object value, int trim = 32)
        {
            if (value == null)
                return new byte[0];

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
                case RDMUID[] @uidArray:
                    List<byte> lBytes = new List<byte>();
                    foreach (RDMUID uid in @uidArray)
                        lBytes.AddRange(uid.ToBytes());
                    return lBytes.ToArray();
                case RDMUID @uid:
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
        public static RDMUID DataToRDMUID(ref byte[] data)
        {
            const int length = 6;
            if (data.Length < length)
                throw new IndexOutOfRangeException();

            return new RDMUID(DataToUShort(ref data), DataToUInt(ref data)); ;
        }

        public static string DataToString(ref byte[] data, int take = 0)
        {
            string res = null;
            if (take != 0)
            {
                res = Encoding.UTF8.GetString(data.Take(take).ToArray());
                data = data.Skip(take).ToArray();
            }
            else
            {
                res = Encoding.UTF8.GetString(data);
                data = new byte[0];
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
            if (list == null) throw new ArgumentNullException(nameof(list));

            if (list is List<T> instance)
                return instance.AsReadOnly();
            return new ReadOnlyCollection<T>(list);
        }
        public static IReadOnlyDictionary<K, V> AsReadOnly<K, V>(this IDictionary<K, V> source)
        {
            if (source == null) throw new ArgumentNullException("source");

            return new ReadOnlyDictionary<K, V>(source);
        }
    }
}
