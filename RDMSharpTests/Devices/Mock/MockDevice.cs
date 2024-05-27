namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDevice : AbstractMockDevice
    {
        public MockDevice(UID uid, bool imitateRealConditions) : base(uid, imitateRealConditions)
        {
        }
    }
}
