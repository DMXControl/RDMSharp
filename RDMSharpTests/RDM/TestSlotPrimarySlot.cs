using RDMSharp.Metadata;
using RDMSharp.PayloadObject;

namespace RDMSharpTests.RDM;

/// <summary>
/// E1.20 SLOT_INFO: the Slot Label ID of a secondary slot (fine, timing, ...) is the offset of its primary slot, not a category.
/// </summary>
public class TestSlotPrimarySlot
{
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [Test]
    public void TestPrimarySlotOfSecondarySlots()
    {
        // pan coarse on slot 3, its fine on slot 4: parsed from SLOT_INFO data
        var data = PayloadTestTools.GetResponseData(ERDM_Parameter.SLOT_INFO, new[]
        {
            new RDMSlotInfo(3, ERDM_SlotType.PRIMARY, ERDM_SlotCategory.PAN),
            new RDMSlotInfo(4, ERDM_SlotType.SEC_FINE, (ERDM_SlotCategory)3),
        });
        var define = MetadataFactory.GetDefine(new ParameterBag(ERDM_Parameter.SLOT_INFO));
        var slotInfos = (RDMSlotInfo[])MetadataFactory.ParseDataToPayload(define, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse, data).ParsedObject;

        var pan = new Slot(slotInfos[0].SlotOffset, slotInfos[0].SlotLabelId, slotInfos[0].SlotType);
        var fine = new Slot(slotInfos[1].SlotOffset, slotInfos[1].SlotLabelId, slotInfos[1].SlotType);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(pan.PrimarySlotId, Is.Null);
            Assert.That(pan.Category, Is.EqualTo(ERDM_SlotCategory.PAN));
            Assert.That(fine.PrimarySlotId, Is.EqualTo((ushort)3));
        }
    }
}
