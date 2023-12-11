namespace RDMSharpTest.RDM
{
    public class GetInterfaceListResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetInterfaceListResponse getInterfaceListResponse = new GetInterfaceListResponse(
                new InterfaceDescriptor(),
                new InterfaceDescriptor(1, 1),
                new InterfaceDescriptor(1, 2),
                new InterfaceDescriptor(1, 3),
                new InterfaceDescriptor(2, 1),
                new InterfaceDescriptor(2, 2),
                new InterfaceDescriptor(2, 3),
                new InterfaceDescriptor(3, 1),
                new InterfaceDescriptor(3, 2),
                new InterfaceDescriptor(3, 3));

            byte[] data = getInterfaceListResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.LIST_INTERFACES,
                ParameterData = data,
            };

            GetInterfaceListResponse resultGetInterfaceListResponse = GetInterfaceListResponse.FromMessage(message);

            Assert.That(resultGetInterfaceListResponse, Is.EqualTo(getInterfaceListResponse));
        }
    }
}