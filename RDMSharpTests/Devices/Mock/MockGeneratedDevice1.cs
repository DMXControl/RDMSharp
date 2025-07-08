using RDMSharp.RDM.Device.Module;

namespace RDMSharpTests.Devices.Mock
{
    internal class MockGeneratedDevice1 : AbstractMockGeneratedDevice
    {
        public override EManufacturer ManufacturerID
        {
            get
            {
                return (EManufacturer)UID.ManufacturerID;
            }
        }
        public override ushort DeviceModelID => 20;
        public override ERDM_ProductCategoryCoarse ProductCategoryCoarse => ERDM_ProductCategoryCoarse.CONTROL;
        public override ERDM_ProductCategoryFine ProductCategoryFine => ERDM_ProductCategoryFine.DATA_CONVERSION;
        public override uint SoftwareVersionID => 0x1234;
        public override bool SupportDMXAddress => true;

        private static readonly GeneratedPersonality[] PERSONALITYS = new GeneratedPersonality[] {
            new GeneratedPersonality(1, "5CH RGB",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer" ),
                new Slot(1, ERDM_SlotCategory.STROBE, "Strobe" , 33),
                new Slot(2, ERDM_SlotCategory.COLOR_ADD_RED, "Red" ),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green" ),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue" )),
            new GeneratedPersonality(2, "8CH RGBAWY",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer" ),
                new Slot(1, ERDM_SlotCategory.STROBE, "Strobe" , 33),
                new Slot(2, ERDM_SlotCategory.COLOR_ADD_RED, "Red" ),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green" ),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue" ),
                new Slot(5, ERDM_SlotCategory.COLOR_CORRECTION, "Amber" ),
                new Slot(6, ERDM_SlotCategory.COLOR_CORRECTION, "White" ),
                new Slot(7, ERDM_SlotCategory.COLOR_CORRECTION, "Yellow" )),
            new GeneratedPersonality(3, "9CH RGB 16-Bit",
                new Slot(0, ERDM_SlotCategory.INTENSITY, "Dimmer" ),
                new Slot(1, ERDM_SlotCategory.INTENSITY,ERDM_SlotType.SEC_FINE, "Dimmer Fine"),
                new Slot(2, ERDM_SlotCategory.STROBE, "Strobe" , 33),
                new Slot(3, ERDM_SlotCategory.COLOR_ADD_RED, "Red" ),
                new Slot(4, ERDM_SlotCategory.COLOR_ADD_RED, ERDM_SlotType.SEC_FINE,"Red Fine"),
                new Slot(5, ERDM_SlotCategory.COLOR_ADD_GREEN, "Green" ),
                new Slot(6, ERDM_SlotCategory.COLOR_ADD_GREEN, ERDM_SlotType.SEC_FINE,"Green Fine"),
                new Slot(7, ERDM_SlotCategory.COLOR_ADD_BLUE, "Blue" ),
                new Slot(8, ERDM_SlotCategory.COLOR_ADD_BLUE,ERDM_SlotType.SEC_FINE, "Blue Fine" )) };

        private static Sensor[] GetSensors() {
            return new Sensor[] {
            new MockSensorTemp(0, 1, 3000),
            new MockSensorTemp(1, 2, 8000),
            new MockSensorTemp(2, 3, 12000),
            new MockSensorVolt3_3(3, 331),
            new MockSensorVolt5(4, 498) };
        }
        public override GeneratedPersonality[] Personalities => PERSONALITYS;

        public override bool SupportQueued => true;

        public override bool SupportStatus => true;

        public MockGeneratedDevice1(UID uid, IReadOnlyCollection<IModule> modules = null) : base(uid, SubDevice.Root, new ERDM_Parameter[] { }, GetSensors(), GetModules().Concat(modules ?? Array.Empty<IModule>()).ToList().AsReadOnly())
        {
            this.SoftwareVersionLabel = $"Dummy Software";
        }
        private static IReadOnlyCollection<IModule> GetModules()
        {
            return new IModule[] {
                new DeviceLabelModule("Dummy Device 1"),
                new ManufacturerLabelModule("Dummy Manufacturer 9FFF"),
                new DeviceModelDescriptionModule("Test Model Description"),
                new BootSoftwareVersionModule(123, $"Dummy Bootloader Software")};
        }
        protected sealed override void OnDispose()
        {
        }

        private class MockSensorTemp : MockGeneratedSensor
        {
            public MockSensorTemp(in byte sensorId, in byte number, in short initValue) : base(sensorId, initValue, ERDM_SensorType.TEMPERATURE, ERDM_SensorUnit.CENTIGRADE, ERDM_UnitPrefix.CENTI, $"Mock Temp. {number}", -2000, 10000, 2000, 5000, true, true)
            {
            }
        }
        private class MockSensorVolt3_3 : MockGeneratedSensor
        {
            public MockSensorVolt3_3(in byte sensorId, in short initValue) : base(sensorId, initValue, ERDM_SensorType.VOLTAGE, ERDM_SensorUnit.VOLTS_DC, ERDM_UnitPrefix.CENTI, $"Mock 3.3V Rail", -200, 500, 330, 360, true, true)
            {
            }
        }
        private class MockSensorVolt5 : MockGeneratedSensor
        {
            public MockSensorVolt5(in byte sensorId, in short initValue) : base(sensorId, initValue, ERDM_SensorType.VOLTAGE, ERDM_SensorUnit.VOLTS_DC, ERDM_UnitPrefix.CENTI, $"Mock 5V Rail ", -200, 1000, 470, 530, true, true)
            {
            }
        }
    }
    public class MockGeneratedSensor : Sensor
    {
        protected MockGeneratedSensor(in byte sensorId,
                                      in short initValue,
                                      in ERDM_SensorType type,
                                      in ERDM_SensorUnit unit,
                                      in ERDM_UnitPrefix prefix,
                                      in string description,
                                      in short rangeMinimum,
                                      in short rangeMaximum,
                                      in short normalMinimum,
                                      in short normalMaximum,
                                      in bool lowestHighestValueSupported = false,
                                      in bool recordedValueSupported = false) : base(
                                          sensorId,
                                          initValue,
                                          type,
                                          unit,
                                          prefix,
                                          description,
                                          rangeMinimum,
                                          rangeMaximum,
                                          normalMinimum,
                                          normalMaximum,
                                          lowestHighestValueSupported,
                                          recordedValueSupported)
        {
        }

        public new void UpdateValue(short value)
        {
            base.UpdateValue(value);
        }

        public new void RecordValue()
        {
            base.RecordValue();
        }

        public new void ResetValues()
        {
            base.ResetValues();
        }

    }
}