namespace RDMSharp.RDM.Device.Module;

public abstract class AbstractSimpleModule : AbstractModule
{
    protected ERDM_Parameter ModuleParameter { get; private set; }

    public ERDM_CommandClass SupportedCommandClass { get; }

    /// <summary>The one parameter of the module.</summary>
    public ERDM_Parameter Parameter => ModuleParameter;
    /// <summary>The current value, read only and boxed (e.g. for a table with one column per module); <c>null</c> until it is known.</summary>
    public abstract object CurrentValue { get; }
    /// <summary>The current value as text (the label of the value, if it has one); <c>null</c> until it is known.</summary>
    public virtual string ValueText => CurrentValue?.ToString();

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == "Value")
        {
            base.OnPropertyChanged(nameof(CurrentValue));
            base.OnPropertyChanged(nameof(ValueText));
        }
    }

    public AbstractSimpleModule(string name, ERDM_Parameter supportedParameter, ERDM_CommandClass supportedCommandClass) : base(name, supportedParameter)
    {
        ModuleParameter = supportedParameter;
        SupportedCommandClass = supportedCommandClass;
    }
    public AbstractSimpleModule(AbstractRemoteRDMDevice remoteDevice, string name, ERDM_Parameter supportedParameter, ERDM_CommandClass supportedCommandClass) : base(
        remoteDevice,
        name,
        supportedParameter)
    {
        ModuleParameter = supportedParameter;
        SupportedCommandClass = supportedCommandClass;
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ModuleParameter)
        {
            if (command == ERDM_Command.GET_COMMAND && !SupportedCommandClass.HasFlag(ERDM_CommandClass.GET))
                return true;
            if (command == ERDM_Command.SET_COMMAND && !SupportedCommandClass.HasFlag(ERDM_CommandClass.SET))
                return true;
        }
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if ((message.Command == ERDM_Command.GET_COMMAND && !SupportedCommandClass.HasFlag(ERDM_CommandClass.GET)) ||
           (message.Command == ERDM_Command.SET_COMMAND && !SupportedCommandClass.HasFlag(ERDM_CommandClass.SET)))
        {
            return new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS)
            {
                SourceUID = message.DestUID,
                DestUID = message.SourceUID,
                Command = message.Command | ERDM_Command.RESPONSE,
                Parameter = message.Parameter
            };
        }
        return base.handleRequest(message);
    }
}