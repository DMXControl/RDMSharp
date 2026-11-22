using Microsoft.Extensions.Logging;
using RDMSharp.Extensions;
using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp;

public sealed class RDMDeviceModel : AbstractRDMCache, IRDMDeviceModel
{
    private static ConcurrentDictionary<int, RDMDeviceModel> knownDeviceModels;
    public static IReadOnlyCollection<RDMDeviceModel> KnownDeviceModels => knownDeviceModels.Values.ToList();
    internal static RDMDeviceModel getDeviceModel(UID uid, SubDevice subDevice, RDMDeviceInfo deviceInfo, Func<RDMMessage, Task> sendRdmFunktion)
    {
        knownDeviceModels ??= new ConcurrentDictionary<int, RDMDeviceModel>();
        var kdm = knownDeviceModels.Values.FirstOrDefault(dm => dm.IsModelOf(uid, subDevice, deviceInfo));
        if (kdm == null)
        {
            kdm = new RDMDeviceModel(uid, subDevice, deviceInfo);
            knownDeviceModels.TryAdd(kdm.GetHashCode(), kdm);
        }

        return kdm;

    }

    private readonly ConcurrentDictionary<byte, RDMPersonalityModel> knownPersonalityModels = new ConcurrentDictionary<byte, RDMPersonalityModel>();
    public IReadOnlyCollection<RDMPersonalityModel> KnownPersonalityModels => knownPersonalityModels.Values.ToList();
    internal RDMPersonalityModel getPersonalityModel(IRDMRemoteDevice remoteRDMDevice, byte personalityId)
    {
        try
        {
            if (!DeviceInfo.Dmx512CurrentPersonality.HasValue)
                return null;
            var kpm = knownPersonalityModels.Values.FirstOrDefault(dm => dm.IsModelOf(
                remoteRDMDevice.UID,
                remoteRDMDevice.DeviceInfo.DeviceModelId,
                remoteRDMDevice.DeviceInfo.SoftwareVersionId,
                personalityId));
            if (kpm == null)
            {
                kpm = new RDMPersonalityModel(
                    remoteRDMDevice,
                    personalityId);
                knownPersonalityModels.TryAdd(kpm.PersonalityID, kpm);
            }
            return kpm;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex);
        }
        return null;
    }

    public new bool IsDisposing { get; private set; }
    public new bool IsDisposed { get; private set; }

    public bool IsInitialized { get; private set; } = false;
    public bool IsInitializing { get; private set; } = false;

    public event EventHandler Initialized;
    public event PropertyChangedEventHandler PropertyChanged;
    public new event EventHandler<ParameterValueAddedEventArgs> ParameterValueAdded
    {
        add
        {
            base.ParameterValueAdded += value;
        }
        remove
        {
            base.ParameterValueAdded -= value;
        }
    }

    public readonly ushort ManufacturerID;
    public readonly EManufacturer Manufacturer;

    private UID currentUsedUID;
    public UID CurrentUsedUID
    {
        get { return currentUsedUID; }
        private set
        {
            if (currentUsedUID == value)
                return;
            currentUsedUID = value;
            PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentUsedUID)));
        }
    }
    private SubDevice currentUsedSubDevice;
    public SubDevice CurrentUsedSubDevice
    {
        get
        {
            return currentUsedSubDevice;
        }
        private set
        {
            if (currentUsedSubDevice == value)
                return;
            currentUsedSubDevice = value;
            PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentUsedSubDevice)));
        }
    }


    public RDMDeviceInfo DeviceInfo
    {
        get { return parameterValues.TryGetValue(ERDM_Parameter.DEVICE_INFO, out object value) ? value as RDMDeviceInfo : null; }
        private set
        {
            if (this.DeviceInfo == value)
                return;

            var dataTreeBranch = DataTreeBranch.FromObject(value, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO);
            updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, dataTreeBranch);
            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), dataTreeBranch);
            PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DeviceInfo)));
        }
    }

    private ConcurrentDictionary<ERDM_Parameter, SupportedParameterMetadata> supportedParameters = new ConcurrentDictionary<ERDM_Parameter, SupportedParameterMetadata>();

    public IReadOnlyCollection<SupportedParameterMetadata> GetSupportedParameters()
    {
        return this.supportedParameters.Where(sp => sp.Value.IsSupported && !sp.Value.IsSilent && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }
    public IReadOnlyCollection<SupportedParameterMetadata> GetSupportedBlueprintModelParameters()
    {
        return this.supportedParameters.Where(sp => sp.Value.IsSupported && !sp.Value.IsSilent && sp.Value.IsBlueprintModel && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }
    public IReadOnlyCollection<SupportedParameterMetadata> GetSupportedBlueprintModelPersonalityParameters()
    {
        return this.supportedParameters.Where(sp => sp.Value.IsSupported && !sp.Value.IsSilent && sp.Value.IsBlueprintModelPersonality && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }
    public IReadOnlyCollection<SupportedParameterMetadata> GetSupportedNonBlueprintParameters()
    {
        return this.supportedParameters.Where(sp => sp.Value.IsSupported && !sp.Value.IsSilent && sp.Value.IsNonBlueprint && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }
    public IReadOnlyCollection<SupportedParameterMetadata> GetKnownNotSupportedParameters()
    {
        return this.supportedParameters.Where(sp => (!sp.Value.IsSupported || sp.Value.IsSilent) && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }

    /// <summary>
    /// The parameters the devices of the model do not answer while they answer everything else (see <see cref="SupportedParameterMetadata.IsSilent"/>).
    /// </summary>
    public IReadOnlyCollection<SupportedParameterMetadata> GetSilentParameters()
    {
        return this.supportedParameters.Where(sp => sp.Value.IsSupported && sp.Value.IsSilent && ((ushort)sp.Value.Parameter > 0x000F)).Select(sp => sp.Value).OrderBy(p => p.Parameter).ToArray().AsReadOnly();
    }
    internal void ReportSilent(ERDM_Parameter parameter, UID uid)
    {
        if (this.supportedParameters is not null && this.supportedParameters.TryGetValue(parameter, out SupportedParameterMetadata supportedParameterMetadata))
            supportedParameterMetadata.ReportSilent(uid);
    }
    internal void ReportAnswered(ERDM_Parameter parameter)
    {
        if (this.supportedParameters is not null && this.supportedParameters.TryGetValue(parameter, out SupportedParameterMetadata supportedParameterMetadata))
            supportedParameterMetadata.ReportAnswered();
    }

    internal RDMDeviceModel(UID uid, SubDevice subdevice, RDMDeviceInfo deviceInfo)
    {
        DeviceInfo = deviceInfo;
        CurrentUsedUID = uid;
        CurrentUsedSubDevice = subdevice;
        ManufacturerID = uid.ManufacturerID;
        Manufacturer = (EManufacturer)uid.ManufacturerID;
        this.ParameterValueAdded += RDMDeviceModel_ParameterValueAdded;
    }

    private readonly SemaphoreSlim initializeSemaphoreSlim = new SemaphoreSlim(1);
    // SUPPORTED_PARAMETERS is asked for this often if the device does not answer, then the device is asked if it answers at all
    private const int MAX_ATTEMPTS = 3;

    /// <summary>
    /// Initializes the model with the device that asks: if the device that was used before is not reachable, the next one can do it.
    /// </summary>
    internal async Task Initialize(UID uid, SubDevice subDevice)
    {
        if (IsInitialized || initializeSemaphoreSlim.CurrentCount == 0)
            return;
        CurrentUsedUID = uid;
        CurrentUsedSubDevice = subDevice;
        await Initialize();
    }
    internal async Task Initialize()
    {
        if (IsInitialized)
            return;
        if (initializeSemaphoreSlim.CurrentCount == 0)
            return;
        IsInitializing = true;

        await initializeSemaphoreSlim.WaitAsync();
        try
        {
            // The model is shared by all devices of the model: if it was initialized without the supported parameters because one request got no answer,
            // all of them would have to do without. A device that is not reachable (or too busy) does not initialize it, the next attempt asks again.
            // A device that answers everything else, but not this, is faulty: the model is initialized without it, as before.
            bool answered = false;
            for (int attempt = 0; attempt < MAX_ATTEMPTS && !answered; attempt++)
            {
                if (attempt > 0)
                    await Task.Delay(GlobalTimers.Instance.MissedRequestRetryInterval);
                answered = await requestSupportedParameters();
            }
            if (!answered && !await probeAlive(CurrentUsedUID, CurrentUsedSubDevice))
            {
                Logger?.LogInformation($"The model of {CurrentUsedUID} is not initialized, the device did not answer SUPPORTED_PARAMETERS and is not reachable");
                return;
            }
            await requestBlueprintParameters(this.GetSupportedBlueprintModelParameters());
            await requestSupportedParametersExtensions();
            //await requestPersonalityBlueprintParameters();

            IsInitialized = true;
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
        finally
        {
            initializeSemaphoreSlim.Release();
            IsInitializing = false;
        }
        Initialized?.Invoke(this, EventArgs.Empty);
    }



    #region Requests without an answer
    private sealed class MissedRequest
    {
        public DateTime LastTry = DateTime.UtcNow;
        public int Attempts;
    }
    private readonly ConcurrentDictionary<(ERDM_Parameter Parameter, object Index), MissedRequest> missedRequests = new();
    private readonly SemaphoreSlim retrySemaphoreSlim = new SemaphoreSlim(1);

    protected override Task OnProcessTimedOut(PeerToPeerProcess process)
    {
        // an entry that got no answer must not end a list or leave a hole in the model for good: it is asked for again
        if (process.Command == ERDM_Command.GET_COMMAND && process.ParameterBag.PID != ERDM_Parameter.SUPPORTED_PARAMETERS && process.ParameterBag.PID != ERDM_Parameter.DEVICE_INFO)
            missedRequests.TryAdd((process.ParameterBag.PID, process.RequestIndex), new MissedRequest());
        return Task.CompletedTask;
    }
    protected override void OnProcessAnswered(PeerToPeerProcess process)
    {
        missedRequests.TryRemove((process.ParameterBag.PID, process.RequestIndex), out _);
    }
    /// <summary>
    /// Asks again for what got no answer, with the device that asks (the one that was used before can be gone). Every entry a few times,
    /// then it is given up.
    /// </summary>
    internal async Task RetryMissed(UID uid, SubDevice subDevice)
    {
        if (!IsInitialized || missedRequests.IsEmpty || retrySemaphoreSlim.CurrentCount == 0)
            return;

        await retrySemaphoreSlim.WaitAsync();
        try
        {
            foreach (var pair in missedRequests.ToArray())
            {
                if (DateTime.UtcNow - pair.Value.LastTry < TimeSpan.FromMilliseconds(GlobalTimers.Instance.MissedRequestRetryInterval))
                    continue;
                if (pair.Value.Attempts >= MAX_ATTEMPTS)
                {
                    missedRequests.TryRemove(pair.Key, out _);
                    Logger?.LogInformation($"{pair.Key.Parameter} (index {pair.Key.Index ?? "none"}) of the model of {uid} got no answer {MAX_ATTEMPTS} times, it is given up");
                    continue;
                }
                pair.Value.LastTry = DateTime.UtcNow;
                pair.Value.Attempts++;

                var parameterBag = new ParameterBag(pair.Key.Parameter, ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define?.GetRequest.HasValue ?? false)
                {
                    if (define.GetRequest.Value.GetIsEmpty())
                        await requestGetParameterWithEmptyPayload(parameterBag, define, uid, subDevice);
                    else
                        await requestGetParameterWithPayload(parameterBag, define, uid, subDevice, pair.Key.Index);
                }
                await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
        finally
        {
            retrySemaphoreSlim.Release();
        }
    }
    #endregion

    #region Requests
    private async Task<bool> requestSupportedParameters()
    {
        ParameterBag parameterBag = new ParameterBag(ERDM_Parameter.SUPPORTED_PARAMETERS);
        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, CurrentUsedUID, CurrentUsedSubDevice, parameterBag);
        await runPeerToPeerProcess(ptpProcess);

        if (!ptpProcess.ResponsePayloadObject.IsUnset)
            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(parameterBag.PID), ptpProcess.ResponsePayloadObject);

        if (ptpProcess.ResponsePayloadObject.ParsedObject is ERDM_Parameter[] parameters)
        {
            foreach (var para in parameters)
            {
                if (!this.supportedParameters.TryGetValue(para, out _))
                    if (ExtensionsManager.Instance.TryGetSupportedParameterMetadata(this.Manufacturer, para, out SupportedParameterMetadata supportedParameterMetadata))
                        supportedParameters.TryAdd(para, supportedParameterMetadata);
            }
            //if (DeviceInfo.Dmx512StartAddress.HasValue && DeviceInfo.Dmx512StartAddress >= 1 && DeviceInfo.Dmx512StartAddress.Value <= 512) // Remote Device not send DMX_START_ADDRESS Parameter but uses it!
            //    supportedParameters.TryAdd(ERDM_Parameter.DMX_START_ADDRESS, true);

            //if (DeviceInfo.Dmx512CurrentPersonality.HasValue) // Remote Device not send DMX_PERSONALITY Parameter but uses it!
            //    supportedParameters.TryAdd(ERDM_Parameter.DMX_PERSONALITY, true);

            //if (!this.supportedParameters.ContainsKey(ERDM_Parameter.PARAMETER_DESCRIPTION) && this.supportedParameters.Any(p => ((ushort)p.Key) >= 0x8000 && ((ushort)p.Key) <= 0xFFDF)) // Remote Device not send PARAMETER_DESCRIPTION Parameter but has Manufacture speific Parameters it!
            //    this.supportedParameters.TryAdd(ERDM_Parameter.PARAMETER_DESCRIPTION, true);

            //if (!this.supportedParameters.ContainsKey(ERDM_Parameter.IDENTIFY_DEVICE)) //Test it if the device supports Identify Device Parameter, if not it will be labled as not supported later on
            //    this.supportedParameters.TryAdd(ERDM_Parameter.IDENTIFY_DEVICE, true);

            //if (!this.supportedParameters.ContainsKey(ERDM_Parameter.SOFTWARE_VERSION_LABEL))//Test it if the device supports Software Version Lable Device Parameter, if not it will be labled as not supported later on
            //    this.supportedParameters.TryAdd(ERDM_Parameter.SOFTWARE_VERSION_LABEL, true);

            //if (!this.supportedParameters.ContainsKey(ERDM_Parameter.FACTORY_DEFAULTS))//Test it if the device supports Factory Defaults Device Parameter, if not it will be labled as not supported later on
            //    this.supportedParameters.TryAdd(ERDM_Parameter.FACTORY_DEFAULTS, true);

            //if (this.supportedParameters.Any(p => ((ushort)p.Key) > 0x9000 && ((ushort)p.Key) <= 0x900D))
            //    this.supportedParameters.TryAdd(ERDM_Parameter.ENDPOINT_LIST, true);
        }
        await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
        return !ptpProcess.TimedOut;
    }
    private async Task requestSupportedParametersExtensions()
    {
        ExtensionsManager.Instance.TryGetSupportedParametersExtensions(this.Manufacturer, out IReadOnlyCollection<ISupportedParametersExtension> supportedParametersExtensionsResult);
        Func<ERDM_Parameter[], Task> getExtSupportedParameters = async (dm) =>
        {
            List<SupportedParameterMetadata> extSupportedParameters = new List<SupportedParameterMetadata>();
            foreach (var p in dm)
                if (ExtensionsManager.Instance.TryGetSupportedParameterMetadata(this.Manufacturer, p, out SupportedParameterMetadata supportedParameterMetadata))
                    if (supportedParameters.TryAdd(p, supportedParameterMetadata))
                        extSupportedParameters.Add(supportedParameterMetadata);

            List<SupportedParameterMetadata> supportedParameterMetadataToRequest = new List<SupportedParameterMetadata>();
            supportedParameterMetadataToRequest.AddRange(extSupportedParameters);
            if (extSupportedParameters.Where(spm => spm.IsManufacturerSpecific).Count() != 0)
                supportedParameterMetadataToRequest.InsertRange(0, this.supportedParameters.Where(p => p.Key == ERDM_Parameter.PARAMETER_DESCRIPTION).Select(p => p.Value));
            await requestBlueprintParameters(supportedParameterMetadataToRequest.Where(sp => sp.IsBlueprintModel).ToList());
        };
        foreach (var ext in supportedParametersExtensionsResult)
            await ext.RegisterAddSupportedParametersHandler(this, getExtSupportedParameters);
    }
    private async Task requestBlueprintParameters(IReadOnlyCollection<SupportedParameterMetadata> parameters)
    {
        foreach (var spm in parameters)
        {
            ERDM_Parameter parameter = spm.Parameter;
            if (parameter == ERDM_Parameter.SUPPORTED_PARAMETERS)
                continue;
            ParameterBag parameterBag = new ParameterBag(parameter, ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define?.GetRequest.HasValue ?? false)
            {
                if (define.GetRequest.Value.GetIsEmpty())
                    await requestGetParameterWithEmptyPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                else if (parameter == ERDM_Parameter.PARAMETER_DESCRIPTION)
                    foreach (var pid in this.supportedParameters.Where(p => p.Value.IsManufacturerSpecific && p.Value.ParameterDescription is null).Select(p => (ushort)p.Key))
                    {
                        var result = await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice, pid);
                        spm.SetParameterDescription(result);
                    }
                else if (parameter == ERDM_Parameter.METADATA_JSON)
                {
                    // the device describes its manufacturer specific parameters as JSON, the MetadataJsonModule registers the defines
                    foreach (var pid in this.supportedParameters.Where(p => p.Value.IsManufacturerSpecific).Select(p => (ushort)p.Key).ToList())
                    {
                        // a device that does not know METADATA_JSON is asked once, not for every manufacturer specific parameter
                        if (!spm.IsSupported || spm.UnsupportedCommandClasses.HasFlag(ERDM_CommandClass.GET))
                            break;
                        await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice, pid);
                    }
                }
                else
                    await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
            }
            await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
        }
    }
    #endregion

    public bool IsModelOf(UID uid, SubDevice subDevice, RDMDeviceInfo other)
    {

        var deviceInfo = this.DeviceInfo;
        if (other is null || deviceInfo is null)
            return false;

        if (this.ManufacturerID != uid.ManufacturerID)
            return false;
        if (this.CurrentUsedSubDevice != subDevice)
            return false;
        if (deviceInfo.DeviceModelId != other.DeviceModelId)
            return false;
        if (deviceInfo.RdmProtocolVersionMajor != other.RdmProtocolVersionMajor)
            return false;
        if (deviceInfo.RdmProtocolVersionMinor != other.RdmProtocolVersionMinor)
            return false;
        if (deviceInfo.SoftwareVersionId != other.SoftwareVersionId)
            return false;
        if (deviceInfo.Dmx512NumberOfPersonalities != other.Dmx512NumberOfPersonalities)
            return false;
        if (deviceInfo.ProductCategoryCoarse != other.ProductCategoryCoarse)
            return false;
        if (deviceInfo.ProductCategoryFine != other.ProductCategoryFine)
            return false;
        if (deviceInfo.SensorCount != other.SensorCount)
            return false;

        return true;
    }
    protected sealed override Task OnResponseMessage(RDMMessage rdmMessage)
    {
        if (rdmMessage.ResponseType == ERDM_ResponseType.NACK_REASON)
            handleNACKReason(rdmMessage);
        return base.OnResponseMessage(rdmMessage);
    }

    internal bool handleNACKReason(RDMMessage rdmMessage)
    {
        if (rdmMessage.NackReason == ERDM_NackReason.FORMAT_ERROR || rdmMessage.NackReason == ERDM_NackReason.UNKNOWN_PID || rdmMessage.NackReason == ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS)
        {
            if (this.supportedParameters.TryGetValue(rdmMessage.Parameter, out SupportedParameterMetadata supportedParameterMetadata))
                supportedParameterMetadata.HandleNack(rdmMessage);
            return false;
        }
        return true;
    }

    private void RDMDeviceModel_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
    {
        if (e.Parameter == ERDM_Parameter.PARAMETER_DESCRIPTION)
        {
            if (e.Value is not RDMParameterDescription pd)
                return;

            var define = MetadataFactory.GetDefine(new ParameterBag((ERDM_Parameter)pd.ParameterId, this.CurrentUsedUID.ManufacturerID, this.DeviceInfo.DeviceModelId, this.DeviceInfo.SoftwareVersionId));
            if (define is null)
                MetadataFactory.AddDefineFromParameterDescription(this.CurrentUsedUID, this.CurrentUsedSubDevice, this.DeviceInfo, pd);
        }
    }

    public new void Dispose()
    {
        if (this.IsDisposed || this.IsDisposing)
            return;

        this.IsDisposing = true;
        this.PropertyChanged = null;
        this.Initialized = null;

        this.supportedParameters = null;
        base.Dispose();

        this.IsDisposed = true;
        this.IsDisposing = false;
    }

    public IReadOnlyCollection<RDMSensorDefinition> GetSensorDefinitions()
    {
        try
        {
            if (!parameterValues.TryGetValue(ERDM_Parameter.SENSOR_DEFINITION, out object value))
                return Array.Empty<RDMSensorDefinition>();
            else
            {
                if (value is ConcurrentDictionary<object, object> cd)
                    return cd.Values.Cast<RDMSensorDefinition>().ToArray();
                return value as RDMSensorDefinition[];
            }
        }
        catch
        {

        }
        return Array.Empty<RDMSensorDefinition>();
    }

    public override string ToString()
    {
        return $"{Enum.GetName(typeof(EManufacturer), Manufacturer)}";
    }
}