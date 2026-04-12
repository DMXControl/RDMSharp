using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class OutputResponseTimeModule : AbstractModule
{
    private const string _moduleName = "OutputResponseTime";
    private const string _moduleDisplayName = "Output Response Time";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.OUTPUT_RESPONSE_TIME,
        ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION
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
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.OUTPUT_RESPONSE_TIME, new RDMOutputResponseTime(currentId.Value, this.count.Value));

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                Task.Run(async () =>
                {
                    if (await SetOutputResponseTime(value.Value))
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

    public readonly IReadOnlyCollection<RDMOutputResponseTimeDescription> _generatedOutputResponseTimes = null;

    private byte _initialOutputResponseTimeId;

    public OutputResponseTimeModule(byte initialOutputResponseTimeId, params RDMOutputResponseTimeDescription[] outputResponseTimes) : base(
        _moduleName,
        _moduleParameters)
    {
        if (!outputResponseTimes.Any(c => c.OutputResponseTimeId == initialOutputResponseTimeId))
            throw new ArgumentOutOfRangeException($"No OutputResponseTime found with ID: {initialOutputResponseTimeId}");

        _initialOutputResponseTimeId = initialOutputResponseTimeId;
        _generatedOutputResponseTimes = (outputResponseTimes ?? Array.Empty<RDMOutputResponseTimeDescription>()).ToList().AsReadOnly();
        Count = (byte)_generatedOutputResponseTimes.Count;
    }
    public OutputResponseTimeModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.OUTPUT_RESPONSE_TIME, out object val) && val is RDMOutputResponseTime outputResponseTime)
            fillFromRemote(outputResponseTime);
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedOutputResponseTimes is not null)
        {
            if (_generatedOutputResponseTimes.Count >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {_generatedOutputResponseTimes}! Maximum is {byte.MaxValue - 1}");

            if (_generatedOutputResponseTimes.Count != 0)
            {
                var outputResponseTimeDesc = new ConcurrentDictionary<object, object>();
                foreach (var gOutputResponseTime in _generatedOutputResponseTimes)
                    if (!outputResponseTimeDesc.TryAdd(gOutputResponseTime.OutputResponseTimeId, gOutputResponseTime))
                        throw new Exception($"{gOutputResponseTime.OutputResponseTimeId} already used as {nameof(gOutputResponseTime.OutputResponseTimeId)}");

                device.setParameterValue(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION, outputResponseTimeDesc);
            }
        }
        await SetOutputResponseTime(_initialOutputResponseTimeId);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (ParentRemoteDevice is null)
            return;

        if (!_moduleParameters.Contains(parameter))
            return;

        switch (parameter)
        {
            case ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION:
                break;
            case ERDM_Parameter.OUTPUT_RESPONSE_TIME when newValue is RDMOutputResponseTime outputResponseTime:
                fillFromRemote(outputResponseTime);
                break;
        }
    }

    private void fillFromRemote(RDMOutputResponseTime outputResponseTime)
    {
        Count = outputResponseTime.ResponseTimes;
        if (idDescriptionPair.Count == 0 && ParentRemoteDevice is not null)
            if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION, out object val) && val is ConcurrentDictionary<object, object> dict)
            {
                foreach (var pair in dict)
                    if (pair.Value is RDMOutputResponseTimeDescription outputResponseTimeDescription)
                        idDescriptionPair.Add((byte)outputResponseTimeDescription.Index, outputResponseTimeDescription.Description);
                OnPropertyChanged(nameof(IdDescriptionPair));
            }
            else
            {
                for (byte i = 1; i <= Count; i++)
                    idDescriptionPair.Add(i, $"OutputResponseTime {i}");
                OnPropertyChanged(nameof(IdDescriptionPair));
            }

        CurrentId = outputResponseTime.CurrentResponseTimeId;
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.OUTPUT_RESPONSE_TIME)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == ERDM_Parameter.OUTPUT_RESPONSE_TIME)
            if (message.Command == ERDM_Command.SET_COMMAND)
            {
                if (message.Value is byte b)
                {
                    try
                    {
                        if (this._generatedOutputResponseTimes.FirstOrDefault(c => c.OutputResponseTimeId == b) is RDMOutputResponseTimeDescription outputResponseTime)
                            CurrentId = outputResponseTime.OutputResponseTimeId;
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

    public async Task<bool> SetOutputResponseTime(byte outputResponseTimeId)
    {
        if (ParentGeneratedDevice is not null)
        {
            if (this._generatedOutputResponseTimes.FirstOrDefault(p => p.OutputResponseTimeId == outputResponseTimeId) is not RDMOutputResponseTimeDescription outputResponseTime)
                throw new ArgumentOutOfRangeException($"No OutputResponseTime found with ID: {outputResponseTimeId}");
            CurrentId = outputResponseTime.OutputResponseTimeId;
        }
        if (ParentRemoteDevice is not null)
        {
            if (!this.IdDescriptionPair.Any(p => p.Key == outputResponseTimeId))
                throw new ArgumentOutOfRangeException($"No OutputResponseTime found with ID: {outputResponseTimeId}");
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.OUTPUT_RESPONSE_TIME, outputResponseTimeId);
        }
        return true;
    }
}