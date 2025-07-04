using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp
{
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


        private ConcurrentDictionary<byte, RDMPersonalityModel> knownPersonalityModels = new ConcurrentDictionary<byte, RDMPersonalityModel>();
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

        private ConcurrentDictionary<ERDM_Parameter, bool> supportedParameters = new ConcurrentDictionary<ERDM_Parameter, bool>();
        public IReadOnlyCollection<ERDM_Parameter> SupportedParameters
        {
            get { return this.supportedParameters.Where(sp => sp.Value).Select(sp => sp.Key).Where(p => ((ushort)p > 0x000F)).OrderBy(p => p).ToArray().AsReadOnly(); }
        }
        public IReadOnlyCollection<ERDM_Parameter> SupportedBlueprintParameters
        {
            get { return this.SupportedParameters.Intersect(Constants.BLUEPRINT_MODEL_PARAMETERS).OrderBy(p => p).ToList().AsReadOnly(); }
        }
        public IReadOnlyCollection<ERDM_Parameter> SupportedPersonalityBlueprintParameters
        {
            get { return this.SupportedParameters.Intersect(Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS).OrderBy(p => p).ToList().AsReadOnly(); }
        }
        public IReadOnlyCollection<ERDM_Parameter> SupportedNonBlueprintParameters
        {
            get { return this.SupportedParameters.Except(SupportedBlueprintParameters).Except(SupportedPersonalityBlueprintParameters).OrderBy(p => p).ToList().AsReadOnly(); }
        }

        public IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters
        {
            get { return this.supportedParameters.Where(sp => !sp.Value).Select(sp => sp.Key).OrderBy(p => p).ToArray().AsReadOnly(); }
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

        private SemaphoreSlim initializeSemaphoreSlim = new SemaphoreSlim(1);
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
                await requestSupportedParameters();
                await requestBlueprintParameters();
                //await requestPersonalityBlueprintParameters();

                IsInitialized = true;
            }
            finally
            {
                initializeSemaphoreSlim.Release();
                IsInitializing = false;
            }
            Initialized?.Invoke(this, EventArgs.Empty);
        }



        #region Requests
        private async Task requestSupportedParameters()
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
                        supportedParameters.TryAdd(para, true);
                }
                if (DeviceInfo.Dmx512StartAddress.HasValue && DeviceInfo.Dmx512StartAddress >= 1 && DeviceInfo.Dmx512StartAddress.Value <= 512) // Remote Device not send DMX_START_ADDRESS Parameter but uses it!
                    supportedParameters.TryAdd(ERDM_Parameter.DMX_START_ADDRESS, true);

                if (DeviceInfo.Dmx512CurrentPersonality.HasValue) // Remote Device not send DMX_PERSONALITY Parameter but uses it!
                    supportedParameters.TryAdd(ERDM_Parameter.DMX_PERSONALITY, true);

                if (!this.supportedParameters.ContainsKey(ERDM_Parameter.PARAMETER_DESCRIPTION) && this.supportedParameters.Any(p=> ((ushort)p.Key) >= 0x8000 && ((ushort)p.Key) <= 0xFFDF)) // Remote Device not send PARAMETER_DESCRIPTION Parameter but has Manufacture speific Parameters it!
                    this.supportedParameters.TryAdd(ERDM_Parameter.PARAMETER_DESCRIPTION, true);

                if (!this.supportedParameters.ContainsKey(ERDM_Parameter.IDENTIFY_DEVICE)) //Test it if the device supports Identify Device Parameter, if not it will be labled as not supported later on
                    this.supportedParameters.TryAdd(ERDM_Parameter.IDENTIFY_DEVICE, true);

                if (!this.supportedParameters.ContainsKey(ERDM_Parameter.SOFTWARE_VERSION_LABEL))//Test it if the device supports Software Version Lable Device Parameter, if not it will be labled as not supported later on
                    this.supportedParameters.TryAdd(ERDM_Parameter.SOFTWARE_VERSION_LABEL, true);

                if (!this.supportedParameters.ContainsKey(ERDM_Parameter.FACTORY_DEFAULTS))//Test it if the device supports Factory Defaults Device Parameter, if not it will be labled as not supported later on
                    this.supportedParameters.TryAdd(ERDM_Parameter.FACTORY_DEFAULTS, true);

                if(this.supportedParameters.Any(p=>((ushort)p.Key)>0x9000 && ((ushort)p.Key) <= 0x900D))
                    this.supportedParameters.TryAdd(ERDM_Parameter.ENDPOINT_LIST, true);
            }
            await Task.Delay(GlobalTimers.Instance.UpdateDelayBetweenRequests);
        }

        private async Task requestBlueprintParameters()
        {
            var parameters = this.SupportedBlueprintParameters.OrderBy(p => (ushort)p).ToList();
            foreach (ERDM_Parameter parameter in parameters)
            {
                if (parameter == ERDM_Parameter.SUPPORTED_PARAMETERS)
                    continue;
                ParameterBag parameterBag = new ParameterBag(parameter, ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define.GetRequest.HasValue)
                {
                    if (define.GetRequest.Value.GetIsEmpty())
                        await requestGetParameterWithEmptyPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                    else if (parameter == ERDM_Parameter.PARAMETER_DESCRIPTION)
                        foreach (var pid in this.supportedParameters.Where(p => (ushort)p.Key >= 0x8000 && (ushort)p.Key <= 0xFFDF).Select(p => (ushort)p.Key))
                            await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice, pid);
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
            if (rdmMessage.NackReason.Contains(ERDM_NackReason.ACTION_NOT_SUPPORTED) || rdmMessage.NackReason.Contains(ERDM_NackReason.UNKNOWN_PID))
            {
                AddParameterToKnownNotSupportedParameters(rdmMessage.Parameter);
                return false;
            }
            return true;
        }

        internal void AddParameterToKnownNotSupportedParameters(ERDM_Parameter parameter)
        {
            this.supportedParameters.AddOrUpdate(parameter, false, (x, y) => false);
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
}