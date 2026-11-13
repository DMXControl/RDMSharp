using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class GetBackgroundQueuedStatusPolicyDescriptionResponseTest
{
    [Test]
    public void InterfaceTests()
    {
        GetBackgroundQueuedStatusPolicyDescriptionResponse getBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(1, "Pseudo Background Queued/Status Message Policy Description");

        Assert.Multiple(() =>
        {
            Assert.That(getBackgroundQueuedStatusPolicyDescriptionResponse.Index, Is.EqualTo(1));
            Assert.That(getBackgroundQueuedStatusPolicyDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(getBackgroundQueuedStatusPolicyDescriptionResponse.Description, Is.EqualTo("Pseudo Background Queued/Status "));
        });
    }
    [Test]
    public void DescriptionCharLimitTest()
    {
        GetBackgroundQueuedStatusPolicyDescriptionResponse resultGetBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(description: "Pseudo Background Queued/Status Message Policy Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Description, Has.Length.EqualTo(32));

        resultGetBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(5, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrEmpty(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Description), Is.True);
            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Index, Is.EqualTo(5));
        });
    }
}