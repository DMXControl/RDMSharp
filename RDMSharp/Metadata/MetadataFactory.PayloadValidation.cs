using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.RDM;
using System;
using System.Linq;

namespace RDMSharp.Metadata;

/// <summary>
/// Result of <see cref="MetadataFactory.ValidatePayload"/>.
/// </summary>
public enum EPayloadValidation
{
    /// <summary>The Parameter Data fits the define (or the define can not be checked).</summary>
    Valid,
    /// <summary>The length does not fit the define: too short, too long, incomplete list item or text too long (NACK FORMAT_ERROR).</summary>
    LengthMismatch,
    /// <summary>A boolean is not 0x00 or 0x01 (NACK DATA_OUT_OF_RANGE).</summary>
    InvalidBoolean
}

public static partial class MetadataFactory
{
    /// <summary>
    /// Lengths the specification allows in addition to the define, the JSON schema can not describe optional fields.
    /// </summary>
    private static readonly (ERDM_Parameter Parameter, Command.ECommandDublicate CommandType, int Length)[] ADDITIONAL_VALID_LENGTHS = new[]
    {
        // E1.20 10.11.6: PDL 0x02 (Scene # only) or 0x08 (Scene # and times)
        (ERDM_Parameter.CAPTURE_PRESET, Command.ECommandDublicate.SetRequest, 2),
    };

    /// <summary>
    /// Checks the Parameter Data against the define before it is parsed, so a faulty Payload is not interpreted:
    /// <list type="bullet">
    /// <item>the length has to fit the fields of the define (fixed sizes, PDL range, complete list items, text length),</item>
    /// <item>a boolean may only be 0x00 or 0x01.</item>
    /// </list>
    /// A layout that can not be checked (e.g. a variable field in front of other fields) is reported as <see cref="EPayloadValidation.Valid"/>.
    /// </summary>
    /// <param name="reason">Why the Parameter Data does not fit, <c>null</c> if it is valid</param>
    public static EPayloadValidation ValidatePayload(MetadataJSONObjectDefine define, Command.ECommandDublicate commandType, byte[] data, out string reason)
    {
        reason = null;
        data ??= Array.Empty<byte>();
        if (define is null)
            return EPayloadValidation.Valid;

        define.GetCommand(commandType, out Command? cmd);
        if (cmd is null || !cmd.HasValue)
            return EPayloadValidation.Valid; // nothing known to check against

        Command command = cmd.Value;
        if (ADDITIONAL_VALID_LENGTHS.Any(a => (ushort)a.Parameter == define.PID && define.ManufacturerID == 0 && a.CommandType == commandType && a.Length == data.Length))
            return EPayloadValidation.Valid;

        if (command.GetIsEmpty())
        {
            if (data.Length == 0)
                return EPayloadValidation.Valid;
            reason = $"PDL {data.Length}, the define has no Parameter Data";
            return EPayloadValidation.LengthMismatch;
        }

        PDL pdl = command.GetDataLength();
        if (!pdl.IsValid(data.Length))
        {
            reason = pdl.Value.HasValue
                ? $"PDL {data.Length}, the define needs {pdl.Value}"
                : $"PDL {data.Length}, the define allows {pdl.MinLength} - {pdl.MaxLength}";
            return EPayloadValidation.LengthMismatch;
        }

        OneOfTypes[] fields = command.SingleField.HasValue ? new[] { command.SingleField.Value } : command.ListOfFields ?? Array.Empty<OneOfTypes>();
        try
        {
            int position = 0;
            var result = validateFields(fields.Select(resolve).ToArray(), data, ref position, true, ref reason);
            if (result != EPayloadValidation.Valid)
                return result;
        }
        catch (NotSupportedException)
        {
            reason = null; // the layout can not be checked
        }
        return EPayloadValidation.Valid;
    }

    private static CommonPropertiesForNamed resolve(OneOfTypes oneOf)
    {
        if (oneOf.ObjectType is not null)
            return oneOf.ObjectType;
        if (oneOf.ReferenceType.HasValue)
            return oneOf.ReferenceType.Value.ReferencedObject;
        return null;
    }

    /// <param name="mayTakeRest">The last field may be a variable field that takes the rest of the data (only at the end of the Parameter Data).</param>
    private static EPayloadValidation validateFields(CommonPropertiesForNamed[] fields, byte[] data, ref int position, bool mayTakeRest, ref string reason)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i] ?? throw new NotSupportedException();
            bool takesRest = mayTakeRest && i == fields.Length - 1;
            switch (field)
            {
                case BooleanType:
                case LabeledBooleanType:
                    if (position + 1 > data.Length)
                    {
                        reason = $"PDL {data.Length}, {field.Name} is missing";
                        return EPayloadValidation.LengthMismatch;
                    }
                    if (data[position] > 1)
                    {
                        reason = $"Boolean {field.Name} is 0x{data[position]:X2}, only 0x00 and 0x01 are valid";
                        return EPayloadValidation.InvalidBoolean;
                    }
                    position++;
                    break;

                case CompoundType compound:
                    {
                        var result = validateFields(compound.Subtypes.Select(resolve).ToArray(), data, ref position, takesRest, ref reason);
                        if (result != EPayloadValidation.Valid)
                            return result;
                        break;
                    }

                case ListType list:
                    {
                        if (!takesRest)
                            throw new NotSupportedException();
                        var item = resolve(list.ItemType) ?? throw new NotSupportedException();
                        var itemFields = item is CompoundType itemCompound ? itemCompound.Subtypes.Select(resolve).ToArray() : new[] { item };
                        // the number of items is limited by the PDL of the list (min/max items x size of an item), checked before
                        while (position < data.Length)
                        {
                            var result = validateFields(itemFields, data, ref position, false, ref reason);
                            if (result != EPayloadValidation.Valid)
                                return result;
                        }
                        break;
                    }

                default:
                    {
                        PDL size = field.GetDataLength();
                        uint? fixedSize = size.Value ?? (size.MinLength.HasValue && size.MinLength == size.MaxLength ? size.MinLength : null);
                        if (fixedSize.HasValue)
                        {
                            if (position + fixedSize.Value > data.Length)
                            {
                                reason = $"PDL {data.Length}, {field.Name} needs {fixedSize} bytes, but only {data.Length - position} are left";
                                return EPayloadValidation.LengthMismatch;
                            }
                            position += (int)fixedSize.Value;
                            break;
                        }
                        // a variable field (e.g. a text) takes the rest of the data, its length is covered by the PDL of the define
                        if (!takesRest)
                            throw new NotSupportedException();
                        position = data.Length;
                        break;
                    }
            }
        }
        return EPayloadValidation.Valid;
    }
}
