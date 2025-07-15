using RDMSharp.RDM.Device.Module;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockGeneratedDevice : AbstractGeneratedRDMDevice
    {

        public AbstractMockGeneratedDevice(UID uid, ERDM_Parameter[] parameters, IRDMDevice[]? subDevices = null, IReadOnlyCollection<IModule> mudules = null) : base(uid, parameters, subDevices: subDevices, modules: mudules)
        {
        }
        public AbstractMockGeneratedDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, IReadOnlyCollection<IModule> mudules = null) : base(uid, subDevice, parameters, modules: mudules)
        {
        }
        internal RDMMessage? ProcessRequestMessage_Internal(RDMMessage request)
        {
            return base.processRequestMessage(request);
        }

        internal void AddStatusMessage(RDMStatusMessage statusMessage)
        {
            base.statusMessageModule?.AddStatusMessage(statusMessage);
        }
        internal void ClearStatusMessage(RDMStatusMessage statusMessage)
        {
            base.statusMessageModule?.ClearStatusMessage(statusMessage);
        }
        internal void RemoveStatusMessage(RDMStatusMessage statusMessage)
        {
            base.statusMessageModule?.RemoveStatusMessage(statusMessage);
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
}
