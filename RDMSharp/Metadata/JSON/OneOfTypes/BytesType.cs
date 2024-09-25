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
            switch (Format)
            {
                case "mac-address":
                case "uid":
                    return new PDL(6);
                case "ipv4":
                case "float":
                    return new PDL(4);
                case "ipv6":
                case "uuid":
                case "guid":
                    return new PDL(16);
                case "double":
                    return new PDL(8);
                case "pid":
                    return new PDL(2);
            }
            return new PDL((uint)(MinLength ?? 1), (uint)(MaxLength ?? PDL.MAX_LENGTH));
        }
        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            switch (Format)
            {
                //Known from E1.37-5 (2024)
                case "uid" when dataTree.Value is UID uid:
                    return uid.ToBytes().ToArray();
                case "ipv4" when dataTree.Value is IPv4Address ipv4:
                    return (byte[])ipv4;
                case "ipv6" when dataTree.Value is IPv6Address ipv6:
                    return (byte[])ipv6;
                case "mac-address" when dataTree.Value is MACAddress macAddress:
                    return (byte[])macAddress;

                //Known from E1.37-5 (2024) as uuid
                case "uuid" when dataTree.Value is Guid uuid:
                    return uuid.ToByteArray();
                case "guid" when dataTree.Value is Guid guid:
                    return guid.ToByteArray();

                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "pid" when dataTree.Value is ERDM_Parameter pid:
                    return Tools.ValueToData(pid);

                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "double" when dataTree.Value is double _double:
                    return BitConverter.GetBytes(_double);
                case "float" when dataTree.Value is float _float:
                    return BitConverter.GetBytes(_float);


                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "ascii" when dataTree.Value is string ascii:
                    return Encoding.ASCII.GetBytes(ascii);
                case "utf8" when dataTree.Value is string utf8:
                    return Encoding.UTF8.GetBytes(utf8);
                case "utf32" when dataTree.Value is string utf32:
                    return Encoding.UTF32.GetBytes(utf32);
                case "unicode" when dataTree.Value is string unicode:
                    return Encoding.Unicode.GetBytes(unicode);
                case "big_edian_unicode" when dataTree.Value is string big_edian_unicode:
                    return Encoding.BigEndianUnicode.GetBytes(big_edian_unicode);
                case "latin1" when dataTree.Value is string latin1:
                    return Encoding.Latin1.GetBytes(latin1);

                //Fallback
                default:
                    if (dataTree.Value is string str)
                        return Encoding.UTF8.GetBytes(str);
                    if (dataTree.Value is string[] strArray)
                        return Encoding.UTF8.GetBytes(string.Join((char)0, strArray));
                    if (dataTree.Value is byte[] byteArray)
                        return byteArray;
                    break;
            }

            throw new ArithmeticException($"The given Object of {nameof(Format)}: \"{Format}\" can't be parsed from {nameof(dataTree.Value)}: {dataTree.Value}");
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            object value = null;
            switch (Format)
            {
                //Known from E1.37-5 (2024)
                case "uid":
                    value = new UID(Tools.DataToUShort(ref data), Tools.DataToUInt(ref data));
                    break;
                case "ipv4":
                    value = new IPv4Address(data.Take(4));
                    data = data.Skip(4).ToArray();
                    break;
                case "ipv6":
                    value = new IPv6Address(data.Take(16));
                    data = data.Skip(16).ToArray();
                    break;
                case "mac-address":
                    value = new MACAddress(data.Take(16));
                    data = data.Skip(6).ToArray();
                    break;

                //Known from E1.37-5 (2024) as uuid
                case "uuid":
                case "guid":
                    value = new Guid(data.Take(16).ToArray());
                    data = data.Skip(16).ToArray();
                    break;

                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "pid":
                    value = Tools.DataToEnum<ERDM_Parameter>(ref data);
                    break;

                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "double":
                    value = BitConverter.ToDouble(data, 0);
                    data = data.Skip(8).ToArray();
                    break;
                case "float":
                    value = BitConverter.ToSingle(data, 0);
                    data = data.Skip(4).ToArray();
                    break;


                //Additional added, because there is no fancy way to di this with E1.37-5 (2024)
                case "ascii":
                    value = Encoding.ASCII.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;
                case "utf8":
                    value = Encoding.UTF8.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;
                case "utf32":
                    value = Encoding.UTF32.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;
                case "unicode":
                    value = Encoding.Unicode.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;
                case "big_edian_unicode":
                    value = Encoding.BigEndianUnicode.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;
                case "latin1":
                    value=Encoding.Latin1.GetString(data);
                    data = data.Skip(data.Length).ToArray();
                    break;

                //Fallback
                default:
                    value = data;
                    data = data.Skip(data.Length).ToArray();
                    issueList.Add(new DataTreeIssue($"No Parser found for {nameof(Format)}: \"{Format}\""));
                    break;
            }
            return new DataTree(this.Name, 0, value, issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}
