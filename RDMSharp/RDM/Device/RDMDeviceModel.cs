using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON.OneOfTypes;
using RDMSharp.ParameterWrapper;
using RDMSharp.ParameterWrapper.Generic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RDMSharp
{
    public sealed class RDMDeviceModel : IRDMDeviceModel
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

        public ushort ManufacturerID { get; private set; }
        public EManufacturer Manufacturer { get; private set; }
        public UID CurrentUsedUID { get; private set; }
        public SubDevice CurrentUsedSubDevice { get; private set; }

        public event EventHandler Initialized;
        public bool IsInitialized { get; private set; } = false;

        private AsyncRDMRequestHelper asyncRDMRequestHelper;

        private readonly ConcurrentDictionary<ushort, IRDMParameterWrapper> manufacturerParameter = new ConcurrentDictionary<ushort, IRDMParameterWrapper>();


        private ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch> parameterValuesDataTreeBranch = new ConcurrentDictionary<ParameterDataCacheBag, DataTreeBranch>();
        private ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object> parameterValuesDependeciePropertyBag = new ConcurrentDictionary<DataTreeObjectDependeciePropertyBag, object>();

        private ConcurrentDictionary<ERDM_Parameter, object> parameterValues = new ConcurrentDictionary<ERDM_Parameter, object>();
        public IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get { return this.parameterValues?.AsReadOnly(); }
        }

        public RDMDeviceInfo DeviceInfo
        {
            get { return parameterValues.TryGetValue(ERDM_Parameter.DEVICE_INFO, out object value) ? value as RDMDeviceInfo : null; }
            private set
            {
                if (this.DeviceInfo == value)
                    return;

                this.parameterValues[ERDM_Parameter.DEVICE_INFO] = value;
                var dataTreeBranch = DataTreeBranch.FromObject(value, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO);
                updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, dataTreeBranch);
                parameterValuesDataTreeBranch[new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO)] = dataTreeBranch;
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
        public IReadOnlyCollection<ERDM_Parameter> SupportedNonBlueprintParameters
        {
            get { return this.SupportedParameters.Except(SupportedBlueprintParameters).OrderBy(p => p).ToList().AsReadOnly(); }
        }

        public IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters
        {
            get { return this.supportedParameters.Where(sp => !sp.Value).Select(sp => sp.Key).OrderBy(p => p).ToArray().AsReadOnly(); }
        }


        public bool IsDisposing { get; private set; }
        public bool IsDisposed { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;
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

            asyncRDMRequestHelper.Dispose();
            asyncRDMRequestHelper = null;

            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }


        private async Task runPeerToPeerProcess(PeerToPeerProcess ptpProcess)
        {
            await ptpProcess?.Run(asyncRDMRequestHelper);
        }

        private void updateParameterValuesDependeciePropertyBag(ERDM_Parameter parameter, DataTreeBranch dataTreeBranch)
        {
            object obj = dataTreeBranch.ParsedObject;
            if (obj == null)
                return;

            foreach (var p in obj.GetType().GetProperties().Where(p => p.GetCustomAttributes<DataTreeObjectDependeciePropertyAttribute>().Any()).ToList())
            {
                object value = p.GetValue(obj);
                foreach (var item in p.GetCustomAttributes<DataTreeObjectDependeciePropertyAttribute>())
                    parameterValuesDependeciePropertyBag.AddOrUpdate(item.Bag, value, (o1, o2) => value);
            }
        }

        #region Requests
        private async Task requestSupportedParameters()
        {
            ParameterBag parameterBag = new ParameterBag(ERDM_Parameter.SUPPORTED_PARAMETERS);
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, CurrentUsedUID, CurrentUsedSubDevice, parameterBag);
            await runPeerToPeerProcess(ptpProcess);

            if (!ptpProcess.ResponsePayloadObject.IsUnset)
                parameterValuesDataTreeBranch.AddOrUpdate(new ParameterDataCacheBag(parameterBag.PID), ptpProcess.ResponsePayloadObject, (o1, o2) => ptpProcess.ResponsePayloadObject);

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
                        await requestParameterWithEmptyPayload(parameterBag, define);
                    else
                        await requestParameterWithPayload(parameterBag, define);
                }
            }
        }
        private async Task requestParameterWithEmptyPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define)
        {
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, CurrentUsedUID, CurrentUsedSubDevice, parameterBag);
            await runPeerToPeerProcess(ptpProcess);
            if (!ptpProcess.ResponsePayloadObject.IsUnset)
            {
                updateParameterValuesDependeciePropertyBag(parameterBag.PID, ptpProcess.ResponsePayloadObject);
                parameterValuesDataTreeBranch.AddOrUpdate(new ParameterDataCacheBag(parameterBag.PID), ptpProcess.ResponsePayloadObject, (o1, o2) => ptpProcess.ResponsePayloadObject);
                object valueToStore = ptpProcess.ResponsePayloadObject.ParsedObject ?? ptpProcess.ResponsePayloadObject;
                this.parameterValues.AddOrUpdate(parameterBag.PID, valueToStore, (o1, o2) => valueToStore);
            }
        }
        private async Task requestParameterWithPayload(ParameterBag parameterBag, MetadataJSONObjectDefine define)
        {
            var req = define.GetRequest.Value.GetRequiredProperties();
            if (req.Length == 1 && req[0] is IIntegerType intType)
            {
                try
                {
                    string name = intType.Name;

                    IComparable dependecyValue = (IComparable)parameterValuesDependeciePropertyBag.FirstOrDefault(bag => bag.Key.Parameter == parameterBag.PID && bag.Key.Command == Metadata.JSON.Command.ECommandDublicte.GetRequest && string.Equals(bag.Key.Name, name)).Value;

                    object i = intType.GetMinimum();
                    object max = intType.GetMaximum();
                    object count = Convert.ChangeType(0, i.GetType());
                    while (dependecyValue.CompareTo(count) > 0)
                    {
                        if (!intType.IsInRange(i))
                            continue;

                        if (((IComparable)max).CompareTo(i) == -1)
                            return;

                        DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree(name, 0, i));
                        PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, CurrentUsedUID, CurrentUsedSubDevice, parameterBag, dataTreeBranch);
                        await runPeerToPeerProcess(ptpProcess);
                        if (!ptpProcess.ResponsePayloadObject.IsUnset)
                        {
                            parameterValuesDataTreeBranch.AddOrUpdate(new ParameterDataCacheBag(parameterBag.PID, i), ptpProcess.ResponsePayloadObject, (o1, o2) => ptpProcess.ResponsePayloadObject);
                        }
                        i = intType.IncrementJumpRange(i);
                        count = intType.Increment(count);
                    }
                }
                catch(Exception e)
                {

                }
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

        public void Dispose()
        {
            this.IsDisposing = true;

            this.parameterValues.Clear();
            this.parameterValues = null;
            this.supportedParameters = null;

            this.IsDisposed = true;
        }

        public RDMSensorDefinition[] GetSensorDefinitions()
        {
            try
            {
                if (!parameterValues.TryGetValue(ERDM_Parameter.SENSOR_DEFINITION, out object value))
                    return Array.Empty<RDMSensorDefinition>();
                else
                {
                    var definitions = value as ConcurrentDictionary<object, object>;
                    return definitions.Values.Cast<RDMSensorDefinition>().ToArray();
                }
            }
            catch
            {

            }
            return Array.Empty<RDMSensorDefinition>();
        }

        public IRDMParameterWrapper GetRDMParameterWrapperByID(ushort parameter)
        {
            if (this.manufacturerParameter.TryGetValue(parameter, out var result))
                return result;
            return null;
        }
        public override string ToString()
        {
            return $"{Enum.GetName(typeof(EManufacturer), Manufacturer)}";
        }
    }
}