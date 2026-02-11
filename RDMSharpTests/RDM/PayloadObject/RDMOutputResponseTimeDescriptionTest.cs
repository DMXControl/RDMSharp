using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMOutputResponseTimeDescriptionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMOutputResponseTimeDescription resultOutputResponseTimeDescription = new RDMOutputResponseTimeDescription(description: "Pseudo OutputResponseTime 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultOutputResponseTimeDescription.Description, Has.Length.EqualTo(32));

        resultOutputResponseTimeDescription = new RDMOutputResponseTimeDescription(3, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrWhiteSpace(resultOutputResponseTimeDescription.Description), Is.True);
            Assert.That(resultOutputResponseTimeDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultOutputResponseTimeDescription.Index, Is.EqualTo(3));
        });
    }
}