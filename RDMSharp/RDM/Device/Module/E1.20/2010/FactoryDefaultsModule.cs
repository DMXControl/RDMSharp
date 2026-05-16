using RDMSharp.Metadata;
using System;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class FactoryDefaultsModule : AbstractModule
{
    private const string _moduleName = "FactoryDefaults";
    private const string _moduleDisplayName = "Factory Defaults";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.FACTORY_DEFAULTS;

    public override string DisplayName => _moduleDisplayName;

    private bool factoryDefaults;
    public bool FactoryDefaults
    {
        get
        {
            return factoryDefaults;
        }
        private set
        {
            if (value == factoryDefaults)
                return;

            factoryDefaults = value;
            OnPropertyChanged(nameof(FactoryDefaults));
        }
    }

    public FactoryDefaultsModule() : base(
        _moduleName,
        _moduleParameter)
    {
    }
    public FactoryDefaultsModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.FACTORY_DEFAULTS, false);
        this.FactoryDefaults = false;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (this.ParentGeneratedDevice is not null)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DMX_PERSONALITY:
                case ERDM_Parameter.DMX_START_ADDRESS:
                case ERDM_Parameter.DMX_STARTUP_MODE:
                case ERDM_Parameter.DISPLAY_INVERT:
                case ERDM_Parameter.PAN_INVERT:
                case ERDM_Parameter.TILT_INVERT:
                case ERDM_Parameter.DEVICE_UNIT_NUMBER:
                case ERDM_Parameter.DEVICE_LABEL:
                case ERDM_Parameter.CURVE:
                case ERDM_Parameter.MODULATION_FREQUENCY:
                case ERDM_Parameter.OUTPUT_RESPONSE_TIME:
                case ERDM_Parameter.DISPLAY_LEVEL:
                case ERDM_Parameter.IDENTIFY_TIMEOUT:
                case ERDM_Parameter.MAXIMUM_LEVEL:
                case ERDM_Parameter.MINIMUM_LEVEL:
                case ERDM_Parameter.PAN_TILT_SWAP:
                    ParentGeneratedDevice.setParameterValue(ERDM_Parameter.FACTORY_DEFAULTS, false);
                    this.FactoryDefaults = false;
                    break;
            }
        }
        switch (parameter)
        {
            case ERDM_Parameter.FACTORY_DEFAULTS when newValue is bool _factoryDefaults:
                this.FactoryDefaults = _factoryDefaults;
                break;
        }
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.FACTORY_DEFAULTS && command == ERDM_Command.SET_COMMAND)
            return true;

        return base.IsHandlingParameter(parameter, command);
    }

    protected override RDMMessage handleRequest(RDMMessage message)
    {
        switch (message.Parameter)
        {
            case ERDM_Parameter.FACTORY_DEFAULTS when message.Command is ERDM_Command.SET_COMMAND:
                if (message.Value is not null)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                try
                {
#if DEBUG
                    if (message.SourceUID.Equals(new UID(0xeeee, 0xf0f0f0f0)))
                        throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                    _ = this.SetFactoryDefaults();
                    return new RDMMessage()
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex);
                }
                break;
            default:
                break;
        }
        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
        {
            SourceUID = message.DestUID,
            DestUID = message.SourceUID,
            Command = message.Command | ERDM_Command.RESPONSE,
            Parameter = message.Parameter
        };
    }

    public async Task<bool> SetFactoryDefaults()
    {
        if (ParentRemoteDevice is not null)
        {
            ParameterBag parameterBag = new ParameterBag(ERDM_Parameter.FACTORY_DEFAULTS);
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.SET_COMMAND, this.ParentDevice.UID, this.ParentDevice.Subdevice, parameterBag);
            await ParentRemoteDevice.runPeerToPeerProcess(ptpProcess);
            if (ptpProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Finished)
            {
                this.FactoryDefaults = true;
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    await ParentRemoteDevice.RequestParameter(ERDM_Command.GET_COMMAND, ERDM_Parameter.FACTORY_DEFAULTS);
                });
                return true;
            }
        }
        else if (ParentGeneratedDevice is not null)
        {
            ParentGeneratedDevice.setParameterValue(ERDM_Parameter.FACTORY_DEFAULTS, true);
            return true;
        }
        return false;
    }
}