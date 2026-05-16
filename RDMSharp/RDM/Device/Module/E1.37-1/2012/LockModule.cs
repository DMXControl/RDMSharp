using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class LockModule : AbstractModule
{
    SemaphoreSlim semaphoreSlimLockState = new SemaphoreSlim(1);
    private const string _moduleName = "Lock";
    private const string _moduleDisplayName = "Lock";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.LOCK_PIN,
        ERDM_Parameter.LOCK_STATE,
        ERDM_Parameter.LOCK_STATE_DESCRIPTION
    };

    public override string DisplayName => _moduleDisplayName;

    private ushort? lockPin;
    public ushort? LockPin
    {
        get
        {
            if (ParentDevice is null)
                return lockPin;

            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.LOCK_PIN, out object res) && res is ushort pin)
                return lockPin = pin;
            return lockPin;
        }
        set
        {
            if (value is null)
                return;

            if (lockPin == value)
                return;

            bool initial = lockPin is null;
            var backup = lockPin;
            lockPin = value;

            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.LOCK_PIN, lockPin.Value);

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                if (semaphoreSlimLockState.CurrentCount == 0)
                    return;
                Task.Run(async () =>
                {
                    await semaphoreSlimLockState.WaitAsync(3000);
                    try
                    {
                        if (await SetLockPin(value.Value))
                            OnPropertyChanged(nameof(LockPin));
                        else
                        {
                            lockPin = backup;
                            OnPropertyChanged(nameof(LockPin));
                        }
                    }
                    finally
                    {
                        semaphoreSlimLockState.Release();
                    }
                });
            }
        }
    }

    private byte? _lockState;
    public byte? LockState
    {
        get
        {
            return _lockState;
        }
        set
        {
            if (value is null)
                return;

            if (_lockState == value)
                return;

            bool initial = _lockState is null;
            var backup = _lockState;
            _lockState = value;

            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.LOCK_STATE, new GetLockStateResponse(_lockState.Value, count.Value));

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                if (semaphoreSlimLockState.CurrentCount == 0)
                    return;
                Task.Run(async () =>
                {
                    await semaphoreSlimLockState.WaitAsync(3000);
                    try
                    {
                        if (await SetLockPin(value.Value))
                            OnPropertyChanged(nameof(LockState));
                        else
                        {
                            _lockState = backup;
                            OnPropertyChanged(nameof(LockState));
                        }
                    }
                    finally
                    {
                        await Task.Delay(200);
                        semaphoreSlimLockState.Release();
                    }
                });
            }
        }
    }
    private byte? count;
    public byte? Count
    {
        get
        {
            return count;
        }
        private set
        {
            if (value == count)
                return;
            count = value;
            OnPropertyChanged();
        }
    }

    private Dictionary<byte, string> idDescriptionPair = new Dictionary<byte, string>();
    public IReadOnlyDictionary<byte, string> IdDescriptionPair => idDescriptionPair;

    public readonly IReadOnlyCollection<RDMLockStateDescription> _generatedLockStates = null;

    private byte _initialLockStateId;
    private ushort? _initialLockPin;

    public LockModule(byte initialLockStateId, ushort? initialLockPin = null, params RDMLockStateDescription[] lockStates) : base(
        _moduleName,
        _moduleParameters)
    {
        if (!(lockStates.Any(ls => ls.LockStateId == initialLockStateId) || initialLockStateId == 0))
            throw new ArgumentOutOfRangeException($"No LockState found with ID: {initialLockStateId}");

        _initialLockStateId = initialLockStateId;
        _initialLockPin = initialLockPin;
        _generatedLockStates = (lockStates ?? Array.Empty<RDMLockStateDescription>()).ToList().AsReadOnly();
        Count = (byte)_generatedLockStates.Count;
    }
    public LockModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.LOCK_STATE, out object val) && val is GetLockStateResponse lockState)
            fillFromRemote(lockState);
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedLockStates is not null)
        {
            if (_generatedLockStates.Count >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {_generatedLockStates}! Maximum is {byte.MaxValue - 1}");

            if (_generatedLockStates.Count != 0)
            {
                var lockStateDesc = new ConcurrentDictionary<object, object>();
                foreach (var gLockState in _generatedLockStates)
                    if (!lockStateDesc.TryAdd(gLockState.LockStateId, gLockState))
                        throw new Exception($"{gLockState.LockStateId} already used as {nameof(gLockState.LockStateId)}");

                device.setParameterValue(ERDM_Parameter.LOCK_STATE_DESCRIPTION, lockStateDesc);
            }
        }
        if (_initialLockPin.HasValue)
            await SetLockPin(_initialLockPin.Value);

        await SetLockState(_initialLockStateId);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (ParentRemoteDevice is null)
            return;

        if (!_moduleParameters.Contains(parameter))
            return;

        switch (parameter)
        {
            case ERDM_Parameter.LOCK_PIN when newValue is ushort _lockPin:
                this.LockPin = _lockPin;
                break;
            case ERDM_Parameter.LOCK_STATE_DESCRIPTION:
                break;
            case ERDM_Parameter.LOCK_STATE:
                if (newValue is GetLockStateResponse lockStateResponse)
                {
                    fillFromRemote(lockStateResponse);
                    if (_lockState != lockStateResponse.CurrentLockStateId)
                    {
                        if (semaphoreSlimLockState.CurrentCount == 0)
                            return;
                        Task.Run(async () =>
                        {
                            await semaphoreSlimLockState.WaitAsync(3000);
                            try
                            {
                                _lockState = lockStateResponse.CurrentLockStateId;
                                OnPropertyChanged(nameof(LockState));
                            }
                            finally
                            {
                                await Task.Delay(2000);
                                semaphoreSlimLockState.Release();
                            }
                        });
                    }
                }
                break;
        }
    }

    private void fillFromRemote(GetLockStateResponse lockStateResponse)
    {
        Count = lockStateResponse.LockStates;
        if (idDescriptionPair.Count == 0 && ParentRemoteDevice is not null)
            if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.LOCK_STATE_DESCRIPTION, out object val) && val is ConcurrentDictionary<object, object> dict)
            {
                foreach (var pair in dict)
                    if (pair.Value is RDMLockStateDescription lockStateDescription)
                        idDescriptionPair.Add((byte)lockStateDescription.LockStateId, lockStateDescription.Description);
                OnPropertyChanged(nameof(IdDescriptionPair));
            }
            else
            {
                for (byte i = 1; i <= Count; i++)
                    idDescriptionPair.Add(i, $"Lock State {i}");
                OnPropertyChanged(nameof(IdDescriptionPair));
            }
        //if (!LockState.HasValue)
        //    LockState = lockStateResponse.CurrentLockStateId;
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.LOCK_STATE || parameter == ERDM_Parameter.LOCK_PIN)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == ERDM_Parameter.LOCK_STATE || message.Parameter == ERDM_Parameter.LOCK_PIN)
            if (message.Command == ERDM_Command.SET_COMMAND)
            {
                if (message.Value is SetLockStateRequest setLockStateRequest)
                {

                    if (this.lockPin.HasValue && setLockStateRequest.PinCode != this.lockPin.Value)
                    {
                        return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                        {
                            DestUID = message.SourceUID,
                            SourceUID = message.DestUID,
                            Parameter = message.Parameter,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE
                        };
                    }
                    try
                    {
                        RDMLockStateDescription lockStateDescription = this._generatedLockStates.FirstOrDefault(c => c.LockStateId == setLockStateRequest.LockStateId);
                        if (setLockStateRequest.LockStateId == 0 || lockStateDescription != null)
                            LockState = lockStateDescription?.LockStateId ?? 0;
                        else
                        {
                            return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                            {
                                DestUID = message.SourceUID,
                                SourceUID = message.DestUID,
                                Parameter = message.Parameter,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex);
                        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
                        {
                            DestUID = message.SourceUID,
                            SourceUID = message.DestUID,
                            Parameter = message.Parameter,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE
                        };
                    }
                    return new RDMMessage()
                    {
                        DestUID = message.SourceUID,
                        SourceUID = message.DestUID,
                        Parameter = message.Parameter,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                    };
                }
                else if (message.Value is SetLockPinRequest setLockPinRequest)
                {
                    try
                    {
                        if ((setLockPinRequest.CurrentPinCode <= 9999 && setLockPinRequest.NewPinCode <= 9999) && LockPin == setLockPinRequest.CurrentPinCode)
                            LockPin = setLockPinRequest.NewPinCode;
                        else
                        {
                            return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                            {
                                DestUID = message.SourceUID,
                                SourceUID = message.DestUID,
                                Parameter = message.Parameter,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex);
                        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
                        {
                            DestUID = message.SourceUID,
                            SourceUID = message.DestUID,
                            Parameter = message.Parameter,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE
                        };
                    }
                    return new RDMMessage()
                    {
                        DestUID = message.SourceUID,
                        SourceUID = message.DestUID,
                        Parameter = message.Parameter,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                    };

                }
                return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                {
                    DestUID = message.SourceUID,
                    SourceUID = message.DestUID,
                    Parameter = message.Parameter,
                    Command = ERDM_Command.SET_COMMAND_RESPONSE
                };
            }
        return base.handleRequest(message);
    }

    public async Task<bool> SetLockState(byte lockState)
    {
        if (ParentGeneratedDevice is not null)
        {
            if (lockState != 0)
                if (this._generatedLockStates.FirstOrDefault(p => p.LockStateId == lockState) is not RDMLockStateDescription lockStateDiscription)
                    throw new ArgumentOutOfRangeException($"No Lock State found with ID: {lockState}");

            LockState = lockState;
        }
        if (ParentRemoteDevice is not null)
        {
            if (lockState != 0)
                if (!this.IdDescriptionPair.Any(p => p.Key == lockState))
                    throw new ArgumentOutOfRangeException($"No Lock State found with ID: {lockState}");

            if (!await ParentRemoteDevice.SetParameter(ERDM_Parameter.LOCK_STATE, new SetLockStateRequest(lockPin ?? 0, lockState)))
            {
                throw new Exception($"Failed to set Lock State to {lockState}!, Wrong Pin ({lockPin})?");
            }
            if (_lockState != lockState)
            {
                await semaphoreSlimLockState.WaitAsync(3000);
                try
                {
                    _lockState = lockState;
                    OnPropertyChanged(nameof(LockState));
                }
                finally
                {
                    await Task.Delay(2000);
                    semaphoreSlimLockState.Release();
                }
            }
        }
        return true;
    }
    public async Task<bool> SetLockPin(ushort lockPin)
    {
        if (lockPin > 9999)
            throw new ArgumentOutOfRangeException($"Lock Pin must be between 0 and 9999! Given: {lockPin}");

        if (ParentGeneratedDevice is not null)
            LockPin = lockPin;

        if (ParentRemoteDevice is not null)
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.LOCK_PIN, new SetLockPinRequest(lockPin, LockPin ?? 0));

        return true;
    }
}