using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class GetBackgroundQueuedStatusPolicyResponseTest
{
    [Test]
    public void InterfaceTests()
    {
        GetBackgroundQueuedStatusPolicyResponse getBackgroundQueuedStatusPolicyResponse = new GetBackgroundQueuedStatusPolicyResponse(9, 123);

        Assert.Multiple(() =>
        {
            Assert.That(getBackgroundQueuedStatusPolicyResponse.Count, Is.EqualTo(123));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.Policies, Is.EqualTo(123));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.Index, Is.EqualTo(9));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.PolicyId, Is.EqualTo(9));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.MinIndex, Is.EqualTo(1));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(getBackgroundQueuedStatusPolicyResponse.DescriptorParameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION));
        });
    }
}