using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleBooleanModule : AbstractSimpleModule
{
    private bool initialized = false;
    private bool _value;
    protected bool? Value
    {
        get
        {
            if (ParentDevice is null)
                return _value;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ModuleParameter, out res))
                return (bool)res;
            return _value;
        }
        set
        {
            if (value is null)
                return;

            if (initialized)
                if (bool.Equals(_value, value.Value))
                    return;

            _value = value.Value;
            initialized = true;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ModuleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetValue(value.Value);
        }
    }

    private bool _initialValue;
    public AbstractSimpleBooleanModule(
        bool initialValue,
        string name,
        ERDM_Parameter supportedParameter,
        ERDM_CommandClass supportedCommandClass) : base(
            name,
            supportedParameter,
            supportedCommandClass)
    {
        _initialValue = initialValue;
    }
    public AbstractSimpleBooleanModule(
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

    protected async Task<bool> SetValue(bool value)
    {
        return await ParentRemoteDevice.SetParameter(ModuleParameter, value);
    }
}