﻿namespace RDMSharpTests.Devices.Mock
{
    internal abstract class AbstractMockGeneratedDevice : AbstractGeneratedRDMDevice
    {


        public new string SoftwareVersionLabel
        {
            get
            {
                return base.SoftwareVersionLabel;
            }
            internal set
            {
                base.SoftwareVersionLabel = value;
            }
        }


        public new string BootSoftwareVersionLabel
        {
            get
            {
                return base.BootSoftwareVersionLabel;
            }
            internal set
            {
                base.BootSoftwareVersionLabel = value;
            }
        }
        public AbstractMockGeneratedDevice(UID uid, ERDM_Parameter[] parameters, string manufacturer, Sensor[]? sensors = null, IRDMDevice[]? subDevices = null) : base(uid, parameters, manufacturer, sensors: sensors, subDevices: subDevices)
        {
        }
        public AbstractMockGeneratedDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, string manufacturer, Sensor[]? sensors = null) : base(uid, subDevice, parameters, manufacturer, sensors: sensors)
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
