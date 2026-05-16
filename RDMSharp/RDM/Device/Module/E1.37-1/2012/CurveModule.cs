using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class CurveModule : AbstractModule
{
    private const string _moduleName = "Curve";
    private const string _moduleDisplayName = "Curve";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.CURVE,
        ERDM_Parameter.CURVE_DESCRIPTION
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
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.CURVE, new RDMCurve(currentId.Value, this.count.Value));

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                Task.Run(async () =>
                {
                    if (await SetCurve(value.Value))
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

    public readonly IReadOnlyCollection<RDMCurveDescription> _generatedCurves = null;

    private byte _initialCurveId;

    public CurveModule(byte initialCurveId, params RDMCurveDescription[] curves) : base(
        _moduleName,
        _moduleParameters)
    {
        if (!curves.Any(c => c.CurveId == initialCurveId))
            throw new ArgumentOutOfRangeException($"No Curve found with ID: {initialCurveId}");

        _initialCurveId = initialCurveId;
        _generatedCurves = (curves ?? Array.Empty<RDMCurveDescription>()).ToList().AsReadOnly();
        Count = (byte)_generatedCurves.Count;
    }
    public CurveModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.CURVE, out object val) && val is RDMCurve curve)
            fillFromRemote(curve);
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedCurves is not null)
        {
            if (_generatedCurves.Count >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {_generatedCurves}! Maximum is {byte.MaxValue - 1}");

            if (_generatedCurves.Count != 0)
            {
                var curveDesc = new ConcurrentDictionary<object, object>();
                foreach (var gCurve in _generatedCurves)
                    if (!curveDesc.TryAdd(gCurve.CurveId, gCurve))
                        throw new Exception($"{gCurve.CurveId} already used as {nameof(gCurve.CurveId)}");

                device.setParameterValue(ERDM_Parameter.CURVE_DESCRIPTION, curveDesc);
            }
        }
        await SetCurve(_initialCurveId);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (ParentRemoteDevice is null)
            return;

        if (!_moduleParameters.Contains(parameter))
            return;

        switch (parameter)
        {
            case ERDM_Parameter.CURVE_DESCRIPTION:
                break;
            case ERDM_Parameter.CURVE when newValue is RDMCurve curve:
                fillFromRemote(curve);
                break;
        }
    }

    private void fillFromRemote(RDMCurve curve)
    {
        Count = curve.Curves;
        if (idDescriptionPair.Count == 0 && ParentRemoteDevice is not null)
            if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.CURVE_DESCRIPTION, out object val) && val is ConcurrentDictionary<object, object> dict)
            {
                foreach (var pair in dict)
                    if (pair.Value is RDMCurveDescription curveDescription)
                        idDescriptionPair.Add((byte)curveDescription.Index, curveDescription.Description);
                OnPropertyChanged(nameof(IdDescriptionPair));
            }
            else
            {
                for (byte i = 1; i <= Count; i++)
                    idDescriptionPair.Add(i, $"Curve {i}");
                OnPropertyChanged(nameof(IdDescriptionPair));
            }

        CurrentId = curve.CurrentCurveId;
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (parameter == ERDM_Parameter.CURVE)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == ERDM_Parameter.CURVE)
            if (message.Command == ERDM_Command.SET_COMMAND)
            {
                if (message.Value is byte b)
                {
                    try
                    {
                        if (this._generatedCurves.FirstOrDefault(c => c.CurveId == b) is RDMCurveDescription curve)
                            CurrentId = curve.CurveId;
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

    public async Task<bool> SetCurve(byte curveId)
    {
        if (ParentGeneratedDevice is not null)
        {
            if (this._generatedCurves.FirstOrDefault(p => p.CurveId == curveId) is not RDMCurveDescription curve)
                throw new ArgumentOutOfRangeException($"No Curve found with ID: {curveId}");
            CurrentId = curve.CurveId;
        }
        if (ParentRemoteDevice is not null)
        {
            if (!this.IdDescriptionPair.Any(p => p.Key == curveId))
                throw new ArgumentOutOfRangeException($"No Curve found with ID: {curveId}");
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.CURVE, curveId);
        }
        return true;
    }
}