using RDMSharpTests.Devices.Mock;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace RDMSharpTest.RDM.Devices
{
    public class TestRDMSendReceive
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestDevice1()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var uid = new RDMUID(0x9fff, 1);
            var generated = new MockGeneratedDevice1(uid);
            var remote = new MockDevice(uid);
            while (remote.DeviceModel?.IsInitialized != true || !remote.AllDataPulled)
            {
                await Task.Delay(10);
                if(sw.ElapsedMilliseconds>5000)
                {
                    if (remote.DeviceModel?.IsInitialized != true)
                        Assert.Fail("Timeouted because DeviceModel not Initialized");
                    else if (!remote.AllDataPulled)
                        Assert.Fail("Timeouted because AllDataPulled not true");
                }
            }
            testAllValues();

            void testAllValues()
            {
                var parameterValuesRemote = remote.GetAllParameterValues();
                var parameterValuesGenerated = generated.GetAllParameterValues();
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
                Assert.That(parameterValuesRemote.Count, Is.EqualTo(parameterValuesGenerated.Count));
            }

            await remote.SetParameter(ERDM_Parameter.DMX_START_ADDRESS, (ushort)512);
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo(512));
            Assert.That(generated.DMXAddress, Is.EqualTo(512));

            await remote.SetParameter(ERDM_Parameter.DMX_PERSONALITY, (byte)3);
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY], Is.EqualTo(3));
            Assert.That(generated.CurrentPersonality, Is.EqualTo(3));

            string label = "Changed Device Label";
            await remote.SetParameter(ERDM_Parameter.DEVICE_LABEL, label);
            Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(label));
            Assert.That(generated.DeviceLabel, Is.EqualTo(label));

            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_INFO, new RDMDeviceInfo()); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, new RDMDMXPersonalityDescription(1, 2, "dasdad")); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, "de"); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.LANGUAGE, 333); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, "Test"); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DISC_MUTE, null); });
            Assert.Throws(typeof(NotSupportedException), () => { generated.TrySetParameter(ERDM_Parameter.DEVICE_LABEL, new RDMDeviceInfo()); });
        }
    }
}