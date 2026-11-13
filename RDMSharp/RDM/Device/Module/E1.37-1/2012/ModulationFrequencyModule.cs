using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class ModulationFrequencyModule : AbstractModule
{
    private const string _moduleName = "ModulationFrequency";
    private const string _moduleDisplayName = "Modulation Frequency";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.MODULATION_FREQUENCY,
        ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION
    };

    public override string DisplayName => _moduleDisplayName;

    private byte? currentId;
    public byte? CurrentId
    {
        get
        {
            return currentId;
        }
        set
        {
            if (value is null)
                return;

            if (currentId == value)
                return;

            bool initial = currentId is null;
            var backup = currentId;
            currentId = value;

            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.MODULATION_FREQUENCY, new RDMModulationFrequency(currentId.Value, this.count.Value));

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                Task.Run(async () =>
                {
                    if (await SetModulationFrequency(value.Value))
                        OnPropertyChanged(nameof(CurrentId));
                    else
                    {
                        currentId = backup;
                        OnPropertyChanged(nameof(CurrentId));
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

    public readonly IReadOnlyCollection<RDMModulationFrequencyDescription> _generatedModulationFrequencys = null;

    private byte _initialModulationFrequencyId;

    public ModulationFrequencyModule(byte initialModulationFrequencyId, params RDMModulationFrequencyDescription[] ModulationFrequencys) : base(
        _moduleName,
        _moduleParameters)
    {
        if (!ModulationFrequencys.Any(c => c.ModulationFrequencyId == initialModulationFrequencyId))
            throw new ArgumentOutOfRangeException($"No ModulationFrequency found with ID: {initialModulationFrequencyId}");

        _initialModulationFrequencyId = initialModulationFrequencyId;
        _generatedModulationFrequencys = (ModulationFrequencys ?? Array.Empty<RDMModulationFrequencyDescription>()).ToList().AsReadOnly();
        Count = (byte)_generatedModulationFrequencys.Count;
    }
    public ModulationFrequencyModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.MODULATION_FREQUENCY, out object val) && val is RDMModulationFrequency ModulationFrequency)
            fillFromRemote(ModulationFrequency);
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedModulationFrequencys is not null)
        {
            if (_generatedModulationFrequencys.Count >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {_generatedModulationFrequencys}! Maximum is {byte.MaxValue - 1}");

            if (_generatedModulationFrequencys.Count != 0)
            {
                var ModulationFrequencyDesc = new ConcurrentDictionary<object, object>();
                foreach (var gModulationFrequency in _generatedModulationFrequencys)
                    if (!ModulationFrequencyDesc.TryAdd(gModulationFrequency.ModulationFrequencyId, gModulationFrequency))
                        throw new Exception($"{gModulationFrequency.ModulationFrequencyId} already used as {nameof(gModulationFrequency.ModulationFrequencyId)}");

                device.setParameterValue(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION, ModulationFrequencyDesc);
            }
        }
        await SetModulationFrequency(_initialModulationFrequencyId);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (ParentRemoteDevice is null)
            return;

        if (!_moduleParameters.Contains(parameter))
            return;

        switch (parameter)
        {
            case ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION:
                break;
            case ERDM_Parameter.MODULATION_FREQUENCY when newValue is RDMModulationFrequency ModulationFrequency:
                fillFromRemote(ModulationFrequency);
                break;
        }
    }

    private void fillFromRemote(RDMModulationFrequency modulationFrequency)
    {
        Count = modulationFrequency.ModulationFrequencys;
        if (idDescriptionPair.Count == 0 && ParentRemoteDevice is not null)
            if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION, out object val) && val is ConcurrentDictionary<object, object> dict)
            {
                foreach (var pair in dict)
                    if (pair.Value is RDMModulationFrequencyDescription modulationFrequencyDescription)
                        idDescriptionPair.Add((byte)modulationFrequencyDescription.Index, modulationFrequencyDescription.Description);
                OnPropertyChanged(nameof(IdDescriptionPair));
            }
            else
            {
                for (byte i = 1; i <= Count; i++)
                    idDescriptionPair.Add(i, $"ModulationFrequency {i}");
                OnPropertyChanged(nameof(IdDescriptionPair));
            }

        CurrentId = modulationFrequency.ModulationFrequencyId;
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.MODULATION_FREQUENCY)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == ERDM_Parameter.MODULATION_FREQUENCY)
            if (message.Command == ERDM_Command.SET_COMMAND)
            {
                if (message.Value is byte b)
                {
                    try
                    {
                        if (this._generatedModulationFrequencys.FirstOrDefault(c => c.ModulationFrequencyId == b) is RDMModulationFrequencyDescription modulationFrequency)
                            CurrentId = modulationFrequency.ModulationFrequencyId;
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

    public async Task<bool> SetModulationFrequency(byte modulationFrequencyId)
    {
        if (ParentGeneratedDevice is not null)
        {
            if (this._generatedModulationFrequencys.FirstOrDefault(p => p.ModulationFrequencyId == modulationFrequencyId) is not RDMModulationFrequencyDescription modulationFrequency)
                throw new ArgumentOutOfRangeException($"No ModulationFrequency found with ID: {modulationFrequencyId}");
            CurrentId = modulationFrequency.ModulationFrequencyId;
        }
        if (ParentRemoteDevice is not null)
        {
            if (!this.IdDescriptionPair.Any(p => p.Key == modulationFrequencyId))
                throw new ArgumentOutOfRangeException($"No ModulationFrequency found with ID: {modulationFrequencyId}");
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.MODULATION_FREQUENCY, modulationFrequencyId);
        }
        return true;
    }
}