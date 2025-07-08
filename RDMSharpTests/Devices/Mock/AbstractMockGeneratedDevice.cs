using RDMSharp.RDM.Device.Module;

namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockGeneratedDevice : AbstractGeneratedRDMDevice
    {

        public AbstractMockGeneratedDevice(UID uid, ERDM_Parameter[] parameters, Sensor[]? sensors = null, IRDMDevice[]? subDevices = null, IReadOnlyCollection<IModule> mudules = null) : base(uid, parameters, sensors: sensors, subDevices: subDevices, modules: mudules)
        {
        }
        public AbstractMockGeneratedDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, Sensor[]? sensors = null, IReadOnlyCollection<IModule> mudules = null) : base(uid, subDevice, parameters, sensors: sensors, modules: mudules)
        {
        }
        internal RDMMessage? ProcessRequestMessage_Internal(RDMMessage request)
        {
            return base.processRequestMessage(request);
        }

        internal new void AddStatusMessage(RDMStatusMessage statusMessage)
        {
            base.AddStatusMessage(statusMessage);
        }
        internal new void ClearStatusMessage(RDMStatusMessage statusMessage)
        {
            base.ClearStatusMessage(statusMessage);
        }
        internal new void RemoveStatusMessage(RDMStatusMessage statusMessage)
        {
            base.RemoveStatusMessage(statusMessage);
        }
        internal new void AddSensors(params Sensor[] sensor)
        {
            base.AddSensors(sensor);
        }
        internal new void RemoveSensors(params Sensor[] sensor)
        {
            base.RemoveSensors(sensor);
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
