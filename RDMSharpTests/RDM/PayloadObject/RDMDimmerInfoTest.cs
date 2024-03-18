namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMDimmerInfoTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDimmerInfo dimmerInfo = new RDMDimmerInfo(numberOfSupportedCurves: 4, levelsResolution: 6, minimumLevelSplitLevelsSupported: true);
            byte[] data = dimmerInfo.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DIMMER_INFO,
                ParameterData = data,
            };

            RDMDimmerInfo resultDimmerInfo = RDMDimmerInfo.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMDimmerInfo.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultDimmerInfo, Is.EqualTo(dimmerInfo));

            var res = resultDimmerInfo.ToString();
            var src = dimmerInfo.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMDimmerInfo(numberOfSupportedCurves: 4, levelsResolution: 0, minimumLevelSplitLevelsSupported: true); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMDimmerInfo(numberOfSupportedCurves: 4, levelsResolution: 36, minimumLevelSplitLevelsSupported: true); });
        }
    }
}