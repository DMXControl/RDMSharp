
namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDiscoveryTool : AbstractDiscoveryTool
    {
        public MockDiscoveryTool() : base()
        {
            SendReceivePipelineImitateRealConditions.RDMMessageReceivedResponse += SendReceivePipelineImitateRealConditions_RDMMessageReceivedResponse;
        }

        private void SendReceivePipelineImitateRealConditions_RDMMessageReceivedResponse(object? sender, byte[] e)
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
