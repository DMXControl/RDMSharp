using RDMSharp.ParameterWrapper;

namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockGeneratedDevice1 : AbstractMockGeneratedDevice
    {
        public override EManufacturer ManufacturerID => (EManufacturer)0x9fff;
        public override ushort DeviceModelID => 20;
        public override ERDM_ProductCategoryCoarse ProductCategoryCoarse => ERDM_ProductCategoryCoarse.CONTROL;
        public override ERDM_ProductCategoryFine ProductCategoryFine => ERDM_ProductCategoryFine.DATA_CONVERSION;
        public override uint SoftwareVersionID => 0x1234;
        public override string DeviceModelDescription => "Test Model Description";
        public override bool SupportDMXAddress => true;

        private static GeneratedPersonality[] PERSONALITYS = [new GeneratedPersonality(1, 5, "5CH RGB"), new GeneratedPersonality(2, 8, "8CH RGBAWY"), new GeneratedPersonality(3, 9, "9CH RGB 16-Bit")];
        public override GeneratedPersonality[] Personalities => PERSONALITYS;
        public MockGeneratedDevice1(RDMUID uid) : base(uid, [ERDM_Parameter.IDENTIFY_DEVICE, ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL],"Dummy Manufacturer 9FFF")
        {
            this.DeviceLabel = "Dummy Device 1";
            this.TrySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, false);
            this.TrySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, $"Dummy Software");
        }
    }
}
