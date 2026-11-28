using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// SHIPPING_LOCK (E1.37-5 4.5): state of the physical transport lock, e.g. of a moving light.
/// GET returns UNLOCKED, LOCKED or PARTIALLY_LOCKED (Table A-2).
/// A device with a remotely operable (electronic) lock also supports SET with Unlock (0x00) / Lock (0x01),
/// a device with only a sensor supports GET only.
/// </summary>
public sealed class ShippingLockModule : AbstractSimpleEnumParameterIDModule<ERDM_ShippingLockState>
{
    private const string _moduleName = "ShippingLock";
    private const string _moduleDisplayName = "Shipping Lock";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.SHIPPING_LOCK;

    public sealed override string DisplayName => _moduleDisplayName;

    /// <summary>
    /// <c>true</c> if the lock can be operated by SHIPPING_LOCK SET, <c>false</c> if the device only reports the state of a sensor.
    /// </summary>
    public bool RemotelyOperable { get; }

    public ERDM_ShippingLockState? ShippingLockState
    {
        get
        {
            return base.Value;
        }
        set
        {
            base.Value = value;
        }
    }

    public ShippingLockModule(bool remotelyOperable, ERDM_ShippingLockState initialState = ERDM_ShippingLockState.UNLOCKED) : base(
        initialState,
        _moduleName,
        _moduleParameter,
        remotelyOperable ? ERDM_CommandClass.GET | ERDM_CommandClass.SET : ERDM_CommandClass.GET)
    {
        RemotelyOperable = remotelyOperable;
    }
    public ShippingLockModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
        RemotelyOperable = true;
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(ShippingLockState));
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (ParentGeneratedDevice is not null && RemotelyOperable && parameter == ModuleParameter && command == ERDM_Command.SET_COMMAND)
            return true;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (ParentGeneratedDevice is not null && RemotelyOperable && message.Parameter == ModuleParameter && message.Command == ERDM_Command.SET_COMMAND)
        {
            if (message.PDL != 1 || message.Value is not ERDM_ShippingLockState state)
                return createNack(message, ERDM_NackReason.FORMAT_ERROR);
            // SET only knows Unlock (0x00) / Lock (0x01), PARTIALLY_LOCKED can only be reported
            if (state != ERDM_ShippingLockState.UNLOCKED && state != ERDM_ShippingLockState.LOCKED)
                return createNack(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
            return null; // valid, the generic handling stores the new state
        }
        return base.handleRequest(message);
    }
    private static RDMMessage createNack(RDMMessage request, ERDM_NackReason nackReason)
    {
        return new RDMMessage(nackReason)
        {
            SourceUID = request.DestUID,
            DestUID = request.SourceUID,
            SubDevice = request.SubDevice,
            Command = request.Command | ERDM_Command.RESPONSE,
            Parameter = request.Parameter
        };
    }

    /// <summary>
    /// Locks (<c>true</c>) or unlocks (<c>false</c>) the shipping lock of the remote device.
    /// </summary>
    public async Task<bool> SetShippingLock(bool locked)
    {
        return await base.SetValue(locked ? ERDM_ShippingLockState.LOCKED : ERDM_ShippingLockState.UNLOCKED);
    }
}
