
namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDiscoveryTool : AbstractDiscoveryTool
    {
        public MockDiscoveryTool() : base()
        {
            SendReceivePipelineImitateRealConditions.RDMMessageRereivedResponse += SendReceivePipelineImitateRealConditions_RDMMessageRereivedResponse;
        }

        private void SendReceivePipelineImitateRealConditions_RDMMessageRereivedResponse(object? sender, byte[] e)
        {
            try
            {
                this.ReceiveRDMMessage(new RDMMessage(e));
            }
            catch
            {
            }
        }

        protected override async Task<bool> SendRDMMessage(RDMMessage rdmMessage)
        {
            await SendReceivePipelineImitateRealConditions.RDMMessageSend(rdmMessage);
            return true;
        }
    }
}
