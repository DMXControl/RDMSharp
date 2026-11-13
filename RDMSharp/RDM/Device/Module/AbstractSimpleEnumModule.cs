using System;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleEnumModule<T> : AbstractModule where T : struct, Enum
{
    protected ERDM_Parameter ModuleParameter { get; private set; }
    private T _value;
    public T? Value
    {
        get
        {
            if (ParentDevice is null)
                return _value;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ModuleParameter, out res))
                return (T)res;
            return _value;
        }
        set
        {
            if (value is null)
                return;

            if (Object.Equals(_value, value.Value))
                return;

            _value = value.Value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ModuleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetValue(value.Value);
        }
    }

    private T _initialValue;
    public AbstractSimpleEnumModule(T initialValue, string name, ERDM_Parameter supportedParameter) : base(name, supportedParameter)
    {
        _initialValue = initialValue;
        ModuleParameter = supportedParameter;
    }
    public AbstractSimpleEnumModule(AbstractRemoteRDMDevice remoteDevice, string name, ERDM_Parameter supportedParameter) : base(
        remoteDevice,
        name,
        supportedParameter)
    {
        ModuleParameter = supportedParameter;
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

    public async Task<bool> SetValue(T value)
    {
        return await ParentRemoteDevice.SetParameter(ModuleParameter, value);
    }
}