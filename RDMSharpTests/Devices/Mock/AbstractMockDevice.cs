using System.Collections.Concurrent;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockDevice : AbstractRDMDevice
    {
        private ConcurrentDictionary<long, RDMMessage> identifyer = new ConcurrentDictionary<long, RDMMessage>();
        private bool eventRegistered = false;
        private byte transactionCounter = 0;
        public AbstractMockDevice(RDMUID uid) : base(uid)
        {
            registerEvent();
        }
        ~AbstractMockDevice()
        {
            SendReceivePipeline.RDMMessageRereived -= SendReceivePipeline_RDMMessageRereived;
        }
        private void registerEvent()
        {
            if (eventRegistered)
                return;
            eventRegistered = true;

            SendReceivePipeline.RDMMessageRereived -= SendReceivePipeline_RDMMessageRereived;
            SendReceivePipeline.RDMMessageRereived += SendReceivePipeline_RDMMessageRereived;
        }
        private async void SendReceivePipeline_RDMMessageRereived(object? sender, Tuple<long, RDMMessage> tuple)
        {
            if (identifyer.TryGetValue(tuple.Item1, out RDMMessage rdmMessage))
            {
                if (rdmMessage.Equals(tuple.Item2))
                {
                    identifyer.Remove(tuple.Item1, out _);
                    return;
                }
            }

            await base.ReceiveRDMMessage(tuple.Item2);
        }
        protected override async Task SendRDMMessage(RDMMessage rdmMessage)
        {
            registerEvent();
            rdmMessage.TransactionCounter = getTransactionCounter();
            var i = SendReceivePipeline.GetNewIdentifyer();
            identifyer.TryAdd(i, rdmMessage);
            SendReceivePipeline.RDMMessageSend(i, rdmMessage);
        }
        private byte getTransactionCounter()
        {
            transactionCounter++;
            return transactionCounter;
        }
    }
}
