using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMCurveDescriptionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMCurveDescription resultCurveDescription = new RDMCurveDescription(description: "Pseudo Curve 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultCurveDescription.Description, Has.Length.EqualTo(32));

        resultCurveDescription = new RDMCurveDescription(6, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrEmpty(resultCurveDescription.Description), Is.True);
            Assert.That(resultCurveDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultCurveDescription.Index, Is.EqualTo(6));
        });
    }
}