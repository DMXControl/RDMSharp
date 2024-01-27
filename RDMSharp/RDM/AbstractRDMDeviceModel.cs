using RDMSharp.ParameterWrapper;
using RDMSharp.ParameterWrapper.Generic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractRDMDeviceModel : IRDMDeviceModel
    {
        private static RDMParameterWrapperCatalogueManager pmManager => RDMParameterWrapperCatalogueManager.GetInstance();
        private static DeviceInfoParameterWrapper deviceInfoParameterWrapper => (DeviceInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEVICE_INFO);
        private static SupportedParametersParameterWrapper supportedParametersParameterWrapper => (SupportedParametersParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SUPPORTED_PARAMETERS);
        private static SensorDefinitionParameterWrapper sensorDefinitionParameterWrapper => (SensorDefinitionParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SENSOR_DEFINITION);

        public ushort ManufacturerID { get; private set; }
        public EManufacturer Manufacturer { get; private set; }
        public RDMUID CurrentUsedUID { get; private set; }

        public event EventHandler Initialized;
        public bool IsInitialized { get; private set; } = false;

        private AsyncRDMRequestHelper asyncRDMRequestHelper;

        private ConcurrentDictionary<ushort, IRDMParameterWrapper> manufacturerParameter = new ConcurrentDictionary<ushort, IRDMParameterWrapper>();



        private ConcurrentDictionary<ERDM_Parameter, object> parameterValues = new ConcurrentDictionary<ERDM_Parameter, object>();
        public IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get { return this.parameterValues?.AsReadOnly(); }
        }

        public RDMDeviceInfo DeviceInfo
        {
            get { return this.parameterValues.ContainsKey(ERDM_Parameter.DEVICE_INFO) ? this.parameterValues[ERDM_Parameter.DEVICE_INFO] as RDMDeviceInfo : null; }
            private set
            {
                if (this.DeviceInfo == value)
                    return;

                this.parameterValues[ERDM_Parameter.DEVICE_INFO] = value;
                PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DeviceInfo)));
            }
        }

        private ConcurrentDictionary<ERDM_Parameter, bool> supportedParameters = new ConcurrentDictionary<ERDM_Parameter, bool>();
        public IReadOnlyCollection<ERDM_Parameter> SupportedParameters
        {
            get { return this.supportedParameters.Where(sp => sp.Value).Select(sp => sp.Key).Where(p => ((ushort)p > 0x000F)).ToArray().AsReadOnly(); }
        }
        public IReadOnlyCollection<ERDM_Parameter> SupportedBlueprintParameters
        {
            get { return this.SupportedParameters.Where(p => pmManager.GetRDMParameterWrapperByID(p) is IRDMBlueprintParameterWrapper).ToArray().AsReadOnly(); }
        }
        public IReadOnlyCollection<ERDM_Parameter> SupportedNonBlueprintParameters
        {
            get { return this.SupportedParameters.Where(p => !(pmManager.GetRDMParameterWrapperByID(p) is IRDMBlueprintParameterWrapper)).ToArray().AsReadOnly(); }
        }

        public IReadOnlyCollection<ERDM_Parameter> KnownNotSupportedParameters
        {
            get { return this.supportedParameters.Where(sp => !sp.Value).Select(sp => sp.Key).ToArray().AsReadOnly(); }
        }


        public bool IsDisposing { get; private set; }
        public bool IsDisposed { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected AbstractRDMDeviceModel(RDMUID uid, RDMDeviceInfo deviceInfo)
        {
            DeviceInfo = deviceInfo;
            this.CurrentUsedUID = uid;
            ManufacturerID = uid.ManufacturerID;
            Manufacturer = (EManufacturer)uid.ManufacturerID;

            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMMessage);
        }
        internal async Task Initialize()
        {
            if (IsInitialized)
                return;

            await processMessage(await requestParameter(supportedParametersParameterWrapper.BuildGetRequestMessage()));

            var sp = this.SupportedParameters.ToArray();
            foreach (ERDM_Parameter parameter in sp)
            {
                var pw = pmManager.GetRDMParameterWrapperByID(parameter);

                switch (pw)
                {
                    case ParameterDescriptionParameterWrapper parameterDescription:
                        foreach (ERDM_Parameter p in this.SupportedParameters.Where(p => !ERDM_Parameter.IsDefined(typeof(ERDM_Parameter), p)))
                            await processMessage(await requestParameter((pw as ParameterDescriptionParameterWrapper).BuildGetRequestMessage(p)));
                        break;
                    case SupportedParametersParameterWrapper supportedParameters:
                        break;
                }

                //if (parameter == ERDM_Parameter.SENSOR_DEFINITION)
                //{
                //    await this.RequestSensorDefinitions(uid);
                //    continue;
                //}

                if (!(pw is IRDMBlueprintParameterWrapper))
                    continue;

                if (pw is IRDMBlueprintDescriptionListParameterWrapper blueprintDL)
                    pw = pmManager.GetRDMParameterWrapperByID(blueprintDL.ValueParameterID);

                RDMMessage request = null;
                if (pw is IRDMGetParameterWrapperWithEmptyGetRequest emptyGet)
                    request = emptyGet.BuildGetRequestMessage();

                if (request == null)
                    continue;


                await processMessage(await requestParameter(request));
            }
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }


        private void sendRDMMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = CurrentUsedUID;
            _ = SendRDMMessage(rdmMessage);
        }

        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);

        protected async void ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage.SourceUID != CurrentUsedUID)
                return;

            if (!rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            if (asyncRDMRequestHelper.ReceiveMethode(rdmMessage))
                return;

            await processMessage(rdmMessage);
        }

        private async Task<RDMMessage> requestParameter(RDMMessage rdmMessage)
        {
            return await asyncRDMRequestHelper.RequestParameter(rdmMessage);
        }

        private async Task processMessage(RDMMessage rdmMessage)
        {
            var pw = pmManager.GetRDMParameterWrapperByID(rdmMessage.Parameter);
            switch (pw)
            {
                case SupportedParametersParameterWrapper _supportedParameters:
                    if (!(rdmMessage.Value is ERDM_Parameter[] parameters))
                        break;

                    foreach (var para in parameters)
                    {
                        if (!this.supportedParameters.TryGetValue(para, out _))
                            supportedParameters.TryAdd(para, true);
                    }
                    break;

                case ParameterDescriptionParameterWrapper _parameterDescription when rdmMessage.Value is RDMParameterDescription pd:
                    if (manufacturerParameter.ContainsKey(pd.ParameterId))
                        break;
                    switch (pd.DataType)
                    {
                        case ERDM_DataType.ASCII:
                            manufacturerParameter.TryAdd(pd.ParameterId, new ASCIIParameterWrapper(pd));
                            break;
                        case ERDM_DataType.SIGNED_BYTE:
                            manufacturerParameter.TryAdd(pd.ParameterId, new SignedByteParameterWrapper(pd));
                            break;
                        case ERDM_DataType.UNSIGNED_BYTE:
                            manufacturerParameter.TryAdd(pd.ParameterId, new UnsignedByteParameterWrapper(pd));
                            break;
                        case ERDM_DataType.SIGNED_WORD:
                            manufacturerParameter.TryAdd(pd.ParameterId, new SignedWordParameterWrapper(pd));
                            break;
                        case ERDM_DataType.UNSIGNED_WORD:
                            manufacturerParameter.TryAdd(pd.ParameterId, new UnsignedWordParameterWrapper(pd));
                            break;
                        case ERDM_DataType.SIGNED_DWORD:
                            manufacturerParameter.TryAdd(pd.ParameterId, new SignedDWordParameterWrapper(pd));
                            break;
                        case ERDM_DataType.UNSIGNED_DWORD:
                            manufacturerParameter.TryAdd(pd.ParameterId, new UnsignedDWordParameterWrapper(pd));
                            break;

                        case ERDM_DataType.BIT_FIELD:
                        default:
                            manufacturerParameter.TryAdd(pd.ParameterId, new NotDefinedParameterWrapper(pd));
                            break;

                    }
                    break;

                case IRDMBlueprintDescriptionListParameterWrapper blueprintDL:
                    HashSet<object> list = null;
                    if (parameterValues.ContainsKey(rdmMessage.Parameter))
                        list = parameterValues[rdmMessage.Parameter] as HashSet<object>;

                    if (list == null)
                        parameterValues[rdmMessage.Parameter] = list = new HashSet<object>();

                    if (list == null)
                        return;

                    if (rdmMessage.Value == null)
                        return;

                    list.Add(rdmMessage.Value);

                    break;
                case IRDMBlueprintParameterWrapper blueprint:
                    parameterValues[rdmMessage.Parameter] = rdmMessage.Value;
                    break;
                default:
                    if (rdmMessage.Value is IRDMPayloadObjectOneOf oneOf)
                    {
                        var bdl = pmManager.ParameterWrappers.OfType<IRDMBlueprintDescriptionListParameterWrapper>().FirstOrDefault(p => p.ValueParameterID == rdmMessage.Parameter);
                        switch (bdl)
                        {
                            case IRDMGetParameterWrapperRequest<byte> @byte:
                                for (byte b = (byte)oneOf.MinIndex; b < (byte)oneOf.Count + (byte)oneOf.MinIndex; b++)
                                    await processMessage(await requestParameter(@byte.BuildGetRequestMessage(b)));
                                break;
                            case IRDMGetParameterWrapperRequest<ushort> @ushort:
                                for (ushort u = (ushort)oneOf.MinIndex; u < (ushort)oneOf.Count + (ushort)oneOf.MinIndex; u++)
                                    await processMessage(await requestParameter(@ushort.BuildGetRequestMessage(u)));
                                break;
                        }
                    }
                    break;
            }
        }

        public bool IsModelOf(RDMUID uid, RDMDeviceInfo other)
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
                if (!this.parameterValues.ContainsKey(ERDM_Parameter.SENSOR_DEFINITION))
                    return new RDMSensorDefinition[0];
                else
                {
                    var definitions = this.parameterValues[ERDM_Parameter.SENSOR_DEFINITION] as HashSet<object>;
                    return definitions.Cast<RDMSensorDefinition>().ToArray();
                }
            }
            catch
            {

            }
            return new RDMSensorDefinition[0];
        }

        public IRDMParameterWrapper GetRDMParameterWrapperByID(ushort parameter)
        {
            if (this.manufacturerParameter.TryGetValue(parameter, out var result))
                return result;
            return null;
        }
        public override string ToString()
        {
            return $"{Enum.GetName(typeof(EManufacturer),Manufacturer)}";
        }
    }
}