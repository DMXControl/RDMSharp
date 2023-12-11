using RDMSharp;

namespace RDMSharpTest.RDM
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

            Assert.That(dimmerInfo, Is.EqualTo(resultDimmerInfo));
        }
    }
}