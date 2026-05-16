using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class SetEndpointTimingRequestTest
{
    [Test]
    public void InterfaceTests()
    {
        SetEndpointTimingRequest setEndpointTimingRequest = new SetEndpointTimingRequest(1, 123);

        Assert.Multiple(() =>
        {
            Assert.That(setEndpointTimingRequest.EndpointId, Is.EqualTo(1));
            Assert.That(setEndpointTimingRequest.TimingId, Is.EqualTo(123));
        });
    }
}