namespace RDMSharpTests.Devices.Mock
{
    internal class MockGeneratedDeviceWithSubDevice1 : AbstractMockGeneratedDevice
    {
        public override EManufacturer ManufacturerID => (EManufacturer)0x9fef;
        public override ushort DeviceModelID => 50;
        public override ERDM_ProductCategoryCoarse ProductCategoryCoarse => ERDM_ProductCategoryCoarse.DIMMER;
        public override ERDM_ProductCategoryFine ProductCategoryFine => ERDM_ProductCategoryFine.DIMMER_CS_LED;
        public override uint SoftwareVersionID => 0x3234;
        public override string DeviceModelDescription => "Test Model Description SubDevice";
        public override bool SupportDMXAddress => true;

        private static readonly GeneratedPersonality[] PERSONALITYS = new GeneratedPersonality[] {
            new GeneratedPersonality(1, "1CH",
                new Slot(0, ERDM_SlotCategory.INTENSITY_MASTER, "Master" ))};

        private static readonly Sensor[] SENSORS = new Sensor[] {
            new MockSensorTemp(0, 1, 3000)};
        public override GeneratedPersonality[] Personalities => PERSONALITYS;
        protected MockGeneratedDeviceWithSubDevice1(UID uid, MockGeneratedDeviceWithSubDeviceSub1[] subDevices = null) : base(uid, new ERDM_Parameter[] { ERDM_Parameter.IDENTIFY_DEVICE, ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL }, "Dummy Manufacturer 9FEF", SENSORS, subDevices)
        {
            this.DeviceLabel = "Dummy Device Master";
            this.setInitParameters();
        }
        protected MockGeneratedDeviceWithSubDevice1(UID uid, SubDevice subDevice) : base(uid, subDevice, new ERDM_Parameter[] { ERDM_Parameter.IDENTIFY_DEVICE, ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL }, "Dummy Manufacturer 9FEF", SENSORS)
        {
            this.DeviceLabel = "Dummy Device SubDevice";
            this.setInitParameters();
        }
        private void setInitParameters()
        {
            this.trySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, false);
            this.trySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, $"Dummy Software");
        }

        private class MockSensorTemp : Sensor
        {
            public MockSensorTemp(in byte sensorId, byte number, short initValue) : base(sensorId, ERDM_SensorType.TEMPERATURE, ERDM_SensorUnit.CENTIGRADE, ERDM_UnitPrefix.CENTI, $"Mock Ambient Temp. {number}", -2000, 10000, 2000, 5000, true, true)
            {
                UpdateValue(initValue);
            }
        }

        protected sealed override void OnDispose()
        {
        }
    }
    internal sealed class MockGeneratedDeviceWithSubDeviceMaster1 : MockGeneratedDeviceWithSubDevice1
    {
        public MockGeneratedDeviceWithSubDeviceMaster1(UID uid, ushort subDevicesCount) : base(uid, getSubDevices(uid, subDevicesCount))
        {
        }

        private static MockGeneratedDeviceWithSubDeviceSub1[] getSubDevices(UID uid, ushort count)
        {
            var subDevice = new MockGeneratedDeviceWithSubDeviceSub1[count];
            for (ushort i = 0; i < count; i++)
                subDevice[i] = new MockGeneratedDeviceWithSubDeviceSub1(uid, (ushort)(i + 1));
            return subDevice;
        }
    }
    internal sealed class MockGeneratedDeviceWithSubDeviceSub1 : MockGeneratedDeviceWithSubDevice1
    {
        public MockGeneratedDeviceWithSubDeviceSub1(UID uid, ushort subDeviceID) : base(uid, getSubDevice(subDeviceID))
        {
        }
        private static SubDevice getSubDevice(ushort subDeviceID)
        {
            var subDevice = new SubDevice(subDeviceID);
            if (subDevice == SubDevice.Root)
                throw new ArgumentException("SubDeviceID must not be Root", nameof(subDeviceID));
            if (subDevice == SubDevice.Broadcast)
                throw new ArgumentException("SubDeviceID must not be Broadcast", nameof(subDeviceID));

            return subDevice;
        }
    }
}