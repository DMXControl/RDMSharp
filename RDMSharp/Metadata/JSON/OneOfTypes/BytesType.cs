using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BytesType : CommonPropertiesForNamed
    {
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public override string Name { get; }
        [JsonPropertyName("displayName")]
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string DisplayName { get; }
        [JsonPropertyName("notes")]
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string Notes { get; }
        [JsonPropertyName("resources")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string[] Resources { get; }

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public string Type { get; }
        [JsonPropertyName("format")]
        [JsonPropertyOrder(11)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Format { get; }
        [JsonPropertyName("minLength")]
        [JsonPropertyOrder(12)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MinLength { get; }
        [JsonPropertyName("maxLength")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MaxLength { get; }


        [JsonConstructor]
        public BytesType(string name,
                         string displayName,
                         string notes,
                         string[] resources,
                         string type,
                         string format,
                         uint? minLength,
                         uint? maxLength) : base()
        {
            if (!"bytes".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"bytes\"");
            if (minLength.HasValue && maxLength.HasValue)
                if (minLength > maxLength)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(minLength)} has to be <= {nameof(maxLength)}");
            if (minLength.HasValue)
                if (minLength > PDL.MAX_LENGTH)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(minLength)} has to be <= {PDL.MAX_LENGTH}");
            if (maxLength.HasValue)
                if (maxLength > PDL.MAX_LENGTH)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(maxLength)} has to be <= {PDL.MAX_LENGTH}");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Format = format;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public override string ToString()
        {
            return Name;
        }

        public override PDL GetDataLength()
        {
            uint length = 0;
            string format = Format;
            bool noFixedSize = false;
            if (!string.IsNullOrWhiteSpace(Format) && Format.EndsWith("[]"))
                format = Format.Replace("[]", "");
            switch (format)
            {
                case "mac-address":
                case "uid":
                    length = 6;
                    break;
                case "ipv4":
                case "float":
                    length = 4;
                    break;
                case "ipv6":
                case "uuid":
                case "guid":
                    length = 16;
                    break;
                case "double":
                    length = 8;
                    break;
                case "pid":
                    length = 2;
                    break;
                default:
                    noFixedSize = true;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(Format) && Format.EndsWith("[]"))
                return new PDL(0, (uint)(Math.Truncate((double)PDL.MAX_LENGTH / length) * length));
            else if (!noFixedSize)
                return new PDL(length);

            return new PDL((uint)(MinLength ?? 1), (uint)(MaxLength ?? PDL.MAX_LENGTH));
        }
        public override IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            if (!string.IsNullOrWhiteSpace(Format) && Format.EndsWith("[]") && dataTree.Value is Array typedArray)
            {
                List<byte> bytes = new List<byte>();
                string format = Format.Replace("[]", "");
                for (int i = 0; i < typedArray.Length; i++)
                {
                    object value = typedArray.GetValue(i);
                    bytes.AddRange(parseData(format, value));
                    if (value is string)
                        bytes.Add(0); //Null-Delimiter
                }
                return Tools.EncaseData(bytes.ToArray());
            }
            else
                return Tools.EncaseData(parseData(Format, dataTree.Value));

            byte[] parseData(string format, object value)
            {
                Exception e = null;
                try
                {
                    switch (format)
                    {
                        //Known from E1.37-5 (2024)
                        case "uid" when value is UID uid:
                            return uid.ToBytes().ToArray();
                        case "ipv4" when value is IPv4Address ipv4:
                            return (byte[])ipv4;
                        case "ipv6" when value is IPv6Address ipv6:
                            return (byte[])ipv6;
                        case "mac-address" when value is MACAddress macAddress:
                            return (byte[])macAddress;

                        //Known from E1.37-5 (2024) as uuid
                        case "uuid" when value is Guid uuid:
                            return uuid.ToByteArray();
                        case "guid" when value is Guid guid:
                            return guid.ToByteArray();

                        //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                        case "pid" when value is ERDM_Parameter pid:
                            return Tools.ValueToData(pid);

                        //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                        case "double" when value is double _double:
                            return BitConverter.GetBytes(_double);
                        case "float" when value is float _float:
                            return BitConverter.GetBytes(_float);


                        //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                        case "ascii" when value is string ascii:
                            return Encoding.ASCII.GetBytes(ascii);
                        case "utf8" when value is string utf8:
                            return Encoding.UTF8.GetBytes(utf8);
                        case "utf32" when value is string utf32:
                            return Encoding.UTF32.GetBytes(utf32);
                        case "unicode" when value is string unicode:
                            return Encoding.Unicode.GetBytes(unicode);
                        case "big_edian_unicode" when value is string big_edian_unicode:
                            return Encoding.BigEndianUnicode.GetBytes(big_edian_unicode);
                        case "latin1" when value is string latin1:
                            return Encoding.Latin1.GetBytes(latin1);

                        //Fallback
                        default:
                            if (value is string str)
                                return Encoding.UTF8.GetBytes(str);
                            if (value is byte[] byteArray)
                                return byteArray;
                            throw new NotImplementedException($"There is no implementation for {nameof(Format)}: {Format} and Value: {value}");
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                }
                throw new ArithmeticException($"The given Object of {nameof(Format)}: \"{Format}\" can't be parsed from {nameof(value)}: {value}", e);
            }
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            object value = null;
            if (!string.IsNullOrWhiteSpace(Format) && Format.EndsWith("[]"))
            {
                List<object> list = new List<object>();
                while (data.Length > 0)
                {
                    try
                    {
                        string format = Format.Replace("[]", "");
                        list.Add(parseData(format, ref data));
                    }
                    catch (Exception e)
                    {
                        issueList.Add(new DataTreeIssue(e.Message));
                        break;
                    }
                }
                if (data.Length > 0)
                    issueList.Add(new DataTreeIssue("Data Length is not 0"));

                if (list.Count == 0)
                    value = null;
                else
                {
                    Type targetType = list.First().GetType();
                    var array = Array.CreateInstance(targetType, list.Count);
                    for (int i = 0; i < list.Count; i++)
                        array.SetValue(Convert.ChangeType(list[i], targetType), i);

                    value = array;
                }
            }
            else
                value = parseData(Format, ref data);


            return new DataTree(this.Name, 0, value, issueList.Count != 0 ? issueList.ToArray() : null);

            object parseData(string format, ref byte[] data)
            {
                void validateDataLength(int length, ref byte[] data)
                {
                    if (data.Length < length)
                        throw new ArithmeticException("Data to short");
                }
                object value = null;
                switch (format)
                {
                    //Known from E1.37-5 (2024)
                    case "uid":
                        validateDataLength(6, ref data);
                        value = new UID(Tools.DataToUShort(ref data), Tools.DataToUInt(ref data));
                        break;
                    case "ipv4":
                        validateDataLength(4, ref data);
                        value = new IPv4Address(data.Take(4));
                        data = data.Skip(4).ToArray();
                        break;
                    case "ipv6":
                        validateDataLength(16, ref data);
                        value = new IPv6Address(data.Take(16));
                        data = data.Skip(16).ToArray();
                        break;
                    case "mac-address":
                        validateDataLength(6, ref data);
                        value = new MACAddress(data.Take(6));
                        data = data.Skip(6).ToArray();
                        break;

                    //Known from E1.37-5 (2024) as uuid
                    case "uuid":
                    case "guid":
                        validateDataLength(16, ref data);
                        value = new Guid(data.Take(16).ToArray());
                        data = data.Skip(16).ToArray();
                        break;

                    //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                    case "pid":
                        validateDataLength(2, ref data);
                        value = Tools.DataToEnum<ERDM_Parameter>(ref data);
                        break;

                    //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                    case "double":
                        validateDataLength(8, ref data);
                        value = BitConverter.ToDouble(data.Take(8).ToArray(), 0);
                        data = data.Skip(8).ToArray();
                        break;
                    case "float":
                        validateDataLength(4, ref data);
                        value = BitConverter.ToSingle(data.Take(4).ToArray(), 0);
                        data = data.Skip(4).ToArray();
                        break;


                    //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                    case "ascii":
                        value = getNullDelimitetData(Encoding.ASCII, ref data);
                        break;
                    case "utf8":
                        value = getNullDelimitetData(Encoding.UTF8, ref data);
                        break;
                    case "utf32":
                        value = getNullDelimitetData(Encoding.UTF32, ref data);
                        break;
                    case "unicode":
                        value = getNullDelimitetData(Encoding.Unicode, ref data);
                        break;
                    case "big_edian_unicode":
                        value = getNullDelimitetData(Encoding.BigEndianUnicode, ref data);
                        break;
                    case "latin1":
                        value = getNullDelimitetData(Encoding.Latin1, ref data);
                        break;

                    //Fallback
                    default:
                        value = data;
                        data = data.Skip(data.Length).ToArray();
                        issueList.Add(new DataTreeIssue($"No Parser found for {nameof(Format)}: \"{Format}\""));
                        break;
                }
                return value;

                string getNullDelimitetData(Encoding encoding, ref byte[] data)
                {
                    string res = encoding.GetString(data);
                    if (res.Contains('\0'))
                    {
                        res = res.Split('\0')[0];
                        int count = encoding.GetByteCount(res + "\0");
                        data = data.Skip(count).ToArray();
                    }
                    else
                        data = new byte[0];
                    return res;
                }
            }
        }
    }
}
