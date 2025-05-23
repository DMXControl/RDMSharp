﻿using System.Collections.Concurrent;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockDevice : AbstractRemoteRDMDevice
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
        public AbstractMockDevice(UID uid, SubDevice? subDevice = null, bool _imitateRealConditions = false) : base(uid, subDevice)
        {
            ImitateRealConditions = _imitateRealConditions;
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
            if (identifyer.TryGetValue(tuple.Item1, out var rdmMessage))
            {
                if (RDMMessage.Equals(rdmMessage, tuple.Item2))
                {
                    identifyer.Remove(tuple.Item1, out _);
                    return;
                }
            }

            await base.ReceiveRDMMessage(tuple.Item2);
        }

        private async void SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest(object? sender, RDMMessage rdmMessage)
        {
            await base.ReceiveRDMMessage(rdmMessage);
        }
        protected override async Task SendRDMMessage(RDMMessage rdmMessage)
        {
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
        protected sealed override void onDispose()
        {
            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            SendReceivePipeline.RDMMessageRereived -= SendReceivePipeline_RDMMessageRereived;
            SendReceivePipelineImitateRealConditions.RDMMessageRereivedRequest -= SendReceivePipelineImitateRealConditions_RDMMessageRereivedRequest;
            eventRegistered = false;
            transactionCounter = 0;
            identifyer.Clear();
            ImitateRealConditions = false;
        }
        protected abstract void OnDispose();
    }

    internal abstract class AbstractMockSubDevice : AbstractMockDevice, IRDMRemoteSubDevice
    {
        protected AbstractMockSubDevice(UID uid, SubDevice subDevice, bool _imitateRealConditions = false) : base(uid, subDevice, _imitateRealConditions)
        {
        }
        protected sealed override AbstractMockSubDevice createSubDevice(UID uid, SubDevice subDevice)
        {
            throw new NotSupportedException();
        }
    }
}