using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module.Generic;

/// <summary>
/// A parameter with one integer field, built from its define. <typeparamref name="T"/> is the type of the field (byte for uint8, ...),
/// the value of the device is stored with exactly this type.
/// </summary>
public class GenericNumericModule<T> : AbstractSimpleNumericModule<T>, IGenericModule where T : struct, IComparable, IConvertible, IFormattable
{
    public MetadataJSONObjectDefine Define { get; }
    public override string DisplayName { get; }
    public T MinValue { get; }
    public T MaxValue { get; }
    /// <summary>Unit of the value as the define gives it, the value itself is the raw value of the device.</summary>
    public ERDM_SensorUnit? Unit { get; }
    /// <summary>
    /// The labels of the define (value and text), empty if it has none. Without <see cref="RestrictToLabeled"/> they name special values
    /// of a number (e.g. "no footprint" of the DMX start address), the value is not limited to them.
    /// </summary>
    public IReadOnlyList<KeyValuePair<long, string>> Labels { get; }
    /// <summary>The define allows only the labeled values.</summary>
    public bool RestrictToLabeled { get; }

    public new T? Value
    {
        get => base.Value;
        set => base.Value = value;
    }

    internal GenericNumericModule(AbstractRemoteRDMDevice remoteDevice, MetadataJSONObjectDefine define, ERDM_Parameter parameter, ERDM_CommandClass commandClass, IntegerType<T> field)
        : base(remoteDevice, GenericModuleFactory.ModuleName(define, parameter), parameter, commandClass)
    {
        Define = define;
        DisplayName = GenericModuleFactory.DisplayNameOf(define);
        MinValue = (T)field.GetMinimum();
        MaxValue = (T)field.GetMaximum();
        Unit = field.Units;
        Labels = (field.Labels ?? Array.Empty<LabeledIntegerType>()).Select(l => new KeyValuePair<long, string>(l.Value, l.DisplayName ?? l.Name)).ToList().AsReadOnly();
        RestrictToLabeled = field.RestrictToLabeled ?? false;
    }

    public Task<bool> SetValueAsync(T value) => SetValue(value);
}

/// <summary>
/// A parameter with one integer field that allows only the labeled values of its define (e.g. 0 = Off, 1 = On): a choice, not a number.
/// </summary>
public sealed class GenericLabeledIntegerModule<T> : GenericNumericModule<T> where T : struct, IComparable, IConvertible, IFormattable
{
    internal GenericLabeledIntegerModule(AbstractRemoteRDMDevice remoteDevice, MetadataJSONObjectDefine define, ERDM_Parameter parameter, ERDM_CommandClass commandClass, IntegerType<T> field)
        : base(remoteDevice, define, parameter, commandClass, field)
    {
    }
}
