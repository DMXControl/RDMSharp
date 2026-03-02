using Microsoft.Extensions.Logging;
using RDMSharp.Extensions;
using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device;
using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp;

public interface IRDMRemoteDevice : IRDMDevice
{
    bool AllDataPulled { get; }
    bool Present { get; }
    DateTime LastSeen { get; }
    Task<bool> SetParameter(ERDM_Parameter parameter, object value = null);
}
public interface IRDMRemoteSubDevice : IRDMRemoteDevice
{
}
public abstract class AbstractRemoteRDMDevice : AbstractRDMDevice, IRDMRemoteDevice
{
    public sealed override bool IsGenerated => false;

    private readonly ConcurrentDictionary<byte, Sensor> sensors = new ConcurrentDictionary<byte, Sensor>();
    public sealed override IReadOnlyDictionary<byte, Sensor> Sensors { get { return sensors.AsReadOnly(); } }
    public sealed override IReadOnlyDictionary<ushort, Slot> Slots { get { return PersonalityModel?.Slots; } }


    private readonly ConcurrentDictionary<int, RDMStatusMessage> statusMessages = new ConcurrentDictionary<int, RDMStatusMessage>();
    public sealed override IReadOnlyDictionary<int, RDMStatusMessage> StatusMessages { get { return statusMessages.AsReadOnly(); } }

    protected ConcurrentQueue<ParameterUpdatedBag> ParameterUpdatedBag = new ConcurrentQueue<ParameterUpdatedBag>();

    private RDMDeviceInfo deviceInfo;
    public override RDMDeviceInfo DeviceInfo { get { return deviceInfo; } }

    private RDMDeviceModel deviceModel;
    public RDMDeviceModel DeviceModel
    {
        get { return deviceModel; }
        private set
        {
            if (deviceModel == value)
                return;
            deviceModel = value;
            OnPropertyChanged(nameof(DeviceModel));
        }
    }
    private RDMPersonalityModel personalityModel;
    public RDMPersonalityModel PersonalityModel
    {
        get { return personalityModel; }
        private set
        {
            if (personalityModel == value)
                return;
            personalityModel = value;
            OnPropertyChanged(nameof(PersonalityModel));
        }
    }
    public sealed override IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
    {
        get
        {
            if (DeviceModel == null)
                return parameterValues.AsReadOnly();

            return DeviceModel.ParameterValues.Where(x => !parameterValues.ContainsKey(x.Key)).Concat(parameterValues).ToDictionary(k => k.Key, v => v.Value).AsReadOnly();
        }
    }
    public sealed override IReadOnlyDictionary<ERDM_Parameter, ParameterMetadata> ParameterMetadataBag
    {
        get
        {
            IReadOnlyDictionary<ERDM_Parameter, ParameterMetadata> deviceModelParameterMetadataBag = null;
            IReadOnlyDictionary<ERDM_Parameter, ParameterMetadata> personalityModelParameterMetadataBag = null;
            if (PersonalityModel is not null)
                personalityModelParameterMetadataBag = PersonalityModel.ParameterMetadataBag.Where(x => !parameterMetadataBag.ContainsKey(x.Key)).ToDictionary(k => k.Key, v => v.Value).AsReadOnly();
            if (DeviceModel is not null)
                deviceModelParameterMetadataBag = DeviceModel.ParameterMetadataBag.Where(x => !parameterMetadataBag.ContainsKey(x.Key) && !(personalityModelParameterMetadataBag?.ContainsKey(x.Key) ?? false)).ToDictionary(k => k.Key, v => v.Value).AsReadOnly();

            if (deviceModelParameterMetadataBag is not null || personalityModelParameterMetadataBag is not null)
                return parameterMetadataBag.Concat(deviceModelParameterMetadataBag ?? Enumerable.Empty<KeyValuePair<ERDM_Parameter, ParameterMetadata>>())
                    .Concat(personalityModelParameterMetadataBag ?? Enumerable.Empty<KeyValuePair<ERDM_Parameter, ParameterMetadata>>())
                    .ToDictionary(k => k.Key, v => v.Value)
                    .AsReadOnly();

            return parameterMetadataBag.AsReadOnly();
        }
    }

    private readonly List<IModule> _modules = new List<IModule>();
    public IReadOnlyCollection<IModule> Modules { get => _modules.AsReadOnly(); }

    private bool allDataPulled;
    public bool AllDataPulled
    {
        get
        {
            return allDataPulled;
        }
        private set
        {
            if (allDataPulled == value)
                return;
            allDataPulled = value;
            OnPropertyChanged(nameof(AllDataPulled));
        }
    }

    private DateTime lastSeen;
    public DateTime LastSeen
    {
        get
        {
            return lastSeen;
        }
        private set
        {
            if (lastSeen == value)
                return;
            lastSeen = value;
            OnPropertyChanged(nameof(LastSeen));
        }
    }

    private DateTime lastSendQueuedMessage;

    private bool present;
    public bool Present
    {
        get
        {
            return present;
        }
        internal set
        {
            if (present == value)
                return;
            present = value;
            OnPropertyChanged(nameof(Present));
        }
    }

    public AbstractRemoteRDMDevice(UID uid) : base(uid)
    {
    }
    public AbstractRemoteRDMDevice(UID uid, SubDevice? subDevice = null) : base(uid, subDevice)
    {
        if (subDevice.HasValue && subDevice.Value.IsBroadcast)
            throw new NotSupportedException("A SubDevice can't be Broadcast.");
    }
    protected override async Task initialize(RDMDeviceInfo deviceInfo = null)
    {
        Logger?.LogDebug($"Remote RDM Device {UID} SubDevice: {Subdevice} initializing.");
        await base.initialize(deviceInfo);
        GlobalTimers.Instance.PresentUpdateTimerElapsed += Instance_PresentUpdateTimerElapsed;
        ParameterValueAdded += AbstractRDMDevice_ParameterValueAdded;
        ParameterValueChanged += AbstractRDMDevice_ParameterValueChanged;
        ParameterRequested += AbstractRDMDevice_ParameterRequested;
        await requestDeviceInfo(deviceInfo);
        UpdateModules();
        Logger?.LogDebug($"Remote RDM Device {UID} SubDevice: {Subdevice} initialized.");
    }

    private void UpdateModules()
    {
        if (ExtensionsManager.Instance.TryGetMatchingModules(this, out var modules))
        {
            this._modules.Clear();
            this._modules.AddRange(modules);
            OnPropertyChanged(nameof(Modules));
        }
    }

    private void Instance_PresentUpdateTimerElapsed(object sender, EventArgs e)
    {
        Present = DateTime.UtcNow - lastSeen < TimeSpan.FromMilliseconds(GlobalTimers.Instance.PresentLostTime);
    }

    private async void DeviceModel_Initialized(object sender, EventArgs e)
    {
        deviceModel.Initialized -= DeviceModel_Initialized;
        deviceModel.ParameterValueAdded -= DeviceModel_ParameterValueAdded;
        await getPersonalityModelAndCollectAllParameters();
        await collectParameters();
    }
    private async Task collectParameters()
    {
        await collectAllParametersOnRoot();
        await scanSubDevices();
        AllDataPulled = true;
        GlobalTimers.Instance.ParameterUpdateTimerElapsed += Instance_ParameterUpdateTimerElapsed;
    }

    private async Task scanSubDevices()
    {
        if (DeviceInfo.SubDeviceCount == 0)
            return;
        if (!this.Subdevice.IsRoot)
            return;

        ushort foundSubDevices = 0;
        ushort currentID = 0;
        var dict = new Dictionary<SubDevice, RDMDeviceInfo>();
        while (foundSubDevices < DeviceInfo.SubDeviceCount)
        {
            currentID++;
            if (currentID > 512)
                break;

            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, new SubDevice(currentID), new ParameterBag(ERDM_Parameter.DEVICE_INFO));
            await runPeerToPeerProcess(ptpProcess);
            if (ptpProcess.ResponsePayloadObject.ParsedObject is RDMDeviceInfo _deviceInfo)
            {
                foundSubDevices++;
                var subDevice = createSubDevice(UID, ptpProcess.SubDevice);
                this.SubDevices_Internal.Add(subDevice);
                dict.Add(ptpProcess.SubDevice, _deviceInfo);
            }
            await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
        }
        foreach (AbstractRemoteRDMDevice sd in this.SubDevices_Internal)
        {
            if (sd.Subdevice.IsRoot)
                continue;
            sd.performInitialize(dict[sd.Subdevice]);
        }
    }

    protected abstract IRDMRemoteSubDevice createSubDevice(UID uid, SubDevice subDevice);

    #region Requests

    private async Task requestDeviceInfo(RDMDeviceInfo deviceInfo = null)
    {
        DataTreeBranch? rpl = null;
        if (deviceInfo == null)
        {
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, Subdevice, new ParameterBag(ERDM_Parameter.DEVICE_INFO));
            await runPeerToPeerProcess(ptpProcess);
            rpl = ptpProcess.ResponsePayloadObject;
            if (rpl.Value.ParsedObject is RDMDeviceInfo _deviceInfo)
            {
                this.deviceInfo = _deviceInfo;
                updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, ptpProcess.ResponsePayloadObject);
                updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), ptpProcess.ResponsePayloadObject);
                await getDeviceModelAndCollectAllParameters();
            }
        }
        else
        {
            rpl = DataTreeBranch.FromObject(deviceInfo, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO);
            this.deviceInfo = deviceInfo;
            updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, rpl.Value);
            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), rpl.Value);
            await getDeviceModelAndCollectAllParameters();
        }
    }
    private async Task requestParameters()
    {
        var parameters = this.DeviceModel?.GetSupportedNonBlueprintParameters().Select(spm => spm.Parameter);
        if (parameters == null)
            return;

        foreach (ERDM_Parameter parameter in parameters)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_INFO:
                    continue;
                case ERDM_Parameter.QUEUED_MESSAGE:
                    tryAddParameterMetadata(parameter);
                    continue;
                case ERDM_Parameter.STATUS_MESSAGES:
                    await requestParameter(parameter, ERDM_Status.NONE);
                    continue;
            }
            tryAddParameterMetadata(parameter);
            await requestParameter(parameter);
            await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
        }
    }
    private async Task requestParameter(ERDM_Parameter parameter, object payload = null)
    {
        try
        {
            ParameterBag parameterBag = new ParameterBag(parameter, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define == null)
            {
                ConcurrentDictionary<object, object> pd = null;
                if (this.deviceModel.ParameterValues.TryGetValue(ERDM_Parameter.PARAMETER_DESCRIPTION, out var obj))
                    pd = obj as ConcurrentDictionary<object, object>;

                if ((pd?.TryGetValue((ushort)parameter, out var desc) ?? false) && desc is RDMParameterDescription pDesc)
                {
                    if (pDesc.CommandClass.HasFlag(ERDM_CommandClass.GET))
                        await requestGetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                }
            }
            if (define?.GetRequest.HasValue ?? false)
            {
                if (define.GetRequest.Value.GetIsEmpty())
                    await requestGetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                else
                    await requestGetParameterWithPayload(parameterBag, define, UID, Subdevice, payload);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex);
        }
    }
    public async Task RequestParameter(ERDM_Command command, ERDM_Parameter parameter)
    {
        ParameterBag parameterBag = new ParameterBag(parameter, this.UID.ManufacturerID, this.deviceInfo.DeviceModelId, this.deviceInfo.SoftwareVersionId);

        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(command, this.UID, this.Subdevice, parameterBag);
        await runPeerToPeerProcess(ptpProcess);
        if (!ptpProcess.ResponsePayloadObject.IsUnset)
        {
            updateParameterValuesDependeciePropertyBag(ptpProcess.ParameterBag.PID, ptpProcess.ResponsePayloadObject);
            updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ptpProcess.ParameterBag.PID), ptpProcess.ResponsePayloadObject);
        }
    }
    public async Task<object> RequestParameterWithPayload(ERDM_Command command, ERDM_Parameter parameter, object payload)
    {
        ParameterBag parameterBag = new ParameterBag(parameter, this.UID.ManufacturerID, this.deviceInfo.DeviceModelId, this.deviceInfo.SoftwareVersionId);
        DataTreeBranch dataTreeBranch = DataTreeBranch.FromObject(payload, null, parameterBag, command);

        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(command, this.UID, this.Subdevice, parameterBag, dataTreeBranch);
        await ptpProcess.Run();
        if (ptpProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Finished)
            return ptpProcess.ResponsePayloadObject.ParsedObject ?? ptpProcess.ResponsePayloadObject;

        return null;
    }

    private SemaphoreSlim updateSenaphoreSlim = new SemaphoreSlim(1);
    private async Task updateParameters()
    {
        var supportedParameters = this.DeviceModel?.GetSupportedNonBlueprintParameters();

        if (updateSenaphoreSlim.CurrentCount == 0)
            return;

        await updateSenaphoreSlim.WaitAsync();
        try
        {
            await requestQueuedMessages();
            var loopDetectionCache = new HashSet<ParameterUpdatedBag>();
            while (ParameterUpdatedBag.TryPeek(out ParameterUpdatedBag bag))
            {
                TimeSpan updateTime = TimeSpan.FromMilliseconds(GlobalTimers.Instance.NonQueuedUpdateTime);
                //if (bag.Parameter.GetAttribute<ParameterUpdateTimeAttribute>() is ParameterUpdateTimeAttribute attribute)
                //    updateTime = TimeSpan.FromMilliseconds(attribute.Milliseconds);
                if ((supportedParameters.FirstOrDefault(spm => spm.Parameter == bag.Parameter)?.ParameterUpdateTimeMilliseconds is int milliseconds) && milliseconds >= 0)
                    updateTime = TimeSpan.FromMilliseconds(milliseconds);

                if (loopDetectionCache.Any(b => b.Parameter == bag.Parameter && b.Index == bag.Index))
                    return;

                if (DateTime.UtcNow - bag.Timestamp < updateTime)
                {
                    ParameterUpdatedBag.TryDequeue(out _);
                    ParameterUpdatedBag.Enqueue(bag);

                    loopDetectionCache.Add(bag);
                    continue;
                }

                loopDetectionCache.Add(bag);

                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMilliseconds(GlobalTimers.Instance.ParameterUpdateTimerInterval));
                var task = Task.Run(async () =>
                {
                    Stopwatch sw = new Stopwatch();
                    sw?.Restart();
                    await requestParameter(bag.Parameter, bag.Index);
                    sw?.Stop();
                    Logger?.LogTrace($"Parameter update for {bag.Parameter} with index {bag.Index} took {sw.ElapsedMilliseconds}ms");

                    UpdateParameterUpdatedBag(bag.Parameter, bag.Index);
                }, cts.Token);
                await task;

                if (task.IsCompletedSuccessfully)
                    await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenNonQueuedUpdateRequests);
                else
                    Logger?.LogTrace(task.Exception, $"Parameter update for {bag.Parameter} with index {bag.Index} failed: {task.Exception?.Message}");
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex);
        }
        finally
        {
            updateSenaphoreSlim.Release();
        }
    }

    private async Task requestQueuedMessages()
    {
        var supportedParameters = this.deviceModel?.GetSupportedNonBlueprintParameters();
        if (supportedParameters.FirstOrDefault(spm => spm.Parameter == ERDM_Parameter.QUEUED_MESSAGE)?.IsSupported ?? false)
        {
            if (DateTime.UtcNow - lastSendQueuedMessage < TimeSpan.FromMilliseconds(GlobalTimers.Instance.QueuedUpdateTime))
                return;
            ParameterBag parameterBag = new ParameterBag(ERDM_Parameter.QUEUED_MESSAGE, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define.GetRequest.HasValue)
            {
                byte mc = 0;
                do
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromMilliseconds(GlobalTimers.Instance.ParameterUpdateTimerInterval));
                    var task = Task.Run(async () =>
                    {
                        lastSendQueuedMessage = DateTime.UtcNow;

                        Stopwatch sw = new Stopwatch();
                        sw?.Restart();
                        var result = await requestGetParameterWithPayload(parameterBag, define, UID, Subdevice, ERDM_Status.ADVISORY);
                        mc = result.MessageCounter;
                        sw?.Stop();
                        if (result.State == PeerToPeerProcess.EPeerToPeerProcessState.Failed)
                            return;
                        Logger?.LogTrace($"Queued Parameter update took {sw.ElapsedMilliseconds}ms for {mc} messages.");
                    }, cts.Token);
                    await task;

                    if (task.IsCompletedSuccessfully)
                        await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenQueuedUpdateRequests);
                    else
                    {
                        Logger?.LogTrace(task.Exception, $"Queue Parameter update failed: {task.Exception?.Message}");
                        return;
                    }

                }
                while (mc != 0);
                return;
            }
        }
    }

    #endregion
    private async Task getDeviceModelAndCollectAllParameters()
    {
        if (deviceModel != null)
            return;
        deviceModel = RDMDeviceModel.getDeviceModel(UID, Subdevice, DeviceInfo, new Func<RDMMessage, Task>(RDMSharp.Instance.SendMessage));
        if (!deviceModel.IsInitialized)
        {
            deviceModel.Initialized += DeviceModel_Initialized;
            deviceModel.ParameterValueAdded += DeviceModel_ParameterValueAdded;
            if (!deviceModel.IsInitializing)
                await deviceModel.Initialize();
            else
                InvkoeDeviceModelParameterValueAdded();
        }
        else
        {
            InvkoeDeviceModelParameterValueAdded();
            await collectParameters();
        }
        void InvkoeDeviceModelParameterValueAdded()
        {
            foreach (var item in this.deviceModel.ParameterValues)
                base.InvokeParameterValueAdded(new ParameterValueAddedEventArgs(item.Key, item.Value));
        }
    }

    private void DeviceModel_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
    {
        base.InvokeParameterValueAdded(e);
    }

    private async Task collectAllParametersOnRoot()
    {
        await requestParameters();
    }

    private async Task getPersonalityModelAndCollectAllParameters()
    {
        byte? personalityId = DeviceInfo.Dmx512CurrentPersonality.Value;
        if (parameterValues.TryGetValue(ERDM_Parameter.DMX_PERSONALITY, out object value) && value is RDMDMXPersonality personality)
            personalityId = personality.CurrentPersonality;

        PersonalityModel = DeviceModel.getPersonalityModel(this, personalityId ?? 0);
        if (!PersonalityModel.IsInitialized)
            await PersonalityModel.Initialize();
    }

    private async void AbstractRDMDevice_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
    {
        switch (e.Parameter)
        {
            case ERDM_Parameter.DMX_PERSONALITY:
                if (DeviceModel.IsInitialized)
                    await getPersonalityModelAndCollectAllParameters();
                break;
            case ERDM_Parameter.SENSOR_VALUE when e.Value is RDMSensorValue sensorValue:
                var sensor = sensors.GetOrAdd(sensorValue.SensorId, (a) => new Sensor(a));
                if (deviceModel.GetSensorDefinitions()?.FirstOrDefault(a => a.SensorId == sensorValue.SensorId) is RDMSensorDefinition sd)
                    sensor.UpdateDescription(sd);
                sensor.UpdateValue(sensorValue);
                break;
        }
    }
    private async void AbstractRDMDevice_ParameterValueChanged(object sender, ParameterValueChangedEventArgs e)
    {
        switch (e.Parameter)
        {
            case ERDM_Parameter.DEVICE_INFO:
                if (this.PersonalityModel.PersonalityID != this.DeviceInfo.Dmx512CurrentPersonality)
                    goto case ERDM_Parameter.DMX_PERSONALITY;
                break;
            case ERDM_Parameter.DMX_PERSONALITY:
                if (DeviceModel.IsInitialized)
                    await getPersonalityModelAndCollectAllParameters();
                break;
            case ERDM_Parameter.SENSOR_VALUE when e.NewValue is RDMSensorValue sensorValue:
                if (sensorValue.SensorId == byte.MaxValue) //Ignore Broadcast as in Spec.
                    break;
                var sensor = sensors.GetOrAdd(sensorValue.SensorId, (a) => new Sensor(a));
                sensor.UpdateValue(sensorValue);
                break;
        }
    }
    private void AbstractRDMDevice_ParameterRequested(object sender, ParameterRequestedEventArgs e)
    {
        LastSeen = DateTime.UtcNow;
        UpdateParameterUpdatedBag(e.Parameter, e.Index);
    }
    private void UpdateParameterUpdatedBag(ERDM_Parameter parameter, object index)
    {
        if (!Constants.BLUEPRINT_MODEL_PARAMETERS.Contains(parameter) && !Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS.Contains(parameter))
        {
            if (ParameterUpdatedBag.Any(p => p.Parameter == parameter && p.Index == index))
            {
                var tempQueue = new ConcurrentQueue<ParameterUpdatedBag>();
                while (ParameterUpdatedBag.TryDequeue(out var item))
                    if (!(item.Parameter.Equals(parameter) && Equals(item.Index, index)))
                        tempQueue.Enqueue(item);


                while (tempQueue.TryDequeue(out var item))
                    ParameterUpdatedBag.Enqueue(item);
            }
            ParameterUpdatedBag.Enqueue(new ParameterUpdatedBag(parameter, index));
        }
    }

    public async Task<bool> SetParameter(ERDM_Parameter parameter, object value = null)
    {
        try
        {
            ParameterBag parameterBag = new ParameterBag(parameter, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define.SetRequest.HasValue)
            {
                if (define.SetRequest.Value.GetIsEmpty())
                    return await requestSetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                else
                    return await requestSetParameterWithPayload(parameterBag, define, UID, Subdevice, value);
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(string.Empty, e);
        }
        return false;
    }

    protected sealed override async Task OnResponseMessage(RDMMessage rdmMessage)
    {
        LastSeen = DateTime.UtcNow;

        if (rdmMessage?.ResponseType == ERDM_ResponseType.NACK_REASON)
            this.deviceModel?.handleNACKReason(rdmMessage);

        await base.OnResponseMessage(rdmMessage);
    }

    public sealed override IReadOnlyDictionary<ERDM_Parameter, object> GetAllParameterValues()
    {

        IReadOnlyDictionary<ERDM_Parameter, object> res = null;
        if (this.DeviceModel is not null)
            res = this.DeviceModel.ParameterValues;
        if (this.PersonalityModel is not null)
            res = res.Concat(this.PersonalityModel.ParameterValues)
                .ToLookup(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, g => g.First()); ;
        if (res is not null)
            return res.Concat(this.ParameterValues)
                .ToLookup(x => x.Key, x => x.Value)
                .ToDictionary(x => x.Key, g => g.First());
        else
            return base.GetAllParameterValues();
    }


    private async void Instance_ParameterUpdateTimerElapsed(object sender, EventArgs e)
    {
        await updateParameters();
    }

    public override string ToString()
    {
        return $"{base.ToString()} {this.DeviceModel}";
    }
    protected sealed override void OnDispose()
    {
        GlobalTimers.Instance.PresentUpdateTimerElapsed -= Instance_PresentUpdateTimerElapsed;
        GlobalTimers.Instance.ParameterUpdateTimerElapsed -= Instance_ParameterUpdateTimerElapsed;
        try
        {
            onDispose();
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
        ParameterValueAdded -= AbstractRDMDevice_ParameterValueAdded;
        ParameterValueChanged -= AbstractRDMDevice_ParameterValueChanged;
        ParameterRequested -= AbstractRDMDevice_ParameterRequested;
    }
    protected abstract void onDispose();


}