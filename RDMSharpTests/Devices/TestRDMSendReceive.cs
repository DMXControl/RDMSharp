using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceive
    {
        private MockGeneratedDevice1? generated;
        private MockDevice? remote;
        private Random random = new Random();
        [SetUp]
        public void Setup()
        {
            var uid = new UID((ushort)random.Next(), (uint)random.Next());
            generated = new MockGeneratedDevice1(uid);
            remote = new MockDevice(uid);
        }
        [TearDown]
        public void TearDown()
        {
            generated?.Dispose();
            generated = null;
            remote?.Dispose();
            remote = null;
        }

        [Test, Retry(3), Order(1)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public async Task TestDevice1()
        {
            var parameterValuesRemote = remote!.GetAllParameterValues();
            var parameterValuesGenerated = generated!.GetAllParameterValues();

            Console.WriteLine($"Generated: {String.Join(", ", parameterValuesGenerated.OrderBy(p => p.Key).Select(x => $"{x.Key}"))}");
            Console.WriteLine($"Remote:    {String.Join(", ", parameterValuesRemote.OrderBy(p => p.Key).Select(x => $"{x.Key}"))}");

            Assert.Multiple(() =>
            {
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
            });

            var deviceLabelModule = generated.Modules.OfType<DeviceLabelModule>().Single();
            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_INFO], Is.EqualTo(generated.DeviceInfo));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(deviceLabelModule.DeviceLabel));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_MODEL_DESCRIPTION], Is.EqualTo(generated.DeviceModelDescription));
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.MANUFACTURER_LABEL], Is.EqualTo(generated.ManufacturerLabel));
                Assert.That(((RDMDMXPersonality)remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY]).Index, Is.EqualTo(generated.CurrentPersonality));
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
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DMX_PERSONALITY], Is.EqualTo(new RDMDMXPersonality(3, 3)));
                Assert.That(generated.CurrentPersonality, Is.EqualTo(3));
            });

            string label = "Changed Device Label";
            await remote.SetParameter(ERDM_Parameter.DEVICE_LABEL, label);
            Assert.Multiple(() =>
            {
                Assert.That(remote.GetAllParameterValues()[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo(label));
                Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo(label));
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
        }

        [Test, Order(2)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public void TestDevice1Slots()
        {
            var slotIntensity = remote!.Slots[0];
            var slotStrobe = remote.Slots[1];
            var slotRed = remote.Slots[2];
            var slotGreen = remote.Slots[3];
            var slotBlue = remote.Slots[4];

            Assert.Multiple(() =>
            {
                Assert.That(slotIntensity, Is.EqualTo(generated!.Personalities[0].Slots[0]));
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

        [Test, CancelAfter(10000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", "NUnit2010:Use EqualConstraint for better assertion messages in case of failure", Justification = "<Ausstehend>")]
        public async Task TestDevice1Sensor()
        {
            var sensorsRemote = remote!.Sensors.Values.ToList();
            var sensorsGenerated = generated!.Sensors.Values.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(0));
            });
            Assert.Multiple(() =>
            {
                Assert.That(remote.DeviceModel.SupportedBlueprintParameters, Contains.Item(ERDM_Parameter.SENSOR_DEFINITION));
                Assert.That(remote.DeviceModel.SupportedNonBlueprintParameters, Contains.Item(ERDM_Parameter.SENSOR_VALUE));
            });

            Assert.Multiple(() =>
            {
                Assert.That(sensorsRemote, Has.Count.EqualTo(sensorsGenerated.Count));
                Assert.That(sensorsRemote, Is.EqualTo(sensorsGenerated));
            });

            await Task.Delay(100);

            #region Update Sensor Values
            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(200);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(500);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(800);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(0));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            #region Record Sensor Values (generated)
            ((MockGeneratedSensor)generated.Sensors[0]).RecordValue();
            ((MockGeneratedSensor)generated.Sensors[1]).RecordValue();
            ((MockGeneratedSensor)generated.Sensors[2]).RecordValue();
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(800));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            #region Record Sensor Values (remote)
            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(4400);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(6600);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(7700);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(800));
            });

            await Task.WhenAll(
                remote.SetParameter(ERDM_Parameter.RECORD_SENSORS, generated.Sensors[0].SensorId),
                remote.SetParameter(ERDM_Parameter.RECORD_SENSORS, generated.Sensors[1].SensorId),
                remote.SetParameter(ERDM_Parameter.RECORD_SENSORS, generated.Sensors[2].SensorId));
            await Task.Delay(400);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(200));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(500));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(800));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(7700));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            #region Reset Sensors
            ((MockGeneratedSensor)generated.Sensors[0]).ResetValues();
            ((MockGeneratedSensor)generated.Sensors[1]).ResetValues();
            ((MockGeneratedSensor)generated.Sensors[2]).ResetValues();
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(7700));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            #region Record Sensor Values (remote) Broadcast
            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(1);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(1);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(1);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(7700));
            });

            await remote.SetParameter(ERDM_Parameter.RECORD_SENSORS, (byte)0xff);
            await Task.Delay(400);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(1));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            #region Reset Sensor Values (remote) Broadcast
            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(2);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(2);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(2);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(4400));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(6600));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(7700));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(1));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(1));
            });

            await remote.SetParameter(ERDM_Parameter.SENSOR_VALUE, (byte)0xff);
            await Task.Delay(400);
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(2));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(2));
            });

            await Task.Delay(400);
            doTests(remote.Sensors.Values.ToArray(), generated.Sensors.Values.ToArray());
            #endregion

            void doTests(Sensor[] remoteSensors, Sensor[] generatedSensors)
            {
                for (byte s= 0; s<remoteSensors.Length;s++)
                {
                    var remoteSensor = remoteSensors[s];
                    var generatedSensor = generatedSensors[s];
                    Assert.Multiple(() =>
                    {
                        Assert.That(remoteSensor.PresentValue, Is.EqualTo(generatedSensor.PresentValue));
                        Assert.That(remoteSensor.LowestValue, Is.EqualTo(generatedSensor.LowestValue));
                        Assert.That(remoteSensor.HighestValue, Is.EqualTo(generatedSensor.HighestValue));
                        Assert.That(remoteSensor.RecordedValue, Is.EqualTo(generatedSensor.RecordedValue));
                        Assert.That(remoteSensor, Is.EqualTo(generatedSensor));
                    });
                }
            }
        }
        [Test, Order(4), CancelAfter(10000)]
        public async Task TestDevice1QueuedUpdates()
        {
            var parameterValuesRemote = remote!.GetAllParameterValues();
            var parameterValuesGenerated = generated!.GetAllParameterValues();
            Assert.That(parameterValuesRemote[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);

            await Task.Delay(400);

            var deviceLabelModule = generated.Modules.OfType<DeviceLabelModule>().Single();

            generated.DMXAddress = 69;
            deviceLabelModule.DeviceLabel = "Test Label QUEUE";
            generated.Identify = true;

            await Task.Delay(400);
            parameterValuesRemote = remote.GetAllParameterValues();

            Assert.That(parameterValuesRemote[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo(69));
            Assert.That(parameterValuesRemote[ERDM_Parameter.DEVICE_LABEL], Is.EqualTo("Test Label QUEUE"));
            Assert.That(parameterValuesRemote[ERDM_Parameter.IDENTIFY_DEVICE], Is.True);

            generated.Identify = false;
            generated.DMXAddress = 44;
            generated.CurrentPersonality = 2;

            await Task.Delay(400);
            parameterValuesRemote = remote.GetAllParameterValues();
            Assert.That(parameterValuesRemote[ERDM_Parameter.IDENTIFY_DEVICE], Is.False);
            Assert.That(parameterValuesRemote[ERDM_Parameter.DMX_START_ADDRESS], Is.EqualTo(44));
            Assert.That(parameterValuesRemote[ERDM_Parameter.DMX_PERSONALITY], Is.EqualTo(new RDMDMXPersonality(2, 3)));
        }
    }
}