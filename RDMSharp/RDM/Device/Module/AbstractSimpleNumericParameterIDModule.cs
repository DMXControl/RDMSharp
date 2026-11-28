using System;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleNumericParameterIDModule<T> : AbstractSimpleParameterIDModule where T : struct, IComparable, IConvertible, IFormattable
{
    public override object CurrentValue => Value;
    private bool initialized = false;
    private T _value;
    protected T? Value
    {
        get
        {
            if (ParentDevice is null)
                return _value;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ModuleParameter, out object res))
                return (T)res;
            return _value;
        }
        set
        {
            if (value is null)
                return;

            // compare with the stored value of the device, the field is outdated after a SET by RDM
            if (initialized)
                if (Object.Equals(Value, value.Value))
                    return;

            _value = value.Value;
            initialized = true;
            ParentGeneratedDevice?.setParameterValue(ModuleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetValue(value.Value);
        }
    }

    private readonly T _initialValue;
    public AbstractSimpleNumericParameterIDModule(
        T initialValue,
        string name,
        ERDM_Parameter supportedParameter,
        ERDM_CommandClass supportedCommandClass) : base(
            name,
            supportedParameter,
            supportedCommandClass)
    {
        _initialValue = initialValue;
    }
    public AbstractSimpleNumericParameterIDModule(
        AbstractRemoteRDMDevice remoteDevice,
        string name,
        ERDM_Parameter supportedParameter,
        ERDM_CommandClass supportedCommandClass) : base(
            remoteDevice,
            name,
            supportedParameter,
            supportedCommandClass)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.Value = _initialValue;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (parameter == ModuleParameter)
            OnPropertyChanged(nameof(Value));
    }

    protected async Task<bool> SetValue(T value)
    {
        return await ParentRemoteDevice.SetParameter(ModuleParameter, value);
    }
}