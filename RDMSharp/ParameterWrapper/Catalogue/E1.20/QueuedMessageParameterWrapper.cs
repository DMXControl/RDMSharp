namespace RDMSharp.ParameterWrapper
{
    public sealed class QueuedMessageParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequestResponse
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public QueuedMessageParameterWrapper() : base(ERDM_Parameter.QUEUED_MESSAGE)
        {
        }

        public override string Name => "Queued Message";
        public override string Description => "The Queued Message parameter shall be used to retrieve a message from the responder’s message queue. The Message Count field of all response messages defines the number of messages that are queued in the responder. Each Queued Message response shall be composed of a single message response.";
    }
}