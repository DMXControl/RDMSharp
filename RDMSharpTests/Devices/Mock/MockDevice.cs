
namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDevice : AbstractMockDevice
    {
        public MockDevice(UID uid) : base(uid)
        {
        }

        protected sealed override AbstractMockSubDevice createSubDevice(UID uid, SubDevice subDevice)
        {
            return new MockSubDevice(uid, subDevice);
        }

        protected override void OnDispose()
        {
        }
    }
    internal sealed class MockSubDevice : AbstractMockSubDevice
    {
        public MockSubDevice(UID uid, SubDevice subDevice) : base(uid, subDevice)
        {
        }

        protected override void OnDispose()
        {
        }
    }
}
