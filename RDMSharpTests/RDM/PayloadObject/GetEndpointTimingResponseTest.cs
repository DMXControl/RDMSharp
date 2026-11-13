using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class GetEndpointTimingResponseTest
{
    [Test]
    public void InterfaceTests()
    {
        GetEndpointTimingResponse getEndpointTimingResponse = new GetEndpointTimingResponse(1, 123, 254);

        Assert.Multiple(() =>
        {
            Assert.That(getEndpointTimingResponse.EndpointId, Is.EqualTo(1));
            Assert.That(getEndpointTimingResponse.Index, Is.EqualTo(123));
            Assert.That(getEndpointTimingResponse.TimingId, Is.EqualTo(123));
            Assert.That(getEndpointTimingResponse.Timings, Is.EqualTo(254));
            Assert.That(getEndpointTimingResponse.Count, Is.EqualTo(254));
            Assert.That(getEndpointTimingResponse.DescriptorParameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION));
            Assert.That(getEndpointTimingResponse.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(getEndpointTimingResponse.MinIndex, Is.EqualTo(1));
        });
    }
}
