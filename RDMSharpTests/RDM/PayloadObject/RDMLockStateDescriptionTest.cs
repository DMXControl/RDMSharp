using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM.PayloadObject;

public class RDMLockStateDescriptionTest
{
    [Test]
    public void DescriptionCharLimitTest()
    {
        RDMLockStateDescription resultLockStateDescription = new RDMLockStateDescription(description: "Pseudo LockState 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
        Assert.That(resultLockStateDescription.Description, Has.Length.EqualTo(32));

        resultLockStateDescription = new RDMLockStateDescription(7, description: "");
        Assert.Multiple(() =>
        {
            Assert.That(string.IsNullOrEmpty(resultLockStateDescription.Description), Is.True);
            Assert.That(resultLockStateDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultLockStateDescription.Index, Is.EqualTo(7));
        });
    }
}