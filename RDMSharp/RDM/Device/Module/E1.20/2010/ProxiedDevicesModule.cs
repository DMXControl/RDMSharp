using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.RDM.Device.Module;

public sealed class ProxiedDevicesModule : AbstractModule
{
    private const string _moduleName = "ProxiedDevices";
    private const string _moduleDisplayName = "Proxied Devices";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.PROXIED_DEVICES,
        ERDM_Parameter.PROXIED_DEVICES_COUNT
    };

    public override string DisplayName => _moduleDisplayName;

    public IReadOnlyCollection<UID> DeviceUIDs
    {
        get
        {
            IReadOnlyCollection<UID> uidList = null;
            if (this.ParentGeneratedDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PROXIED_DEVICES, out object proxiedDevices))
            {
                if (proxiedDevices is RDMProxiedDevices obj)
                    uidList = obj.Devices.ToList().AsReadOnly();
            }
            return uidList;
        }
    }
    private ConcurrentDictionary<UID, ConcurrentQueue<RDMMessage>> proxiedDevicesOngoingTransaktions = new ConcurrentDictionary<UID, ConcurrentQueue<RDMMessage>>();
    public ProxiedDevicesModule() : base(
        _moduleName,
        _moduleParameters)
    {
    }
    public ProxiedDevicesModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES, new RDMProxiedDevices());
        this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES_COUNT, new RDMProxiedDeviceCount(0, false));
    }

    protected override async void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        switch (parameter)
        {
            case ERDM_Parameter.PROXIED_DEVICES:
                OnPropertyChanged(nameof(DeviceUIDs));
                break;

            case ERDM_Parameter.PROXIED_DEVICES_COUNT:
                if (newValue is RDMProxiedDeviceCount proxiedDeviceCount && proxiedDeviceCount.ListChange)
                    await this.ParentRemoteDevice.RequestParameter(ERDM_Command.GET_COMMAND, ERDM_Parameter.PROXIED_DEVICES);
                break;
        }
    }
    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        switch (parameter)
        {
            case ERDM_Parameter.PROXIED_DEVICES:
            case ERDM_Parameter.PROXIED_DEVICES_COUNT:
                return true; // These parameters are handled by this module.
        }
        return false; // Default case, not handled by this module.
    }

    protected override RDMMessage handleRequest(RDMMessage message)
    {
        switch (message.Parameter)
        {
            case ERDM_Parameter.PROXIED_DEVICES when message.Command is ERDM_Command.GET_COMMAND:
                if (!proxiedDevicesOngoingTransaktions.ContainsKey(message.SourceUID))
                {
                    var devices = DeviceUIDs;
                    var chunks = devices.Chunk(38);
                    ConcurrentQueue<RDMMessage> queue = new ConcurrentQueue<RDMMessage>();
                    byte messageCounter = (byte)chunks.Count();
                    foreach (var chunk in chunks)
                    {
                        messageCounter--;
                        queue.Enqueue(new RDMMessage()
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            Command = ERDM_Command.GET_COMMAND_RESPONSE,
                            Parameter = message.Parameter,
                            ControllerFlags_or_MessageCounter = messageCounter,
                            ParameterData = new RDMProxiedDevices(chunk).ToPayloadData()
                        });
                    }
                    proxiedDevicesOngoingTransaktions.TryAdd(message.SourceUID, queue);
                }
                proxiedDevicesOngoingTransaktions.TryGetValue(message.SourceUID, out ConcurrentQueue<RDMMessage> msgQueue);
                if (msgQueue != null && msgQueue.TryDequeue(out RDMMessage responseMessage))
                {
                    if (msgQueue.IsEmpty)
                    {
                        proxiedDevicesOngoingTransaktions.TryRemove(message.SourceUID, out _);
                        this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES_COUNT, new RDMProxiedDeviceCount((ushort)DeviceUIDs.Count, false));
                    }
                    return responseMessage;
                }
                break;

            case ERDM_Parameter.PROXIED_DEVICES_COUNT when message.Command is ERDM_Command.GET_COMMAND:
                return new RDMMessage()
                {
                    SourceUID = message.DestUID,
                    DestUID = message.SourceUID,
                    Command = ERDM_Command.GET_COMMAND_RESPONSE,
                    Parameter = message.Parameter,
                    ParameterData = (this.ParentGeneratedDevice.GetAllParameterValues()[ERDM_Parameter.PROXIED_DEVICES_COUNT] as RDMProxiedDeviceCount).ToPayloadData()
                };
        }
        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
        {
            SourceUID = message.DestUID,
            DestUID = message.SourceUID,
            Command = message.Command | ERDM_Command.RESPONSE,
            Parameter = message.Parameter
        };
    }

    public void AddProxiedDevices(params UID[] deviceUIDs)
    {
        var currentDevices = this.ParentGeneratedDevice.GetAllParameterValues()[ERDM_Parameter.PROXIED_DEVICES] as RDMProxiedDevices;
        var devicesList = currentDevices.Devices.ToList();
        bool changed = false;
        foreach (var deviceUID in deviceUIDs)
        {
            if (!devicesList.Contains(deviceUID))
            {
                devicesList.Add(deviceUID);
                changed = true;
            }
        }
        if (changed)
        {
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES, new RDMProxiedDevices(devicesList.ToArray()));
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES_COUNT, new RDMProxiedDeviceCount((ushort)devicesList.Count, true));
            OnPropertyChanged(nameof(DeviceUIDs));
        }
    }
    public void RemoveProxiedDevices(params UID[] deviceUIDs)
    {
        var currentDevices = this.ParentGeneratedDevice.GetAllParameterValues()[ERDM_Parameter.PROXIED_DEVICES] as RDMProxiedDevices;
        var devicesList = currentDevices.Devices.ToList();
        bool changed = false;
        foreach (var deviceUID in deviceUIDs)
        {
            if (devicesList.Contains(deviceUID))
            {
                devicesList.Remove(deviceUID);
                changed = true;
            }
        }
        if (changed)
        {
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES, new RDMProxiedDevices(devicesList.ToArray()));
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PROXIED_DEVICES_COUNT, new RDMProxiedDeviceCount((ushort)devicesList.Count, true));
            OnPropertyChanged(nameof(DeviceUIDs));
        }
    }
}