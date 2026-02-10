using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class GetEndpointTimingDescriptionResponseTest
{
    [Test]
    public void InterfaceTests()
    {
        GetEndpointTimingDescriptionResponse getGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(1, "Pseudo Endpoint Timing Description");

        Assert.Multiple(() =>
        {
            Assert.That(getGetEndpointTimingDescriptionResponse.Index, Is.EqualTo(1));
            Assert.That(getGetEndpointTimingDescriptionResponse.TimingId, Is.EqualTo(1));
            Assert.That(getGetEndpointTimingDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(getGetEndpointTimingDescriptionResponse.Description, Is.EqualTo("Pseudo Endpoint Timing Descripti"));
        });
    }
    [Test]
    public void DescriptionCharLimitTest()
    {
        GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(description: "Pseudo Endpoint Timing Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.Multiple(() =>
        {
            Assert.That(resultGetEndpointTimingDescriptionResponse.Description, Has.Length.EqualTo(32));
            Assert.That(resultGetEndpointTimingDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(resultGetEndpointTimingDescriptionResponse.Index, Is.EqualTo(1));
        });

        resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(10, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrWhiteSpace(resultGetEndpointTimingDescriptionResponse.Description), Is.True);
            Assert.That(resultGetEndpointTimingDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(resultGetEndpointTimingDescriptionResponse.Index, Is.EqualTo(10));
        });
    }
}