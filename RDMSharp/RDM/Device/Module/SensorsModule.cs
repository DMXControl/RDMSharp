using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class SensorsModule : AbstractModule
    {
        private readonly ConcurrentDictionary<byte, Sensor> sensors = new ConcurrentDictionary<byte, Sensor>();
        public IReadOnlyDictionary<byte, Sensor> Sensors
        {
            get
            {
                return sensors.AsReadOnly();
            }
        }

        private ConcurrentDictionary<object, object> sensorDef = new ConcurrentDictionary<object, object>();
        private ConcurrentDictionary<object, object> sensorValue = new ConcurrentDictionary<object, object>();
        private ConcurrentDictionary<object, object> sensorUnit;
        private ConcurrentDictionary<object, object> sensorType;

        public SensorsModule(params Sensor[] sensors) : base(
            "Sensors",
            getSupportedParameters(sensors))
        {
            if (sensors is null)
                throw new ArgumentNullException(nameof(sensors), "Sensors cannot be null.");
            if (sensors.Length == 0)
                throw new ArgumentException("Sensors cannot be empty.", nameof(sensors));

            foreach (var sensor in sensors)
            {
                if (!this.sensors.TryAdd(sensor.SensorId, sensor))
                    throw new ArgumentException($"Sensor with ID {sensor.SensorId} already exists in the SensorsModule.");

                if (!sensorDef.TryAdd(sensor.SensorId, (RDMSensorDefinition)sensor))
                    throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorDefinition)}");

                if (!sensorValue.TryAdd(sensor.SensorId, (RDMSensorValue)sensor))
                    throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorValue)}");

                if (sensor.CustomUnit is not null)
                {
                    if(sensorUnit is null)
                        sensorUnit = new ConcurrentDictionary<object, object>();
                    sensorUnit.TryAdd(sensor.SensorId, sensor.CustomUnit);
                }
                if (sensor.CustomType is not null)
                {
                    if (sensorType is null)
                        sensorType = new ConcurrentDictionary<object, object>();
                    sensorType.TryAdd(sensor.SensorId, sensor.CustomType);
                }

                sensor.PropertyChanged += Sensor_PropertyChanged;
            }
        }

        private static ERDM_Parameter[] getSupportedParameters(Sensor[] sensors)
        {
            List<ERDM_Parameter> supportedParameters = new List<ERDM_Parameter>
            {
                ERDM_Parameter.SENSOR_DEFINITION,
                ERDM_Parameter.SENSOR_VALUE
            };
            if (sensors.Any(s => s.RecordedValueSupported))
                supportedParameters.Add(ERDM_Parameter.RECORD_SENSORS);

            if (sensors.Any(s => ((byte)s.Unit) >= 0x80))
                supportedParameters.Add(ERDM_Parameter.SENSOR_UNIT_CUSTOM);

            if (sensors.Any(s => ((byte)s.Type) >= 0x80))
                supportedParameters.Add(ERDM_Parameter.SENSOR_TYPE_CUSTOM);

            return supportedParameters.ToArray();
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef);
            this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);
            if (sensorUnit is not null)
                this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_UNIT_CUSTOM, sensorUnit);
            if (sensorType is not null)
                this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_TYPE_CUSTOM, sensorType);
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
        }
        private void Sensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Sensor sensor)
                return;

            switch (e.PropertyName)
            {
                case nameof(Sensor.Type):
                case nameof(Sensor.Unit):
                case nameof(Sensor.Prefix):
                case nameof(Sensor.RangeMaximum):
                case nameof(Sensor.RangeMinimum):
                case nameof(Sensor.NormalMaximum):
                case nameof(Sensor.NormalMinimum):
                case nameof(Sensor.LowestHighestValueSupported):
                case nameof(Sensor.RecordedValueSupported):
                    sensorDef.AddOrUpdate(sensor.SensorId, (RDMSensorDefinition)sensor, (o1, o2) => (RDMSensorDefinition)sensor);
                    this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef, sensor.SensorId);
                    OnPropertyChanged(nameof(Sensors));
                    break;
                case nameof(Sensor.PresentValue):
                case nameof(Sensor.LowestValue):
                case nameof(Sensor.HighestValue):
                case nameof(Sensor.RecordedValue):
                    sensorValue.AddOrUpdate(sensor.SensorId, (RDMSensorValue)sensor, (o1, o2) => (RDMSensorValue)sensor);
                    this.ParentDevice.setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue, sensor.SensorId);
                    OnPropertyChanged(nameof(Sensors));
                    break;
            }
        }
    }
}