using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleBooleanModule : AbstractModule
{
    protected ERDM_Parameter ModuleParameter { get; private set; }
    private bool _value;
    public bool? Value
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

            if (bool.Equals(_value, value.Value))
                return;

            _value = value.Value;
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ModuleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetValue(value.Value);
        }
    }

    private bool _initialValue;
    public AbstractSimpleBooleanModule(bool initialValue, string name, ERDM_Parameter supportedParameter) : base(name, supportedParameter)
    {
        _initialValue = initialValue;
        _value = !initialValue;
        ModuleParameter = supportedParameter;
    }
    public AbstractSimpleBooleanModule(AbstractRemoteRDMDevice remoteDevice, string name, ERDM_Parameter supportedParameter) : base(
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

    public async Task<bool> SetValue(bool value)
    {
        return await ParentRemoteDevice.SetParameter(ModuleParameter, value);
    }
}