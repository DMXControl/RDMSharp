using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            var kdm = knownDeviceModels.Values.FirstOrDefault(dm => dm.IsModelOf(uid, deviceInfo));
            if (kdm == null)
            {
                kdm = new RDMDeviceModel(uid, subDevice, deviceInfo, sendRdmFunktion);
                knownDeviceModels.TryAdd(kdm.GetHashCode(), kdm);
            }

            return kdm;

        }


        private ConcurrentDictionary<byte, RDMPersonalityModel> knownPersonalityModels = new ConcurrentDictionary<byte, RDMPersonalityModel>();
        public IReadOnlyCollection<RDMPersonalityModel> KnownPersonalityModels => knownPersonalityModels.Values.ToList();
        internal async Task<RDMPersonalityModel> getPersonalityModel(AbstractRemoteRDMDevice remoteRDMDevice)
        {
            try
            {
                if (!DeviceInfo.Dmx512CurrentPersonality.HasValue)
                    return null;
                var kpm = knownPersonalityModels.Values.FirstOrDefault(dm => dm.IsModelOf(
                    remoteRDMDevice.UID,
                    remoteRDMDevice.DeviceInfo.DeviceModelId,
                    remoteRDMDevice.DeviceInfo.SoftwareVersionId,
                    remoteRDMDevice.DeviceInfo.Dmx512CurrentPersonality.Value));
                if (kpm == null)
                {
                    kpm = new RDMPersonalityModel(
                        remoteRDMDevice.UID,
                        remoteRDMDevice.Subdevice,
                        remoteRDMDevice.DeviceInfo.DeviceModelId,
                        remoteRDMDevice.DeviceInfo.SoftwareVersionId,
                        remoteRDMDevice.DeviceInfo.Dmx512CurrentPersonality.Value,
                        sendRdmFunktion
                        );
                    if (knownPersonalityModels.TryAdd(kpm.PersonalityID, kpm))
                    {
                        currentUsedUID = remoteRDMDevice.UID;
                        currentUsedSubDevice = remoteRDMDevice.Subdevice;
                        DeviceInfo = remoteRDMDevice.DeviceInfo;
                        var di= remoteRDMDevice.parameterValuesDataTreeBranch.FirstOrDefault(d => d.Key.Parameter == ERDM_Parameter.DEVICE_INFO);
                        updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, di.Value);
                        await requestPersonalityBlueprintParameters(kpm);
                    }
                }
                return kpm;
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        public new bool IsDisposing { get; private set; }
        public new bool IsDisposed { get; private set; }

        public bool IsInitialized { get; private set; } = false;

        public event EventHandler Initialized;
        public event PropertyChangedEventHandler PropertyChanged;

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
            get { return this.SupportedParameters.Except(SupportedBlueprintParameters).OrderBy(p => p).ToList().AsReadOnly(); }
        }

        public IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters
        {
            get { return this.supportedParameters.Where(sp => !sp.Value).Select(sp => sp.Key).OrderBy(p => p).ToArray().AsReadOnly(); }
        }



        private readonly Func<RDMMessage, Task> sendRdmFunktion;

        internal RDMDeviceModel(UID uid, SubDevice sudevice, RDMDeviceInfo deviceInfo, Func<RDMMessage, Task> sendRdmFunktion)
        {
            this.sendRdmFunktion = sendRdmFunktion;
            DeviceInfo = deviceInfo;
            CurrentUsedUID = uid;
            CurrentUsedSubDevice = sudevice;
            ManufacturerID = uid.ManufacturerID;
            Manufacturer = (EManufacturer)uid.ManufacturerID;
        }
        internal async Task Initialize()
        {
            if (IsInitialized)
                return;

            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMMessage);

            await requestSupportedParameters();
            await requestBlueprintParameters();
            await requestPersonalityBlueprintParameters();

            asyncRDMRequestHelper.Dispose();
            asyncRDMRequestHelper = null;

            IsInitialized = true;
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
            }
        }

        private async Task requestBlueprintParameters()
        {
            foreach (ERDM_Parameter parameter in this.SupportedBlueprintParameters)
            {
                ParameterBag parameterBag = new ParameterBag(parameter, ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define.GetRequest.HasValue)
                {
                    if (define.GetRequest.Value.GetIsEmpty())
                        await requestGetParameterWithEmptyPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                    else
                        await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                }
            }
        }
        private async Task requestPersonalityBlueprintParameters(RDMPersonalityModel personalityModel = null)
        {
            if (personalityModel == null)
            {
                personalityModel = new RDMPersonalityModel(
                        currentUsedUID,
                        currentUsedSubDevice,
                        DeviceInfo.DeviceModelId,
                        DeviceInfo.SoftwareVersionId,
                        DeviceInfo.Dmx512CurrentPersonality.Value,
                        sendRdmFunktion
                        );
                knownPersonalityModels.TryAdd(personalityModel.PersonalityID, personalityModel);
            }
            var backup = asyncRDMRequestHelper;
            try
            {
                asyncRDMRequestHelper = personalityModel.GetAsyncRDMRequestHelper();
                this.ParameterValueAdded += personalityModel.RDMDeviceModel_ParameterValueAdded;
                foreach (ERDM_Parameter parameter in this.SupportedPersonalityBlueprintParameters)
                {
                    ParameterBag parameterBag = new ParameterBag(parameter, personalityModel.ManufacturerID, personalityModel.DeviceModelID, personalityModel.SoftwareVersionID);
                    var define = MetadataFactory.GetDefine(parameterBag);
                    if (define.GetRequest.HasValue)
                    {
                        if (define.GetRequest.Value.GetIsEmpty())
                            await requestGetParameterWithEmptyPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                        else
                            await requestGetParameterWithPayload(parameterBag, define, CurrentUsedUID, CurrentUsedSubDevice);
                    }
                }
            }
            finally
            {
                this.ParameterValueAdded -= personalityModel.RDMDeviceModel_ParameterValueAdded;
                asyncRDMRequestHelper = backup;
                personalityModel.DisposeAsyncRDMRequestHelper();
            }
        }

        #endregion


        private async Task sendRDMMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = CurrentUsedUID;
            await sendRdmFunktion.Invoke(rdmMessage);
        }

        internal async Task ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage.SourceUID != CurrentUsedUID)
                return;

            if (!rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            asyncRDMRequestHelper?.ReceiveMessage(rdmMessage);
        }

        public bool IsModelOf(UID uid, RDMDeviceInfo other)
        {
            var deviceInfo = this.DeviceInfo;
            if (this.ManufacturerID != uid.ManufacturerID)
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

        internal bool handleNACKReason(RDMMessage rdmMessage)
        {
            if (rdmMessage.NackReason.Contains(ERDM_NackReason.ACTION_NOT_SUPPORTED))
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