using System.Collections.Concurrent;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockGeneratedDevice : AbstractGeneratedRDMDevice
    {
        private readonly ConcurrentDictionary<long, RDMMessage> identifyer = new ConcurrentDictionary<long, RDMMessage>();
        private bool eventRegistered = false;
        private byte transactionCounter = 0;
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
        public AbstractMockGeneratedDevice(UID uid, ERDM_Parameter[] parameters, string manufacturer) : base(uid, parameters, manufacturer)
        {
            registerEvent();
        }
        private void registerEvent()
        {
            if (eventRegistered)
                return;
            eventRegistered = true;

            SendReceivePipeline.RDMMessageRereived -= SendReceivePipeline_RDMMessageRereived;
            SendReceivePipelineImitateRealConditions.RDMMessageRereivedRequest -= SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest;

            if (ImitateRealConditions)
                SendReceivePipelineImitateRealConditions.RDMMessageRereivedRequest += SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest;
            else
                SendReceivePipeline.RDMMessageRereived += SendReceivePipeline_RDMMessageRereived;
        }

        private async void SendReceivePipeline_RDMMessageRereived(object? sender, Tuple<long, RDMMessage> tuple)
        {
            if (identifyer.TryGetValue(tuple.Item1, out var rdmMessage) && RDMMessage.Equals(rdmMessage, tuple.Item2))
            {
                identifyer.Remove(tuple.Item1, out _);
                return;
            }

            await base.ReceiveRDMMessage(tuple.Item2);
        }
        private async void SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest(object? sender, RDMMessage rdmMessage)
        {
            await base.ReceiveRDMMessage(rdmMessage);
        }
        protected override async Task SendRDMMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage == null)
                return;
            registerEvent();
            rdmMessage.TransactionCounter = getTransactionCounter();
            var i = SendReceivePipeline.GetNewIdentifyer();
            identifyer.TryAdd(i, rdmMessage);
            if (ImitateRealConditions)
                await SendReceivePipelineImitateRealConditions.RDMMessageSend(rdmMessage);
            else
                SendReceivePipeline.RDMMessageSend(i, rdmMessage);
        }
        private byte getTransactionCounter()
        {
            transactionCounter++;
            return transactionCounter;
        }
        protected override void OnDispose()
        {
            SendReceivePipeline.RDMMessageRereived -= SendReceivePipeline_RDMMessageRereived;
            SendReceivePipelineImitateRealConditions.RDMMessageRereivedRequest -= SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest;
            eventRegistered = false;
            transactionCounter = 0;
            identifyer.Clear();
            ImitateRealConditions = false;
            base.OnDispose();
        }
    }
}
