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

        private static GeneratedPersonality[] PERSONALITYS = [
            new GeneratedPersonality(1, "5CH RGB",
                new Slot(0){ Category = ERDM_SlotCategory.INTENSITY, Description = "Dimmer" },
                new Slot(1){ Category = ERDM_SlotCategory.STROBE, Description = "Strobe" , DefaultValue= 33},
                new Slot(2){ Category = ERDM_SlotCategory.COLOR_ADD_RED, Description = "Red" },
                new Slot(3){ Category = ERDM_SlotCategory.COLOR_ADD_GREEN, Description = "Green" },
                new Slot(4){ Category = ERDM_SlotCategory.COLOR_ADD_BLUE, Description = "Blue" }),
            new GeneratedPersonality(2, "8CH RGBAWY",
                new Slot(0){ Category = ERDM_SlotCategory.INTENSITY, Description = "Dimmer" },
                new Slot(1){ Category = ERDM_SlotCategory.STROBE, Description = "Strobe" , DefaultValue= 33},
                new Slot(2){ Category = ERDM_SlotCategory.COLOR_ADD_RED, Description = "Red" },
                new Slot(3){ Category = ERDM_SlotCategory.COLOR_ADD_GREEN, Description = "Green" },
                new Slot(4){ Category = ERDM_SlotCategory.COLOR_ADD_BLUE, Description = "Blue" },
                new Slot(5){ Category = ERDM_SlotCategory.COLOR_CORRECTION, Description = "Amber" },
                new Slot(6){ Category = ERDM_SlotCategory.COLOR_CORRECTION, Description = "White" },
                new Slot(7){ Category = ERDM_SlotCategory.COLOR_CORRECTION, Description = "Yellow" }),
            new GeneratedPersonality(3, "9CH RGB 16-Bit",
                new Slot(0){ Category = ERDM_SlotCategory.INTENSITY, Description = "Dimmer" },
                new Slot(1){ Category = ERDM_SlotCategory.INTENSITY, Description = "Dimmer Fine" , Type= ERDM_SlotType.SEC_FINE},
                new Slot(2){ Category = ERDM_SlotCategory.STROBE, Description = "Strobe" , DefaultValue= 33},
                new Slot(3){ Category = ERDM_SlotCategory.COLOR_ADD_RED, Description = "Red" },
                new Slot(4){ Category = ERDM_SlotCategory.COLOR_ADD_RED, Description = "Red Fine" , Type= ERDM_SlotType.SEC_FINE},
                new Slot(5){ Category = ERDM_SlotCategory.COLOR_ADD_GREEN, Description = "Green" },
                new Slot(6){ Category = ERDM_SlotCategory.COLOR_ADD_GREEN, Description = "Green Fine" , Type= ERDM_SlotType.SEC_FINE},
                new Slot(7){ Category = ERDM_SlotCategory.COLOR_ADD_BLUE, Description = "Blue" },
                new Slot(8){ Category = ERDM_SlotCategory.COLOR_ADD_BLUE, Description = "Blue Fine" , Type= ERDM_SlotType.SEC_FINE})];
        public override GeneratedPersonality[] Personalities => PERSONALITYS;
        public MockGeneratedDevice1(RDMUID uid) : base(uid, [ERDM_Parameter.IDENTIFY_DEVICE, ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL], "Dummy Manufacturer 9FFF")
        {
            this.DeviceLabel = "Dummy Device 1";
            this.TrySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, false);
            this.TrySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, $"Dummy Software");
        }
    }
}
