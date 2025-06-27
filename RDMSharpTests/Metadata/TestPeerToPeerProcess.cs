using RDMSharp.Metadata;

namespace RDMSharpTests.Metadata
{
    public class TestPeerToPeerProcess
    {

        [Test, MaxTime(6000)]
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

            AsyncRDMRequestHelper? helper = null;
            helper = new AsyncRDMRequestHelper(sendMessage);

            try
            {
                await peerToPeerProcess.Run(helper);

                Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Value, Is.EqualTo(DMX_ADDRESS));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(DMX_ADDRESS));
                Assert.That(peerToPeerProcess.Exception, Is.Null);
            }
            finally
            {
                helper?.Dispose();
                helper = null;
            }

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
                    ParameterData = MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("dmx_address", 0, DMX_ADDRESS) })).First()
                };

                await Task.Delay(10);
                helper?.ReceiveMessage(response);
            }
        }


        [Test, MaxTime(6000)]
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
            Assert.That(peerToPeerProcess.Exception, Is.Null);

            AsyncRDMRequestHelper? helper = null;
            byte[] parameterData = MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("device_uids", 0, children: children) })).SelectMany(en=>en).ToArray();
            helper = new AsyncRDMRequestHelper(sendMessage);

            try
            {
                await peerToPeerProcess.Run(helper);

                Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Children, Is.EqualTo(children));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(children.Select(dt => dt.Value).ToList()));
            }
            finally
            {
                helper?.Dispose();
                helper = null;
            }

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
                helper?.ReceiveMessage(response);
            }
        }
        [Test, MaxTime(6000)]
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
            Assert.That(peerToPeerProcess.Exception, Is.Null);

            AsyncRDMRequestHelper? helper = null;
            byte count = 0;

            try
            {
                helper = new AsyncRDMRequestHelper(sendMessage);

                await peerToPeerProcess.Run(helper);

                Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.Children[0].Value, Is.EqualTo(LAMP_STRIKES));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.EqualTo(LAMP_STRIKES));
                Assert.That(peerToPeerProcess.Exception, Is.Null);
            }
            finally
            {
                helper?.Dispose();
                helper = null;
            }
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
                    ParameterData = count == 0 ? new AcknowledgeTimer(TimeSpan.FromSeconds(3)).ToPayloadData() : MetadataFactory.GetResponseMessageData(parameterBag, new DataTreeBranch(new DataTree[] { new DataTree("strikes", 0, LAMP_STRIKES) })).First(),
                    PortID_or_Responsetype = count == 0 ? (byte)ERDM_ResponseType.ACK_TIMER : (byte)ERDM_ResponseType.ACK
                };

                await Task.Delay(10);
                count++;
                helper?.ReceiveMessage(response);
            }
        }
        [Test, MaxTime(6000)]
        public async Task Test_Get_SLOT_INFO()
        {
            var command = ERDM_Command.GET_COMMAND;
            var uid = new UID(1234, 123456);
            var subdevice = SubDevice.Root;
            var parameterBag = new ParameterBag(ERDM_Parameter.SLOT_INFO);
            byte[] SLOT_INFO_1 = new byte[] {
                0x00, 0x00, 0x00, 0x01, 0x01, 0x00, 0x01, 0x01,
                0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x02, 0x00,
                0x03, 0x01, 0x00, 0x02, 0x00, 0x04, 0x00, 0x05,
                0x03, 0x00, 0x05, 0x00, 0x05, 0x02, 0x00, 0x06,
                0x00, 0x91, 0x00, 0x00, 0x07, 0x00, 0x91, 0x01,
                0x00, 0x08, 0x00, 0x05, 0x02, 0x00, 0x09, 0x00,
                0x02, 0x01, 0x00, 0x0a, 0x01, 0x00, 0x09, 0x00,
                0x0b, 0x00, 0x02, 0x01, 0x00, 0x0c, 0x01, 0x00,
                0x0b, 0x00, 0x0d, 0x00, 0x02, 0x02, 0x00, 0x0e,
                0x00, 0x02, 0x04, 0x00, 0x0f, 0x00, 0x02, 0x03,
                0x00, 0x10, 0x00, 0x02, 0x08, 0x00, 0x11, 0x00,
                0x02, 0x08, 0x00, 0x12, 0x00, 0x05, 0x04, 0x00,
                0x13, 0x00, 0x05, 0x03, 0x00, 0x14, 0x00, 0xff,
                0xff, 0x00, 0x15, 0x00, 0xff, 0xff, 0x00, 0x16,
                0x00, 0x03, 0x04, 0x00, 0x17, 0x06, 0x00, 0x16,
                0x00, 0x18, 0x04, 0x00, 0x17
            };
            byte[] SLOT_INFO_2 = new byte[] {
                0x00, 0x19, 0x00, 0x03, 0x02, 0x00, 0x1a, 0x07,
                0x00, 0x19, 0x00, 0x1b, 0x01, 0x00, 0x1a, 0x00,
                0x1c, 0x00, 0x03, 0x02, 0x00, 0x1d, 0x07, 0x00,
                0x1c, 0x00, 0x1e, 0x01, 0x00, 0x1d, 0x00, 0x1f,
                0x00, 0x03, 0x03, 0x00, 0x20, 0x06, 0x00, 0x1f,
                0x00, 0x21, 0x00, 0x03, 0x03, 0x00, 0x22, 0x06,
                0x00, 0x21, 0x00, 0x23, 0x00, 0x04, 0x03, 0x00,
                0x24, 0x00, 0x04, 0x01, 0x00, 0x25, 0x01, 0x00,
                0x24, 0x00, 0x26, 0x00, 0x04, 0x05, 0x00, 0x27,
                0x01, 0x00, 0x26, 0x00, 0x28, 0x00, 0x04, 0x02,
                0x00, 0x29, 0x01, 0x00, 0x28, 0x00, 0x2a, 0x00,
                0x04, 0x06, 0x00, 0x2b, 0x00, 0x04, 0x06, 0x00,
                0x2c, 0x06, 0x00, 0x2b, 0x00, 0x2d, 0x00, 0x04,
                0x06, 0x00, 0x2e, 0x06, 0x00, 0x2d, 0x00, 0x2f,
                0x00, 0x04, 0x06, 0x00, 0x30, 0x06, 0x00, 0x2f,
                0x00, 0x31, 0x00, 0x04, 0x06, 0x00, 0x32, 0x06,
                0x00, 0x31, 0x00, 0x33, 0x00, 0x04, 0x04, 0x00,
                0x34, 0x00, 0x00, 0x02, 0x00, 0x35, 0x01, 0x00,
                0x34
            };
            PeerToPeerProcess peerToPeerProcess = new PeerToPeerProcess(command, uid, subdevice, parameterBag);

            Assert.That(peerToPeerProcess.Define, Is.Not.Null);
            Assert.That(peerToPeerProcess.State, Is.EqualTo(PeerToPeerProcess.EPeerToPeerProcessState.Waiting));
            Assert.That(peerToPeerProcess.RequestPayloadObject.IsUnset, Is.True);
            Assert.That(peerToPeerProcess.ResponsePayloadObject.IsUnset, Is.True);
            Assert.That(peerToPeerProcess.Exception, Is.Null);

            AsyncRDMRequestHelper? helper = null;
            byte count = 0;

            try
            {
                helper = new AsyncRDMRequestHelper(sendMessage);

                await peerToPeerProcess.Run(helper);

                Assert.That(peerToPeerProcess.ResponsePayloadObject, Is.TypeOf(typeof(DataTreeBranch)));
                Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.Not.Null);
                Assert.That(peerToPeerProcess.ResponsePayloadObject.ParsedObject, Is.TypeOf(typeof(RDMSlotInfo[])));
                RDMSlotInfo[] slotInfos = (RDMSlotInfo[])peerToPeerProcess.ResponsePayloadObject.ParsedObject;
                Assert.That(slotInfos, Has.Length.EqualTo(54));
                Assert.That(peerToPeerProcess.Exception, Is.Null);
            }
            finally
            {
                helper?.Dispose();
                helper = null;
            }
            async Task sendMessage(RDMMessage message)
            {
                Assert.That(count, Is.LessThan(2));

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
                    ParameterData = count % 2 == 0 ? SLOT_INFO_1 : SLOT_INFO_2,
                    PortID_or_Responsetype = count % 2 == 0 ? (byte)ERDM_ResponseType.ACK_OVERFLOW : (byte)ERDM_ResponseType.ACK
                };

                await Task.Delay(10);
                count++;
                helper?.ReceiveMessage(response);
            }
        }
    }
}