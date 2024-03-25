using RDMSharpTests.Devices.Mock;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceive
    {
        private MockGeneratedDevice1 generated;
        private MockDevice remote;
        [SetUp]
        public void Setup()
        {
            var uid = new RDMUID(0x9fff, 1);
            generated = new MockGeneratedDevice1(uid);
            remote = new MockDevice(uid, false);
        }
        [TearDown]
        public void TearDown()
        {
            generated.Dispose();
            remote.Dispose();
        }

        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public async Task TestDevice1()
        {
            var parameterValuesRemote = remote.GetAllParameterValues();
            var parameterValuesGenerated = generated.GetAllParameterValues();
            var sensorsRemote = remote.Sensors.Values.ToList();
            var sensorsGenerated = generated.Sensors.ToList();

            Assert.Multiple(() =>
            {
                foreach (var parameter in parameterValuesGenerated.Keys)
                {
                    Assert.That(parameterValuesRemote.Keys, Contains.Item(parameter));
                    Assert.That(parameterValuesGenerated[parameter], Is.EqualTo(parameterValuesRemote[parameter]));
                }
                foreach (var parameter in parameterValuesRemote.Keys)
                {
                    Assert.That(parameterValuesGenerated.Keys, Contains.Item(parameter));
                    Assert.That(parameterValuesRemote[parameter], Is.EqualTo(parameterValuesGenerated[parameter]));
                }
                Assert.That(parameterValuesRemote, Has.Count.EqualTo(parameterValuesGenerated.Count));
            });
            Assert.Multiple(() =>
            {
                Assert.That(remote.DeviceModel.SupportedBlueprintParameters, Contains.Item(ERDM_Parameter.SENSOR_DEFINITION));
                Assert.That(remote.DeviceModel.SupportedNonBlueprintParameters, Contains.Item(ERDM_Parameter.SENSOR_VALUE));
            });

            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_INFO], Is.EqualTo(generated.DeviceInfo));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(generated.DeviceLabel));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_MODEL_DESCRIPTION], Is.EqualTo(generated.DeviceModelDescription));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.MANUFACTURER_LABEL], Is.EqualTo(generated.ManufacturerLabel));
                Assert.That(((RDMDMXPersonality)remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY]).Index, Is.EqualTo(generated.CurrentPersonality));
            });
            Assert.Multiple(() =>
            {
                
                Assert.That(sensorsRemote, Has.Count.EqualTo(sensorsGenerated.Count));
                Assert.That(sensorsRemote, Is.EqualTo(sensorsGenerated));
            });

            await remote.SetParameter(ERDM_Parameter.DMX_START_ADDRESS, (ushort)512);
            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo(512));
                Assert.That(generated.DMXAddress, Is.EqualTo(512));
            });

            await remote.SetParameter(ERDM_Parameter.DMX_PERSONALITY, (byte)3);
            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY], Is.EqualTo(3));
                Assert.That(generated.CurrentPersonality, Is.EqualTo(3));
            });

            string label = "Changed Device Label";
            await remote.SetParameter(ERDM_Parameter.DEVICE_LABEL, label);
            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(label));
                Assert.That(generated.DeviceLabel, Is.EqualTo(label));
            });
            Assert.Multiple(async () =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
                Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
                await remote.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, true);
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.True);
                Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.True);
                await remote.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, false);
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
                Assert.That(generated.GetAllParameterValues()[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            });
            Assert.Multiple(() =>
            {
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo()); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, new RDMDMXPersonalityDescription(1, 2, "dasdad")); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, "de"); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, 333); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, "Test"); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DISC_MUTE, null); });
                Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, new RDMDeviceInfo()); });
            });

            var slotIntensity = remote.Slots[0];
            var slotStrobe = remote.Slots[1];
            var slotRed = remote.Slots[2];
            var slotGreen = remote.Slots[3];
            var slotBlue = remote.Slots[4];

            Assert.Multiple(() =>
            {
                Assert.That(slotIntensity, Is.EqualTo(generated.Personalities[0].Slots[0]));
                Assert.That(slotStrobe, Is.EqualTo(generated.Personalities[0].Slots[1]));
                Assert.That(slotRed, Is.EqualTo(generated.Personalities[0].Slots[2]));
                Assert.That(slotGreen, Is.EqualTo(generated.Personalities[0].Slots[3]));
                Assert.That(slotBlue, Is.EqualTo(generated.Personalities[0].Slots[4]));

                Assert.That(slotIntensity, Is.Not.EqualTo(slotStrobe));
                Assert.That(slotIntensity, Is.Not.EqualTo(slotRed));
                Assert.That(slotIntensity, Is.Not.EqualTo(slotGreen));
                Assert.That(slotIntensity, Is.Not.EqualTo(slotBlue));
                Assert.That(slotIntensity.DefaultValue, Is.EqualTo(0));
                Assert.That(slotIntensity.Category, Is.EqualTo(ERDM_SlotCategory.INTENSITY));
                Assert.That(slotIntensity.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                Assert.That(slotIntensity.DefaultValue, Is.EqualTo(0));

                Assert.That(slotStrobe, Is.Not.EqualTo(slotRed));
                Assert.That(slotStrobe, Is.Not.EqualTo(slotGreen));
                Assert.That(slotStrobe, Is.Not.EqualTo(slotBlue));
                Assert.That(slotStrobe.Category, Is.EqualTo(ERDM_SlotCategory.STROBE));
                Assert.That(slotStrobe.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                Assert.That(slotStrobe.DefaultValue, Is.EqualTo(33));

                Assert.That(slotRed, Is.Not.EqualTo(slotGreen));
                Assert.That(slotRed, Is.Not.EqualTo(slotBlue));
                Assert.That(slotRed.Category, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_RED));
                Assert.That(slotRed.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                Assert.That(slotRed.DefaultValue, Is.EqualTo(0));

                Assert.That(slotGreen, Is.Not.EqualTo(slotBlue));
                Assert.That(slotGreen.Category, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_GREEN));
                Assert.That(slotGreen.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                Assert.That(slotGreen.DefaultValue, Is.EqualTo(0));

                Assert.That(slotBlue.Category, Is.EqualTo(ERDM_SlotCategory.COLOR_ADD_BLUE));
                Assert.That(slotBlue.Type, Is.EqualTo(ERDM_SlotType.PRIMARY));
                Assert.That(slotBlue.DefaultValue, Is.EqualTo(0));

                Assert.That(slotIntensity != slotStrobe, Is.True);
                Assert.That(slotRed != slotBlue, Is.True);

                Assert.That(slotIntensity == slotStrobe, Is.False);
                Assert.That(slotRed == slotBlue, Is.False);

                Assert.That(((object)slotRed).Equals(null), Is.False);
                Assert.That(((object)slotRed!).Equals(slotBlue), Is.False);
                Assert.That(slotRed.Equals(slotBlue), Is.False);
                Assert.That(slotRed.Equals(slotStrobe), Is.False);

                Assert.That(string.IsNullOrWhiteSpace(slotRed.ToString()), Is.False);
                HashSet<Slot> slots = new HashSet<Slot>();
                Assert.That(slots.Add(slotIntensity), Is.True);
                Assert.That(slots.Add(slotIntensity), Is.False);
                Assert.That(slots, Does.Contain(slotIntensity));
                Assert.That(slots.Add(slotStrobe), Is.True);
                Assert.That(slots.Add(slotStrobe), Is.False);
                Assert.That(slots, Does.Contain(slotStrobe));
                Assert.That(slots.Add(slotRed), Is.True);
                Assert.That(slots.Add(slotRed), Is.False);
                Assert.That(slots, Does.Contain(slotRed));
                Assert.That(slots.Add(slotGreen), Is.True);
                Assert.That(slots.Add(slotGreen), Is.False);
                Assert.That(slots, Does.Contain(slotGreen));
                Assert.That(slots.Add(slotBlue), Is.True);
                Assert.That(slots.Add(slotBlue), Is.False);
                Assert.That(slots, Does.Contain(slotBlue));
            });
        }
    }
}