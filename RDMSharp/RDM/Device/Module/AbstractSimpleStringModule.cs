using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleStringModule : AbstractSimpleModule
{
    private bool initialized = false;
    private string _value;
    protected string Value
    {
        get
        {
            if (ParentDevice is null)
                return _value;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ModuleParameter, out res))
                return (string)res;
            return _value;
        }
        set
        {
            if (value is null)
                return;

            if (initialized)
                if (string.Equals(_value, value))
                    return;

            _value = value;
            initialized = true;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ModuleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetValue(value);
        }
    }

    private string _initialValue;
    public AbstractSimpleStringModule(
        string initialValue,
        string name,
        ERDM_Parameter supportedParameter,
        ERDM_CommandClass supportedCommandClass) : base(
            name,
            supportedParameter,
            supportedCommandClass)
    {
        _initialValue = initialValue;
    }
    public AbstractSimpleStringModule(
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

    protected async Task<bool> SetValue(string value)
    {
        return await ParentRemoteDevice.SetParameter(ModuleParameter, value);
    }
}