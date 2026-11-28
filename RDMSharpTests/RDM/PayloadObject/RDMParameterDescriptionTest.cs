using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMParameterDescriptionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMParameterDescription parameterDescription = new RDMParameterDescription(2, description: "Pseudo Parameter 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(parameterDescription.Description, Has.Length.EqualTo(32));

        parameterDescription = new RDMParameterDescription(4, description: "");
        Assert.That(string.IsNullOrEmpty(parameterDescription.Description), Is.True);
    }

    [Test]
    public void SecoundConstructor()
    {
        RDMParameterDescription parameterDescription = new RDMParameterDescription((ushort)ERDM_Parameter.MINIMUM_LEVEL, 2, ERDM_DataType.UINT32, ERDM_CommandClass.GET, 1, ERDM_SensorUnit.CANDELA, ERDM_UnitPrefix.TERRA, 2, 4, 5, "dadsad");
        Assert.That(parameterDescription.Description, Has.Length.EqualTo(6));
        Assert.That(parameterDescription.ParameterId, Is.EqualTo((ushort)ERDM_Parameter.MINIMUM_LEVEL));
        Assert.That(parameterDescription.DataType, Is.EqualTo(ERDM_DataType.UINT32));
        Assert.That(parameterDescription.CommandClass, Is.EqualTo(ERDM_CommandClass.GET));
    }
}