namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockDevice : AbstractRemoteRDMDevice
    {
        public AbstractMockDevice(UID uid, SubDevice? subDevice = null, bool _imitateRealConditions = false) : base(uid, subDevice)
        {
        }
        protected sealed override void onDispose()
        {
            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }
        }
#pragma warning disable CS0114
        protected abstract void OnDispose();
#pragma warning restore CS0114
    }

    internal abstract class AbstractMockSubDevice : AbstractMockDevice, IRDMRemoteSubDevice
    {
        protected AbstractMockSubDevice(UID uid, SubDevice subDevice, bool _imitateRealConditions = false) : base(uid, subDevice, _imitateRealConditions)
        {
        }
        protected sealed override AbstractMockSubDevice createSubDevice(UID uid, SubDevice subDevice)
        {
            throw new NotSupportedException();
        }
    }
}