using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes;

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

    private readonly bool formatIsArray;
    private readonly string baseFormat;


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

        formatIsArray = Format?.EndsWith("[]") ?? false;
        if (formatIsArray)
            baseFormat = Format.Replace("[]", "");
        else
            baseFormat = Format;
    }

    public override string ToString()
    {
        return Name;
    }

    public override PDL GetDataLength()
    {
        uint length = 0;
        bool noFixedSize = false;
        if (Extensions.ExtensionsManager.Instance.TryGetBytesParser(baseFormat, out Extensions.IBytesParser bytesParser))
        {
            if (!bytesParser.PayloadDataLength.HasValue)
            {
                noFixedSize = true;
                goto END;
            }
            var pdl = bytesParser.PayloadDataLength.Value;

            if (pdl.Value.HasValue && formatIsArray)
            {
                length = pdl.Value.Value;
                goto END;
            }
            return pdl;
        }
        noFixedSize = true;
    END:
        if (formatIsArray && length != 0)
            return new PDL(0, (uint)(Math.Truncate((double)PDL.MAX_LENGTH / length) * length));

        return new PDL((uint)(MinLength ?? 1), (uint)(MaxLength ?? PDL.MAX_LENGTH));
    }
    public override IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
    {
        if (!string.Equals(dataTree.Name, this.Name))
            throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

        if (formatIsArray && dataTree.Value is Array typedArray)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < typedArray.Length; i++)
            {
                object value = typedArray.GetValue(i);
                bytes.AddRange(parseData(baseFormat, value));
                if (value is string)
                    bytes.Add(0); //Null-Delimiter
            }
            return Tools.EncaseData(bytes.ToArray());
        }
        else
            return Tools.EncaseData(parseData(baseFormat, dataTree.Value));

        byte[] parseData(string format, object value)
        {
            Exception e = null;
            try
            {
                if (Extensions.ExtensionsManager.Instance.TryGetBytesParser(format, out Extensions.IBytesParser bytesParser))
                    return bytesParser.ParseToData(value);

                if (value is string str)
                    return Encoding.ASCII.GetBytes(str);
                if (value is IReadOnlyCollection<string> strings)
                {
                    List<byte> bytes = new List<byte>();
                    foreach (string _str in strings)
                    {
                        bytes.AddRange(Encoding.ASCII.GetBytes(_str));
                        bytes.Add(0x00);
                    }
                    return bytes.ToArray();
                }
                if (value is byte[] byteArray)
                    return byteArray;
                throw new NotImplementedException($"There is no implementation for {nameof(format)}: {format} and Value: {value}");
            }
            catch (Exception ex)
            {
                e = ex;
            }
            throw new ArithmeticException($"The given Object of {nameof(format)}: \"{format}\" can't be parsed from {nameof(value)}: {value}", e);
        }
    }
    public override DataTree ParseDataToPayload(ref byte[] data)
    {
        List<DataTreeIssue> issueList = new List<DataTreeIssue>();
        object value = null;
        if (formatIsArray)
        {
            List<object> list = new List<object>();
            while (data.Length > 0)
            {
                try
                {
                    list.Add(parseData(baseFormat, ref data));
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
            value = parseData(baseFormat, ref data);


        return new DataTree(this.Name, 0, value, issueList.Count != 0 ? issueList.ToArray() : null);

        object parseData(string format, ref byte[] data)
        {
            if (Extensions.ExtensionsManager.Instance.TryGetBytesParser(format, out Extensions.IBytesParser bytesParser))
                return bytesParser.ParseToObject(ref data);

            object value = null;
            value = data;
            data = data.Skip(data.Length).ToArray();
            issueList.Add(new DataTreeIssue($"No Parser found for {nameof(format)}: \"{format}\""));
            return value;
        }
    }
}
