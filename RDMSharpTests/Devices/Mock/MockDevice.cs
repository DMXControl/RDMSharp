﻿
namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDevice : AbstractMockDevice
    {
        public MockDevice(UID uid, bool imitateRealConditions) : base(uid, _imitateRealConditions:imitateRealConditions)
        {
        }

        protected sealed override AbstractMockSubDevice createSubDevice(UID uid, SubDevice subDevice)
        {
            return new MockSubDevice(uid, subDevice, this.ImitateRealConditions);
        }

        protected override void OnDispose()
        {
        }
    }
    internal sealed class MockSubDevice : AbstractMockSubDevice
    {
        public MockSubDevice(UID uid, SubDevice subDevice, bool imitateRealConditions) : base(uid, subDevice, imitateRealConditions)
        {
        }

        protected override void OnDispose()
        {
        }
    }
}
