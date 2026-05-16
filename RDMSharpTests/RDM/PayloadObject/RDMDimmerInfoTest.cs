using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMDimmerInfoTest
{
    [Test]
    public void ExceptionTests()
    {
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMDimmerInfo(numberOfSupportedCurves: 4, levelsResolution: 0, minimumLevelSplitLevelsSupported: true); });
        Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMDimmerInfo(numberOfSupportedCurves: 4, levelsResolution: 36, minimumLevelSplitLevelsSupported: true); });
    }
}