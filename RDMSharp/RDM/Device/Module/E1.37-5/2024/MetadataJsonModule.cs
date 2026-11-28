using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using RDMSharp.PayloadObject;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// METADATA_JSON (E1.37-5): a device describes its manufacturer specific parameters as JSON defines.
/// <list type="bullet">
/// <item>Generated device: delivers the given JSON defines (GET METADATA_JSON with the PID of the parameter).</item>
/// <item>Remote device: registers every define the device delivers in the <see cref="MetadataFactory"/>
/// (<see cref="MetadataFactory.TryAddDefineFromJson"/>), so the parameter is parsed with it from then on.</item>
/// </list>
/// </summary>
/// <remarks>
/// A JSON define that is not accepted (invalid, not for this device, not a manufacturer specific parameter) is not registered,
/// the reason is kept in <see cref="RejectedParameters"/> and logged.
/// </remarks>
public sealed class MetadataJsonModule : AbstractParameterIDModule
{
    private const string _moduleName = "MetadataJson";
    private const string _moduleDisplayName = "Metadata JSON";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.METADATA_JSON;
    private const ushort MANUFACTURER_SPECIFIC_PID_MIN = 0x8000;
    private const ushort MANUFACTURER_SPECIFIC_PID_MAX = 0xFFDF;

    public override string DisplayName => _moduleDisplayName;

    private readonly RDMMetadataJson[] _generatedMetadataJsons = Array.Empty<RDMMetadataJson>();

    private readonly ConcurrentDictionary<ERDM_Parameter, bool> _registeredParameters = new ConcurrentDictionary<ERDM_Parameter, bool>();
    private readonly ConcurrentDictionary<ERDM_Parameter, string> _rejectedParameters = new ConcurrentDictionary<ERDM_Parameter, string>();

    /// <summary>The JSON defines a generated device delivers.</summary>
    public IReadOnlyCollection<RDMMetadataJson> MetadataJsons => _generatedMetadataJsons;

    /// <summary>The parameters whose JSON define was registered in the <see cref="MetadataFactory"/> (remote).</summary>
    public IReadOnlyCollection<ERDM_Parameter> RegisteredParameters => _registeredParameters.Keys.OrderBy(p => (ushort)p).ToList().AsReadOnly();

    /// <summary>The parameters whose JSON define was not accepted, with the reason (remote).</summary>
    public IReadOnlyDictionary<ERDM_Parameter, string> RejectedParameters => new Dictionary<ERDM_Parameter, string>(_rejectedParameters);

    /// <summary>
    /// Generated device.
    /// </summary>
    /// <param name="metadataJsons">The JSON defines of the manufacturer specific parameters of the device, one per parameter.</param>
    public MetadataJsonModule(params RDMMetadataJson[] metadataJsons) : base(
        _moduleName,
        _moduleParameter)
    {
        metadataJsons ??= Array.Empty<RDMMetadataJson>();
        var used = new HashSet<ERDM_Parameter>();
        foreach (var metadataJson in metadataJsons)
        {
            ushort pid = (ushort)metadataJson.ParameterId;
            if (pid < MANUFACTURER_SPECIFIC_PID_MIN || pid > MANUFACTURER_SPECIFIC_PID_MAX)
                throw new ArgumentOutOfRangeException(nameof(metadataJsons), $"0x{pid:X4} is not a manufacturer specific parameter");
            if (!used.Add(metadataJson.ParameterId))
                throw new ArgumentException($"{metadataJson.ParameterId} is described more than once", nameof(metadataJsons));
        }
        _generatedMetadataJsons = metadataJsons.ToArray();
    }

    /// <summary>
    /// Remote device.
    /// </summary>
    public MetadataJsonModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
        // the model of the device is requested before the modules exist, so its JSON defines are already there
        if (ParentRemoteDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.METADATA_JSON, out object values))
            registerAll(values);
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedMetadataJsons.Length == 0)
            return;

        // the index of a GET is the parameter the request is about (the PID of the request is an ERDM_Parameter)
        var metadataJsons = new ConcurrentDictionary<object, object>();
        foreach (var metadataJson in _generatedMetadataJsons)
            metadataJsons.TryAdd(metadataJson.ParameterId, metadataJson);

        device.setParameterValue(ERDM_Parameter.METADATA_JSON, metadataJsons);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        // a generated device delivers its defines, it must not register them as if another device had sent them
        if (ParentRemoteDevice is null)
            return;

        if (parameter == ERDM_Parameter.METADATA_JSON)
            registerAll(newValue);
    }

    /// <summary>
    /// A single response, or all of them (a device of an already known model gets the values of the model at once).
    /// </summary>
    private void registerAll(object value)
    {
        switch (value)
        {
            case RDMMetadataJson metadataJson:
                register(metadataJson.ParameterId, metadataJson.JSON);
                break;

            case IDictionary dictionary:
                foreach (var item in dictionary.Values)
                    if (item is RDMMetadataJson itemJson)
                        register(itemJson.ParameterId, itemJson.JSON);
                break;
        }
    }

    private void register(ERDM_Parameter parameter, string json)
    {
        // the module of a remote device exists after the DEVICE_INFO of the device was requested
        var device = ParentRemoteDevice;
        if (MetadataFactory.TryAddDefineFromJson(device.UID, device.DeviceInfo, parameter, json, out var define, out string reason))
        {
            _rejectedParameters.TryRemove(parameter, out _);
            _registeredParameters[parameter] = true;
            requestValue(parameter, define);
        }
        else
        {
            _registeredParameters.TryRemove(parameter, out _);
            _rejectedParameters[parameter] = reason;
            Logger?.LogWarning($"JSON define of {device.UID} for {parameter} (0x{(ushort)parameter:X4}) is not registered: {reason}");
        }
        OnPropertyChanged(nameof(RegisteredParameters));
        OnPropertyChanged(nameof(RejectedParameters));
    }

    /// <summary>
    /// The module exists after the first request of all parameters, which skipped the parameter because it had no define.
    /// Nothing requests it again (the updates only refresh parameters that already have a value), so it is requested now.
    /// </summary>
    private void requestValue(ERDM_Parameter parameter, MetadataJSONObjectDefine define)
    {
        var device = ParentRemoteDevice;
        // only a GET without a request payload can be requested like this
        if (!(define.GetRequest?.GetIsEmpty() ?? false))
            return;
        if (device.GetAllParameterValues().ContainsKey(parameter))
            return;

        Task.Run(async () =>
        {
            try
            {
                await device.RequestParameter(ERDM_Command.GET_COMMAND, parameter);
            }
            catch (Exception ex)
            {
                Logger?.LogWarning(ex, $"Can't request {parameter} (0x{(ushort)parameter:X4}) of {device.UID} after its JSON define was registered");
            }
        });
    }
}
