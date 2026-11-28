using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class GetSetEndpointLabelTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        GetSetEndpointLabel resultGetSetEndpointLabel = new GetSetEndpointLabel(endpointLabel: "Pseudo Endpoint Label 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultGetSetEndpointLabel.EndpointLabel, Has.Length.EqualTo(32));
        resultGetSetEndpointLabel = new GetSetEndpointLabel(endpointLabel: "");
        Assert.That(string.IsNullOrWhiteSpace(resultGetSetEndpointLabel.EndpointLabel), Is.True);
    }
}