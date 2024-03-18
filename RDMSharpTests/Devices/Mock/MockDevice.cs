namespace RDMSharpTests.Devices.Mock
{
    internal sealed class MockDevice : AbstractMockDevice
    {
        public MockDevice(RDMUID uid, bool imitateRealConditions) : base(uid, imitateRealConditions)
        {
        }
    }
}
