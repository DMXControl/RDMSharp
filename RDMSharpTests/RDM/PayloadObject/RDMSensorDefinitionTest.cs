using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMSensorDefinitionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMSensorDefinition sensorDefinition = new RDMSensorDefinition(description: "Pseudo Sensor 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(sensorDefinition.Description, Has.Length.EqualTo(32));

        sensorDefinition = new RDMSensorDefinition(3, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrEmpty(sensorDefinition.Description), Is.True);
            Assert.That(sensorDefinition.MinIndex, Is.EqualTo(0));
            Assert.That(sensorDefinition.Index, Is.EqualTo(3));
        });
    }
}