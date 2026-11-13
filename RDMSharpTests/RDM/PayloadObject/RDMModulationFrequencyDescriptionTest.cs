using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMModulationFrequencyDescriptionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMModulationFrequencyDescription resultModulationFrequencyDescription = new RDMModulationFrequencyDescription(description: "Pseudo ModulationFrequency 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultModulationFrequencyDescription.Description, Has.Length.EqualTo(32));

        resultModulationFrequencyDescription = new RDMModulationFrequencyDescription(5, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrEmpty(resultModulationFrequencyDescription.Description), Is.True);
            Assert.That(resultModulationFrequencyDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultModulationFrequencyDescription.Index, Is.EqualTo(5));
        });
    }
}