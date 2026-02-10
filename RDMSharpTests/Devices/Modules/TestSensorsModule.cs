using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestSensorsModule
{
    private MockGeneratedDevice1? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(123, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        generated = new MockGeneratedDevice1(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test, Order(51)]
    public void TestGetSENSOR_DEFINITION()
    {
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.SENSOR_DEFINITION,
            SubDevice = SubDevice.Root,
        };
        RDMMessage? response = null;
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Sensors, Has.Count.EqualTo(5));
        doTests(generated.Sensors.Values.ToArray());
        #endregion

        void doTests(Sensor[] sensors)
        {
            foreach (Sensor sensor in sensors)
            {
                request.ParameterData = new byte[] { sensor.SensorId };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(13 + sensor.Description.Length));
                Assert.That(response.Value, Is.EqualTo(new RDMSensorDefinition(sensor.SensorId, sensor.Type, sensor.Unit, sensor.Prefix, sensor.RangeMinimum, sensor.RangeMaximum, sensor.NormalMinimum, sensor.NormalMaximum, sensor.LowestHighestValueSupported, sensor.RecordedValueSupported, sensor.Description)));
            }
        }
    }

    [Test, Order(52)]
    public void TestGetSENSOR_VALUE()
    {
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.SENSOR_VALUE,
            SubDevice = SubDevice.Root,
        };
        RDMMessage? response = null;
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));

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

        doTests(generated.Sensors.Values.ToArray());
        #endregion

        #region Test New Sensor Values
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Sensors, Has.Count.EqualTo(5));
        ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(99);
        ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(122);
        ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(155);

        Assert.Multiple(() =>
        {
            Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(99));
            Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(122));
            Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(155));

            Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(99));
            Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(122));
            Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(155));

            Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
            Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
            Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
        });
        doTests(generated.Sensors.Values.ToArray());
        #endregion

        #region Test Recorded Sensor Values
        Assert.Multiple(() =>
        {
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));
            Assert.That(generated.Sensors[0].RecordedValueSupported, Is.True);
            Assert.That(generated.Sensors[1].RecordedValueSupported, Is.True);
            Assert.That(generated.Sensors[2].RecordedValueSupported, Is.True);
            Assert.That(generated.Sensors[0].LowestHighestValueSupported, Is.True);
            Assert.That(generated.Sensors[1].LowestHighestValueSupported, Is.True);
            Assert.That(generated.Sensors[2].LowestHighestValueSupported, Is.True);
        });

        ((MockGeneratedSensor)generated.Sensors[0]).RecordValue();
        ((MockGeneratedSensor)generated.Sensors[1]).RecordValue();
        ((MockGeneratedSensor)generated.Sensors[2]).RecordValue();

        ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(111);
        ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(666);
        ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(987);

        Assert.Multiple(() =>
        {
            Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(99));
            Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(122));
            Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(155));

            Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(111));
            Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(666));
            Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(987));

            Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(99));
            Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(122));
            Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(155));

            Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
            Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
            Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
        });
        doTests(generated.Sensors.Values.ToArray());
        #endregion

        #region Test Reset Sensor Values Set_Request
        request.Command = ERDM_Command.SET_COMMAND;
        for (byte b = 0; b < 2; b++)
        {
            request.ParameterData = new byte[] { b };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                Assert.That(response.ParameterData[0], Is.EqualTo(b));
                Assert.That(response.Value, Is.TypeOf(typeof(RDMSensorValue)));
            });
        }
        #endregion

        #region Test Reset Sensor Values Set_Request (Broadcast)
        request.ParameterData = new byte[] { 0xff };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9));
            Assert.That(response.ParameterData[0], Is.EqualTo(0xff));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMSensorValue)));
        });
        foreach (var generatedSensor in generated.Sensors.Values)
        {
            Assert.Multiple(() =>
            {
                Assert.That(generatedSensor, Is.Not.Null);
                Sensor remoteSensor = generated.Sensors[generatedSensor.SensorId];
                Assert.That(remoteSensor, Is.Not.Null);
                Assert.That(remoteSensor.PresentValue, Is.EqualTo(generatedSensor.PresentValue));
                Assert.That(remoteSensor.LowestValue, Is.EqualTo(generatedSensor.LowestValue));
                Assert.That(remoteSensor.HighestValue, Is.EqualTo(generatedSensor.HighestValue));
                Assert.That(remoteSensor.RecordedValue, Is.EqualTo(generatedSensor.RecordedValue));
            });
        }
        #endregion

        void doTests(Sensor[] sensors)
        {
            foreach (Sensor sensor in sensors)
            {
                request.ParameterData = new byte[] { sensor.SensorId };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.Multiple(() =>
                {
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                    Assert.That(response.Value, Is.EqualTo(new RDMSensorValue(sensor.SensorId, sensor.PresentValue, sensor.LowestValue, sensor.HighestValue, sensor.RecordedValue)));
                });
            }
        }
    }
    [Test, Order(53)]
    public void TestGetRECORD_SENSORS()
    {
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.SET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.RECORD_SENSORS,
            SubDevice = SubDevice.Root,
        };
        RDMMessage? response = null;

        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Sensors, Has.Count.EqualTo(5));

        #region Test Recorded Sensor Values Set_Request
        for (byte b = 0; b < 2; b++)
        {
            request.ParameterData = new byte[] { b };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            });
        }
        #endregion

        #region Test Recorded Sensor Values Set_Request (Broadcast)
        request.ParameterData = new byte[] { 0xff };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        });
        foreach (var generatedSensor in generated.Sensors.Values)
        {
            Assert.Multiple(() =>
            {
                Assert.That(generatedSensor, Is.Not.Null);
                Sensor remoteSensor = generated.Sensors[generatedSensor.SensorId];
                Assert.That(remoteSensor, Is.Not.Null);
                Assert.That(remoteSensor.PresentValue, Is.EqualTo(generatedSensor.PresentValue));
                Assert.That(remoteSensor.LowestValue, Is.EqualTo(generatedSensor.LowestValue));
                Assert.That(remoteSensor.HighestValue, Is.EqualTo(generatedSensor.HighestValue));
                Assert.That(remoteSensor.RecordedValue, Is.EqualTo(generatedSensor.RecordedValue));
            });
        }
        #endregion

        #region Test Invalid Calls
        request.ParameterData = new byte[] { 30 };
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.DATA_OUT_OF_RANGE));

        request.ParameterData = new byte[] { 2 };
        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
        Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
        #endregion
    }
}