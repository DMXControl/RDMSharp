using System.Collections.Concurrent;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockGeneratedDevice : AbstractGeneratedRDMDevice
    {
        private readonly ConcurrentDictionary<long, RDMMessage> identifyer = new ConcurrentDictionary<long, RDMMessage>();
        private bool eventRegistered = false;
        private bool imitateRealConditions = false;
        internal bool ImitateRealConditions
        {
            get { return imitateRealConditions; }
            set
            {
                imitateRealConditions = value;
                eventRegistered = false;
                registerEvent();
            }
        }


        public new string SoftwareVersionLabel
        {
            get
            {
                return base.SoftwareVersionLabel;
            }
            internal set
            {
                base.SoftwareVersionLabel = value;
            }
        }


        public new string BootSoftwareVersionLabel
        {
            get
            {
                return base.BootSoftwareVersionLabel;
            }
            internal set
            {
                base.BootSoftwareVersionLabel = value;
            }
        }
        public AbstractMockGeneratedDevice(UID uid, ERDM_Parameter[] parameters, string manufacturer, Sensor[]? sensors = null, IRDMDevice[]? subDevices = null) : base(uid, parameters, manufacturer, sensors: sensors, subDevices: subDevices)
        {
            registerEvent();
        }
        public AbstractMockGeneratedDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, string manufacturer, Sensor[]? sensors = null) : base(uid, subDevice, parameters, manufacturer, sensors: sensors)
        {
            registerEvent();
        }
        private void registerEvent()
        {
            if (eventRegistered)
                return;
            eventRegistered = true;

            SendReceivePipeline.RDMMessageReceived -= SendReceivePipeline_RDMMessageReceived;
            SendReceivePipelineImitateRealConditions.RDMMessageReceivedRequest -= SendReceivePipelineImitateRealConditions_RDMMessageReceivedRequest;

            if (ImitateRealConditions)
                SendReceivePipelineImitateRealConditions.RDMMessageReceivedRequest += SendReceivePipelineImitateRealConditions_RDMMessageReceivedRequest;
            else
                SendReceivePipeline.RDMMessageReceived += SendReceivePipeline_RDMMessageReceived;
        }

        private async void SendReceivePipeline_RDMMessageReceived(object? sender, Tuple<long, RDMMessage> tuple)
        {
            if (identifyer.TryGetValue(tuple.Item1, out var rdmMessage) && RDMMessage.Equals(rdmMessage, tuple.Item2))
            {
                identifyer.Remove(tuple.Item1, out _);
                return;
            }

            await base.ReceiveRDMMessage(tuple.Item2);
        }
        private async void SendReceivePipelineImitateRealConditions_RDMMessageReceivedRequest(object? sender, RDMMessage rdmMessage)
        {
            await base.ReceiveRDMMessage(rdmMessage);
        }
        internal RDMMessage? ProcessRequestMessage_Internal(RDMMessage request)
        {
            return base.processRequestMessage(request);
        }
        protected override async Task SendRDMMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage == null)
            {
                await Task.CompletedTask;
                return;
            }
            registerEvent();
            
            var i = SendReceivePipeline.GetNewIdentifyer();
            identifyer.TryAdd(i, rdmMessage);
            if (ImitateRealConditions)
                _ = SendReceivePipelineImitateRealConditions.RDMMessageSend(rdmMessage); // in this case as responder we don't wait for comlpletion of the send, we just send it and forget it
            else
                SendReceivePipeline.RDMMessageSend(i, rdmMessage);
        }

        internal new void AddStatusMessage(RDMStatusMessage statusMessage)
        {
            base.AddStatusMessage(statusMessage);
        }
        internal new void ClearStatusMessage(RDMStatusMessage statusMessage)
        {
            base.ClearStatusMessage(statusMessage);
        }
        internal new void RemoveStatusMessage(RDMStatusMessage statusMessage)
        {
            base.RemoveStatusMessage(statusMessage);
        }
        internal new void AddSensors(params Sensor[] sensor)
        {
            base.AddSensors(sensor);
        }
        internal new void RemoveSensors(params Sensor[] sensor)
        {
            base.RemoveSensors(sensor);
        }

        protected sealed override void onDispose()
        {
            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }
            SendReceivePipeline.RDMMessageReceived -= SendReceivePipeline_RDMMessageReceived;
            SendReceivePipelineImitateRealConditions.RDMMessageReceivedRequest -= SendReceivePipelineImitateRealConditions_RDMMessageReceivedRequest;
            eventRegistered = false;
            identifyer.Clear();
            ImitateRealConditions = false;
        }
#pragma warning disable CS0114
        protected abstract void OnDispose();
#pragma warning restore CS0114 
    }
}
