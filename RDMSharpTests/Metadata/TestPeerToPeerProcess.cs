using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata
{
    public class TestPeerToPeerProcess
    {

        [Test]
        public async Task Test_Get_DMX_START_ADDRESS()
        {
            var command = ERDM_Command.GET_COMMAND;
            var uid = new UID(1234, 123456);
            var subdevice = SubDevice.Root;
            var parameterBag = new ParameterBag(ERDM_Parameter.DMX_START_ADDRESS);
            const ushort DMX_ADDRESS = 33;
            PeerToPeerProcess peerToPeerProcess = new PeerToPeerProcess(command, uid, subdevice, parameterBag);

            Assert.That(peerToPeerProcess.Define, Is.Not.Null);
            Assert.That(peerToPeerProcess.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Waiting));
            Assert.That(peerToPeerProcess.RequestPayloadObject, Is.Null);
            Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.Null);

            AsyncRDMRequestHelper helper = null;
            helper = new AsyncRDMRequestHelper(sendMessage);
            peerToPeerProcess.Run(helper);

            while (peerToPeerProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Running)
                await Task.Delay(100);

            Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTree[])));
            Assert.That(((DataTree[])peerToPeerProcess.ResponsePayloadObject)[0].Value, Is.EqualTo(DMX_ADDRESS));

            async Task sendMessage(RDMMessage message)
            {
                Assert.That(message.Command, Is.EqualTo(command));
                Assert.That(message.DestUID, Is.EqualTo(uid));
                Assert.That(message.SubDevice, Is.EqualTo(subdevice));
                Assert.That(message.Parameter, Is.EqualTo(parameterBag.PID));

                RDMMessage response = new RDMMessage()
                {
                    Command= message.Command | ERDM_Command.RESPONSE,
                    DestUID = message.SourceUID,
                    SourceUID = message.DestUID,
                    Parameter= message.Parameter,
                    SubDevice= message.SubDevice,
                    ParameterData= MetadataFactory.GetResponseMessageData(parameterBag, new DataTree[] { new DataTree("dmx_address", 0, DMX_ADDRESS) })
                };

                await Task.Delay(10);
                helper.ReceiveMessage(response);
            }
        }
    }
}