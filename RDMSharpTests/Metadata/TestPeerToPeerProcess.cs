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
            Assert.That(peerToPeerProcess.RequestPayloadObject.IsUnset, Is.True);
            Assert.That(peerToPeerProcess.ResponsePayloadObject.IsUnset, Is.True);

            AsyncRDMRequestHelper helper = null;
            helper = new AsyncRDMRequestHelper(sendMessage);

            await Task.WhenAny(
                peerToPeerProcess.Run(helper),
                Task.Run(async () =>
                {
                    while (peerToPeerProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Running)
                        await Task.Delay(100);
                }));

            Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Value, Is.EqualTo(DMX_ADDRESS));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(DMX_ADDRESS));

            async Task sendMessage(RDMMessage message)
            {
                Assert.That(message.Command, Is.EqualTo(command));
                Assert.That(message.DestUID, Is.EqualTo(uid));
                Assert.That(message.SubDevice, Is.EqualTo(subdevice));
                Assert.That(message.Parameter, Is.EqualTo(parameterBag.PID));

                RDMMessage response = new RDMMessage()
                {
                    Command = message.Command | ERDM_Command.RESPONSE,
                    DestUID = message.SourceUID,
                    SourceUID = message.DestUID,
                    Parameter = message.Parameter,
                    SubDevice = message.SubDevice,
                    ParameterData = MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("dmx_address", 0, DMX_ADDRESS) }))
                };

                await Task.Delay(10);
                helper.ReceiveMessage(response);
            }
        }


        [Test]
        public async Task Test_Get_PROXIED_DEVICES()
        {
            var command = ERDM_Command.GET_COMMAND;
            var uid = new UID(1234, 123456);
            var subdevice = SubDevice.Root;
            var parameterBag = new ParameterBag(ERDM_Parameter.PROXIED_DEVICES);
            DataTree[] children = new DataTree[500];
            Random rnd = new Random();
            for (int i = 0; i < children.Length; i++)
                children[i] = new DataTree("device_uid", (uint)i, new UID(0x1234, (uint)rnd.Next(1, int.MaxValue)));

            PeerToPeerProcess peerToPeerProcess = new PeerToPeerProcess(command, uid, subdevice, parameterBag);

            Assert.That(peerToPeerProcess.Define, Is.Not.Null);
            Assert.That(peerToPeerProcess.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Waiting));
            Assert.That(peerToPeerProcess.RequestPayloadObject.IsUnset, Is.True);
            Assert.That(peerToPeerProcess.ResponsePayloadObject.IsUnset, Is.True);

            AsyncRDMRequestHelper helper = null;
            byte[] parameterData = MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("device_uids", 0, children: children) }));
            helper = new AsyncRDMRequestHelper(sendMessage);
            
            await Task.WhenAny(
                peerToPeerProcess.Run(helper),
                Task.Run(async () =>
                {
                    while (peerToPeerProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Running)
                        await Task.Delay(100);
                }));

            Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Children, Is.EqualTo(children));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(children.Select(dt => dt.Value).ToList()));


            async Task sendMessage(RDMMessage message)
            {
                Assert.That(message.Command, Is.EqualTo(command));
                Assert.That(message.DestUID, Is.EqualTo(uid));
                Assert.That(message.SubDevice, Is.EqualTo(subdevice));
                Assert.That(message.Parameter, Is.EqualTo(parameterBag.PID));

                var count = Math.Min(parameterData.Length, 0xE4);
                var _parameterData = parameterData.Take(count).ToArray();
                parameterData= parameterData.Skip(count).ToArray();
                RDMMessage response = new RDMMessage()
                {
                    Command = message.Command | ERDM_Command.RESPONSE,
                    DestUID = message.SourceUID,
                    SourceUID = message.DestUID,
                    Parameter = message.Parameter,
                    SubDevice = message.SubDevice,
                    ParameterData = _parameterData,
                    PortID_or_Responsetype = parameterData.Length == 0 ? (byte)ERDM_ResponseType.ACK : (byte)ERDM_ResponseType.ACK_OVERFLOW
                };

                await Task.Delay(10);
                helper.ReceiveMessage(response);
            }
        }
        [Test]
        public async Task Test_Get_LAMP_STRIKES()
        {
            var command = ERDM_Command.GET_COMMAND;
            var uid = new UID(1234, 123456);
            var subdevice = SubDevice.Root;
            var parameterBag = new ParameterBag(ERDM_Parameter.LAMP_STRIKES);
            const uint LAMP_STRIKES = 33;
            PeerToPeerProcess peerToPeerProcess = new PeerToPeerProcess(command, uid, subdevice, parameterBag);

            Assert.That(peerToPeerProcess.Define, Is.Not.Null);
            Assert.That(peerToPeerProcess.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Waiting));
            Assert.That(peerToPeerProcess.RequestPayloadObject.IsUnset, Is.True);
            Assert.That(peerToPeerProcess.ResponsePayloadObject.IsUnset, Is.True);

            AsyncRDMRequestHelper helper = null;
            byte count = 0;
            helper = new AsyncRDMRequestHelper(sendMessage);

            await Task.WhenAny(
                peerToPeerProcess.Run(helper),
                Task.Run(async () =>
                {
                    while (peerToPeerProcess.State == PeerToPeerProcess.EPeerToPeerProcessState.Running)
                        await Task.Delay(100);
                }));

            Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Value, Is.EqualTo(LAMP_STRIKES));
            Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(LAMP_STRIKES));

            async Task sendMessage(RDMMessage message)
            {
                Assert.That(count, Is.LessThan(2));
                if(count==0)
                    Assert.That(message.Parameter, Is.EqualTo(parameterBag.PID));
                else if(count==1)
                    Assert.That(message.Parameter, Is.EqualTo(ERDM_Parameter.QUEUED_MESSAGE));
                Assert.That(message.Command, Is.EqualTo(command));
                Assert.That(message.DestUID, Is.EqualTo(uid));
                Assert.That(message.SubDevice, Is.EqualTo(subdevice));

                RDMMessage response = new RDMMessage()
                {
                    Command = message.Command | ERDM_Command.RESPONSE,
                    DestUID = message.SourceUID,
                    SourceUID = message.DestUID,
                    Parameter = parameterBag.PID,
                    SubDevice = message.SubDevice,
                    ParameterData = count == 0 ? new AcknowledgeTimer(TimeSpan.FromSeconds(3)).ToPayloadData() : MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("strikes", 0, LAMP_STRIKES) })),
                    PortID_or_Responsetype = count == 0 ? (byte)ERDM_ResponseType.ACK_TIMER : (byte)ERDM_ResponseType.ACK
                };

                await Task.Delay(10);
                count++;
                helper.ReceiveMessage(response);
            }
        }
    }
}