using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceiveSubDevices
    {
        private MockGeneratedDeviceWithSubDeviceMaster1 generated;
        private MockDevice remote;

        private const ushort SUBDEVICE_COUNT = 12;

        [SetUp]
        public void Setup()
        {
            var uid = new UID(0x9fef, 1);
            generated = new MockGeneratedDeviceWithSubDeviceMaster1(uid, SUBDEVICE_COUNT);
            remote = new MockDevice(uid, false);
        }
        [TearDown]
        public void TearDown()
        {
            generated.Dispose();
            remote.Dispose();
        }

        [Test, CancelAfter(10000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public async Task TestDevice1()
        {
            while (!remote.IsInitialized)
                await Task.Delay(10);

            SubDevice[] subDeviceIDs_generated = generated.SubDevices.Select(x => x.Subdevice).ToArray();
            SubDevice[] subDeviceIDs_remote = remote.SubDevices.Select(x => x.Subdevice).ToArray();

            Assert.That(generated.DeviceInfo.SubDeviceCount, Is.EqualTo(SUBDEVICE_COUNT));
            Assert.That(generated.SubDevices.Count, Is.EqualTo(SUBDEVICE_COUNT + 1));

            Assert.That(remote.DeviceInfo.SubDeviceCount, Is.EqualTo(SUBDEVICE_COUNT));
            Assert.That(remote.SubDevices.Count, Is.EqualTo(SUBDEVICE_COUNT + 1));

            Assert.That(subDeviceIDs_remote, Is.EquivalentTo(subDeviceIDs_generated));

            Assert.That(subDeviceIDs_generated, Has.ItemAt(0).EqualTo(SubDevice.Root));
            Assert.That(subDeviceIDs_remote, Has.ItemAt(0).EqualTo(SubDevice.Root));

            foreach(AbstractGeneratedRDMDevice gen in generated.SubDevices)
            {
                await PerformTests((AbstractRemoteRDMDevice)remote.SubDevices.First(sd => sd.Subdevice == gen.Subdevice), gen);
            }

        }
        private async Task PerformTests(AbstractRemoteRDMDevice remote, AbstractGeneratedRDMDevice generated)
        {
            var parameterValuesRemote = remote.GetAllParameterValues();
            var parameterValuesGenerated = generated.GetAllParameterValues();
            //Assert.Multiple(() =>
            //{
            Assert.That(parameterValuesGenerated.Keys, Is.EquivalentTo(parameterValuesRemote.Keys));
            foreach (var parameter in parameterValuesGenerated.Keys)
            {
                Assert.That(parameterValuesRemote.Keys, Contains.Item(parameter), $"Tested Parameter {parameter}");
                if (parameterValuesGenerated[parameter] is Array)
                    Assert.That(parameterValuesGenerated[parameter], Is.EquivalentTo((Array)parameterValuesRemote[parameter]), $"Tested Parameter {parameter}");
                else
                    Assert.That(parameterValuesGenerated[parameter], Is.EqualTo(parameterValuesRemote[parameter]), $"Tested Parameter {parameter}");
            }
            foreach (var parameter in parameterValuesRemote.Keys)
            {
                Assert.That(parameterValuesGenerated.Keys, Contains.Item(parameter), $"Tested Parameter {parameter}");
                if (parameterValuesRemote[parameter] is Array)
                    Assert.That(parameterValuesRemote[parameter], Is.EquivalentTo((Array)parameterValuesGenerated[parameter]), $"Tested Parameter {parameter}");
                else
                    Assert.That(parameterValuesRemote[parameter], Is.EqualTo(parameterValuesGenerated[parameter]), $"Tested Parameter {parameter}");
            }
            Assert.That(parameterValuesRemote, Has.Count.EqualTo(parameterValuesGenerated.Count));
            //});

            //Assert.Multiple(() =>
            //{
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_INFO], Is.EqualTo(generated.DeviceInfo));
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(generated.DeviceLabel));
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_MODEL_DESCRIPTION], Is.EqualTo(generated.DeviceModelDescription));
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.MANUFACTURER_LABEL], Is.EqualTo(generated.ManufacturerLabel));
            Assert.That(((RDMDMXPersonality)remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY]).Index, Is.EqualTo(generated.CurrentPersonality));
            //});

            await remote.SetParameter(ERDM_Parameter.DMX_START_ADDRESS, (ushort)512);
            //Assert.Multiple(() =>
            //{
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo(512));
            Assert.That(generated.DMXAddress, Is.EqualTo(512));
            //});

            if (generated.Personalities.Any(p => p.ID == 3))
            {
                await remote.SetParameter(ERDM_Parameter.DMX_PERSONALITY, (byte)3);
                //Assert.Multiple(() =>
                //{
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY], Is.EqualTo(3));
                Assert.That(generated.CurrentPersonality, Is.EqualTo(3));
                //});
            }

            string label = "Changed Device Label";
            await remote.SetParameter(ERDM_Parameter.DEVICE_LABEL, label);
            //Assert.Multiple(() =>
            //{
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(label));
            Assert.That(generated.DeviceLabel, Is.EqualTo(label));
            //});
            //Assert.Multiple(async () =>
            //{
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            await remote.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, true);
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.True);
            Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.True);
            await remote.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, false);
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            //});
            //Assert.Multiple(() =>
            //{
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo()); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, new RDMDMXPersonalityDescription(1, 2, "dasdad")); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, "de"); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, 333); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, "Test"); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DISC_MUTE, null); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, new RDMDeviceInfo()); });
            //});
        }
    }
}