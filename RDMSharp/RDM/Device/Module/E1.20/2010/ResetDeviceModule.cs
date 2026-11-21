using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class ResetDeviceModule : AbstractModule
{
    private const string _moduleName = "ResetDevice";
    private const string _moduleDisplayName = "Reset Device";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.RESET_DEVICE;
    public event EventHandler<ERDM_ResetType> ResetRequested;

    public readonly IReadOnlyCollection<ERDM_ResetType> SupportedResetTypes;

    public override string DisplayName => _moduleDisplayName;

    public ResetDeviceModule(in ERDM_ResetType[] supportedResetTypes = null) : base(
        _moduleName,
        _moduleParameter)
    {
        if (supportedResetTypes is null)
            SupportedResetTypes = new List<ERDM_ResetType>()
            {
                ERDM_ResetType.Warm,
                ERDM_ResetType.Cold
            }.AsReadOnly();
        else
            SupportedResetTypes = supportedResetTypes.ToList().AsReadOnly();
    }
    public ResetDeviceModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.RESET_DEVICE && command == ERDM_Command.SET_COMMAND)
            return true;

        return base.IsHandlingParameter(parameter, command);
    }

    protected override RDMMessage handleRequest(RDMMessage message)
    {
        switch (message.Parameter)
        {
            case ERDM_Parameter.RESET_DEVICE when message.Command is ERDM_Command.SET_COMMAND:
                if (message.PDL is not 1)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                ERDM_ResetType resetType = (ERDM_ResetType)message.Value;

                if (!SupportedResetTypes.Contains(resetType))
                    return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
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
                    _ = this.SetReset(resetType);
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

    public async Task<bool> SetReset(ERDM_ResetType value)
    {
        if (ParentRemoteDevice is not null)
        {
            bool result = await ParentRemoteDevice.SetParameter(ERDM_Parameter.RESET_DEVICE, value);
            if (result)
                ResetRequested?.InvokeFailSafe(this, (ERDM_ResetType)value);
            return result;
        }
        if (ParentGeneratedDevice is not null)
        {
            ResetRequested?.InvokeFailSafe(this, (ERDM_ResetType)value);
            return true;
        }
        return false;
    }
}