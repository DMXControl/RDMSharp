using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class EndpointsModule : AbstractModule
{
    private const string _moduleName = "Endpoints";
    private const string _moduleDisplayName = "Endpoints";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.ENDPOINT_LIST,
        ERDM_Parameter.ENDPOINT_LIST_CHANGE,
        ERDM_Parameter.IDENTIFY_ENDPOINT,
        ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
        ERDM_Parameter.ENDPOINT_MODE,
        ERDM_Parameter.ENDPOINT_LABEL,
        ERDM_Parameter.RDM_TRAFFIC_ENABLE,
        ERDM_Parameter.DISCOVERY_STATE,
        ERDM_Parameter.BACKGROUND_DISCOVERY,
        ERDM_Parameter.ENDPOINT_TIMING,
        ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
        ERDM_Parameter.ENDPOINT_RESPONDERS,
        ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
        ERDM_Parameter.BINDING_CONTROL_FIELDS,
        ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
        ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION
    };

    public override string DisplayName => _moduleDisplayName;

    private ConcurrentDictionary<ushort, Endpoint> _endpoints;
    public IReadOnlyCollection<Endpoint> Endpoints
    {
        get
        {
            return _endpoints.Values.ToList();
        }
    }

    internal void AddEndpoint(Endpoint endpoint)
    {
        if (_endpoints.ContainsKey(endpoint.EndpointId))
            throw new ArgumentException($"An endpoint with the ID {endpoint.EndpointId} already exists.");
        if (_endpoints.TryAdd(endpoint.EndpointId, endpoint))
        {
            ListChanged++;
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST, new GetEndpointListResponse(listChanged, _endpoints.Values.Select(ep => new EndpointDescriptor(ep.EndpointId, ep.Type)).ToArray()));
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST_CHANGE, ListChanged);
        }
    }
    internal void RemoveEndpoint(ushort endpointId)
    {
        if (!_endpoints.ContainsKey(endpointId))
            throw new ArgumentException($"No endpoint with the ID {endpointId} exists.");
        if (_endpoints.TryRemove(endpointId, out _))
        {
            ListChanged++;
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST, new GetEndpointListResponse(listChanged, _endpoints.Values.Select(ep => new EndpointDescriptor(ep.EndpointId, ep.Type)).ToArray()));
            this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST_CHANGE, ListChanged);
        }
    }

    private uint listChanged;
    public uint ListChanged
    {
        get { return listChanged; }
        private set
        {
            if (listChanged == value)
                return;
            listChanged = value;
            OnPropertyChanged(nameof(ListChanged));
        }
    }

    private byte _backgroundQueuedStatusPolicy;
    public byte BackgroundQueuedStatusPolicy
    {
        get { return _backgroundQueuedStatusPolicy; }
        set
        {
            if (_backgroundQueuedStatusPolicy == value)
                return;
            _backgroundQueuedStatusPolicy = value;
            ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY, new GetBackgroundQueuedStatusPolicyResponse(this._backgroundQueuedStatusPolicy, (byte)this.backgroundQueueStatusPolicyDescriptionDict.Count));
            OnPropertyChanged(nameof(BackgroundQueuedStatusPolicy));
        }
    }
    private IDictionary<byte, string> _backgroundQueuedStatusPolicyDescriptions;
    public IReadOnlyDictionary<byte, string> BackgroundQueuedStatusPolicyDescriptions
    {
        get { return _backgroundQueuedStatusPolicyDescriptions.AsReadOnly(); }
    }
    private IDictionary<byte, string> _timingDescriptions;
    public IReadOnlyDictionary<byte, string> TimingDescriptions
    {
        get { return _timingDescriptions.AsReadOnly(); }
    }

    private ConcurrentDictionary<object, object> identifyDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> universeDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> modeDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> lableDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> rdmTraficDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> discoveryStateDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> backgroundDiscoveryDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> timingDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> respondersDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> responderListChangedDict = new ConcurrentDictionary<object, object>();

    private ConcurrentDictionary<object, object> timingDescriptionDict = new ConcurrentDictionary<object, object>();
    private ConcurrentDictionary<object, object> backgroundQueueStatusPolicyDescriptionDict = new ConcurrentDictionary<object, object>();

    public EndpointsModule(
        byte backgroundQueuedStatusPolicy,
        IDictionary<byte, string> backgroundQueuedStatusPolicyDescriptions,
        IDictionary<byte, string> timingDescriptions,
        IReadOnlyCollection<Endpoint> endpoints) : base(
        _moduleName,
        _moduleParameters)
    {
        _backgroundQueuedStatusPolicy = backgroundQueuedStatusPolicy;
        if (backgroundQueuedStatusPolicyDescriptions.Count > byte.MaxValue)
            throw new IndexOutOfRangeException($"Maximum lenght of {nameof(backgroundQueuedStatusPolicyDescriptions)} is {byte.MaxValue}");
        _backgroundQueuedStatusPolicyDescriptions = backgroundQueuedStatusPolicyDescriptions;

        if (timingDescriptions.Count > byte.MaxValue)
            throw new IndexOutOfRangeException($"Maximum lenght of {nameof(timingDescriptions)} is {byte.MaxValue}");
        _timingDescriptions = timingDescriptions;

        _endpoints = new ConcurrentDictionary<ushort, Endpoint>();

        if (endpoints.Any(ep => ep.EndpointId == 0))
            throw new ArgumentOutOfRangeException(nameof(endpoints));

        foreach (var epoint in endpoints)
            _endpoints.TryAdd(epoint.EndpointId, epoint);
    }
    public EndpointsModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
        _endpoints = new ConcurrentDictionary<ushort, Endpoint>();
        _backgroundQueuedStatusPolicyDescriptions = new Dictionary<byte, string>();
        _timingDescriptions = new Dictionary<byte, string>();

        fillFromRemoteCache();
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {

        foreach (var epoint in _endpoints.Values)
        {
            if (epoint.Identify.HasValue)
                identifyDict.TryAdd(epoint.EndpointId, new GetSetIdentifyEndpoint(epoint.EndpointId, epoint.Identify.Value));

            if (epoint.Universe.HasValue)
                universeDict.TryAdd(epoint.EndpointId, new GetSetEndpointToUniverse(epoint.EndpointId, epoint.Universe.Value));

            if (epoint.Mode.HasValue)
                modeDict.TryAdd(epoint.EndpointId, new GetSetEndpointMode(epoint.EndpointId, epoint.Mode.Value));

            if (epoint.Lable != null)
                lableDict.TryAdd(epoint.EndpointId, new GetSetEndpointLabel(epoint.EndpointId, epoint.Lable));

            if (epoint.RDMTraffic.HasValue)
                rdmTraficDict.TryAdd(epoint.EndpointId, new GetSetEndpointRDMTrafficEnable(epoint.EndpointId, epoint.RDMTraffic.Value));

            if (!epoint.DiscoveryState.HasValue && epoint.DiscoveryStateCount.HasValue)
                discoveryStateDict.TryAdd(epoint.EndpointId, new GetDiscoveryStateResponse(epoint.EndpointId, epoint.DiscoveryStateCount.Value, epoint.DiscoveryState.Value));

            if (!epoint.BackgroundDiscovery.HasValue)
                backgroundDiscoveryDict.TryAdd(epoint.EndpointId, new GetSetEndpointBackgroundDiscovery(epoint.EndpointId, epoint.BackgroundDiscovery.Value));

            if (epoint.Timing.HasValue)
                timingDict.TryAdd(epoint.EndpointId, new GetEndpointTimingResponse(epoint.EndpointId, epoint.Timing.Value, (byte)this.TimingDescriptions.Count));
            respondersDict.TryAdd(epoint.EndpointId, new GetEndpointRespondersResponse(epoint.EndpointId, epoint.ResponderListChanged, epoint.Responders.ToArray()));
            responderListChangedDict.TryAdd(epoint.EndpointId, new GetEndpointResponderListChangeResponse(epoint.EndpointId, epoint.ResponderListChanged));
            epoint.PropertyChanged += Endpoint_PropertyChanged;
        }

        foreach (var bDisc in _backgroundQueuedStatusPolicyDescriptions)
            backgroundQueueStatusPolicyDescriptionDict.TryAdd(bDisc.Key, new GetBackgroundQueuedStatusPolicyDescriptionResponse(bDisc.Key, bDisc.Value));

        foreach (var tDisc in _timingDescriptions)
            timingDescriptionDict.TryAdd(tDisc.Key, new GetEndpointTimingDescriptionResponse(tDisc.Key, tDisc.Value));

        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IDENTIFY_ENDPOINT, identifyDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_TO_UNIVERSE, universeDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_MODE, modeDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LABEL, lableDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.RDM_TRAFFIC_ENABLE, rdmTraficDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DISCOVERY_STATE, discoveryStateDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BACKGROUND_DISCOVERY, backgroundDiscoveryDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_TIMING, timingDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_RESPONDERS, respondersDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, responderListChangedDict);

        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION, backgroundQueueStatusPolicyDescriptionDict);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION, timingDescriptionDict);

        //ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BINDING_CONTROL_FIELDS, null); // Handled dynamically in handleRequest, because of two identifiers (endpoint and responder UID)
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY, new GetBackgroundQueuedStatusPolicyResponse(this._backgroundQueuedStatusPolicy, (byte)this.backgroundQueueStatusPolicyDescriptionDict.Count));

        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST, new GetEndpointListResponse(listChanged, _endpoints.Values.Select(ep => new EndpointDescriptor(ep.EndpointId, ep.Type)).ToArray()));
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LIST_CHANGE, listChanged);
    }
    private void fillFromRemoteCache()
    {
        try
        {
            var values = ParentRemoteDevice.GetAllParameterValues();

            object value = null;

            if (values.TryGetValue(ERDM_Parameter.IDENTIFY_ENDPOINT, out value) && value is ConcurrentDictionary<object, object> _identifyDict)
                identifyDict = _identifyDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_TO_UNIVERSE, out value) && value is ConcurrentDictionary<object, object> _universeDict)
                universeDict = _universeDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_MODE, out value) && value is ConcurrentDictionary<object, object> _modeDict)
                modeDict = _modeDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_LABEL, out value) && value is ConcurrentDictionary<object, object> _lableDict)
                lableDict = _lableDict;

            if (values.TryGetValue(ERDM_Parameter.RDM_TRAFFIC_ENABLE, out value) && value is ConcurrentDictionary<object, object> _rdmTraficDict)
                rdmTraficDict = _rdmTraficDict;

            if (values.TryGetValue(ERDM_Parameter.DISCOVERY_STATE, out value) && value is ConcurrentDictionary<object, object> _discoveryStateDict)
                discoveryStateDict = _discoveryStateDict;

            if (values.TryGetValue(ERDM_Parameter.BACKGROUND_DISCOVERY, out value) && value is ConcurrentDictionary<object, object> _backgroundDiscoveryDict)
                backgroundDiscoveryDict = _backgroundDiscoveryDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_TIMING, out value) && value is ConcurrentDictionary<object, object> _timingDict)
                timingDict = _timingDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_RESPONDERS, out value) && value is ConcurrentDictionary<object, object> _respondersDict)
                respondersDict = _respondersDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, out value) && value is ConcurrentDictionary<object, object> _responderListChangedDict)
                responderListChangedDict = _responderListChangedDict;


            if (values.TryGetValue(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION, out value) && value is ConcurrentDictionary<object, object> _backgroundQueueStatusPolicyDescriptionDict)
                backgroundQueueStatusPolicyDescriptionDict = _backgroundQueueStatusPolicyDescriptionDict;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION, out value) && value is ConcurrentDictionary<object, object> _timingDescriptionDict)
                timingDescriptionDict = _timingDescriptionDict;

            if (values.TryGetValue(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY, out value) && value is GetBackgroundQueuedStatusPolicyResponse getBackgroundQueuedStatusPolicyResponse)
                this._backgroundQueuedStatusPolicy = getBackgroundQueuedStatusPolicyResponse.PolicyId;

            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_LIST_CHANGE, out value) && value is uint _listChanged)
                this.listChanged = _listChanged;


            bool backgroundQueueStatusPolicyDescriptionChanged = false;
            foreach (var backgroundQueueStatusPolicy in backgroundQueueStatusPolicyDescriptionDict)
            {
                byte key = (byte)backgroundQueueStatusPolicy.Key;
                if (!_backgroundQueuedStatusPolicyDescriptions.TryGetValue(key, out string description))
                {
                    _backgroundQueuedStatusPolicyDescriptions[key] = ((GetEndpointTimingDescriptionResponse)backgroundQueueStatusPolicy.Value).Description;
                    backgroundQueueStatusPolicyDescriptionChanged = true;
                }
                else if (!string.Equals(description, ((GetEndpointTimingDescriptionResponse)backgroundQueueStatusPolicy.Value).Description))
                {
                    _backgroundQueuedStatusPolicyDescriptions[key] = ((GetEndpointTimingDescriptionResponse)backgroundQueueStatusPolicy.Value).Description;
                    backgroundQueueStatusPolicyDescriptionChanged = true;
                }
            }

            bool timingDescriptionChanged = false;
            foreach (var timing in timingDescriptionDict)
            {
                byte key = (byte)timing.Key;
                if (!_timingDescriptions.TryGetValue(key, out string description))
                {
                    _timingDescriptions[key] = ((GetEndpointTimingDescriptionResponse)timing.Value).Description;
                    timingDescriptionChanged = true;
                }
                else if (!string.Equals(description, ((GetEndpointTimingDescriptionResponse)timing.Value).Description))
                {
                    _timingDescriptions[key] = ((GetEndpointTimingDescriptionResponse)timing.Value).Description;
                    timingDescriptionChanged = true;
                }
            }
            if (backgroundQueueStatusPolicyDescriptionChanged)
                OnPropertyChanged(nameof(BackgroundQueuedStatusPolicyDescriptions));
            if (timingDescriptionChanged)
                OnPropertyChanged(nameof(TimingDescriptions));


            if (values.TryGetValue(ERDM_Parameter.ENDPOINT_LIST, out value) && value is EndpointDescriptor[] endpointDescriptors)
                foreach (EndpointDescriptor endpointDescriptor in endpointDescriptors)
                {
                    ushort endpointId = endpointDescriptor.EndpointId;
                    RemoteEndpoint _endpoint = null;
                    if (!_endpoints.TryGetValue(endpointId, out Endpoint _epoint))
                    {
                        _endpoint = new RemoteEndpoint(this, endpointId, endpointDescriptor.EndpointType);
                        _endpoints.TryAdd(endpointId, _endpoint);
                    }

                    if (_epoint is RemoteEndpoint)
                        _endpoint = (RemoteEndpoint)_epoint;

                    if (identifyDict.TryGetValue(endpointId, out object identifyValue) && identifyValue is GetSetIdentifyEndpoint getSetIdentifyEndpoint)
                        _endpoint.Identify = getSetIdentifyEndpoint.IdentifyState;

                    if (universeDict.TryGetValue(endpointId, out object universeValue) && universeValue is GetSetEndpointToUniverse getSetEndpointToUniverse)
                        _endpoint.Universe = getSetEndpointToUniverse.Universe;

                    if (modeDict.TryGetValue(endpointId, out object modeValue) && modeValue is GetSetEndpointMode getSetEndpointMode)
                        _endpoint.Mode = getSetEndpointMode.EndpointMode;

                    if (lableDict.TryGetValue(endpointId, out object lableValue) && lableValue is GetSetEndpointLabel getSetEndpointLabel)
                        _endpoint.Lable = getSetEndpointLabel.EndpointLabel;

                    if (rdmTraficDict.TryGetValue(endpointId, out object rdmTraficValue) && rdmTraficValue is GetSetEndpointRDMTrafficEnable getSetEndpointRDMTrafficEnable)
                        _endpoint.RDMTraffic = getSetEndpointRDMTrafficEnable.RDMTrafficEnabled;

                    if (discoveryStateDict.TryGetValue(endpointId, out object discoveryStateValue) && discoveryStateValue is GetDiscoveryStateResponse getDiscoveryStateResponse)
                        _endpoint.DiscoveryState = getDiscoveryStateResponse.DiscoveryState;

                    if (backgroundDiscoveryDict.TryGetValue(endpointId, out object backgroundDiscoveryValue) && backgroundDiscoveryValue is GetSetEndpointBackgroundDiscovery getSetEndpointBackgroundDiscovery)
                        _endpoint.BackgroundDiscovery = getSetEndpointBackgroundDiscovery.BackgroundDiscovery;

                    if (timingDict.TryGetValue(endpointId, out object timingValue) && timingValue is GetEndpointTimingResponse getEndpointTimingResponse)
                        _endpoint.Timing = getEndpointTimingResponse.TimingId;

                    if (respondersDict.TryGetValue(endpointId, out object respondersValue) && respondersValue is GetEndpointRespondersResponse getEndpointRespondersResponse)
                        _endpoint.Responders = getEndpointRespondersResponse.UIDs;

                    if (responderListChangedDict.TryGetValue(endpointId, out object responderListChangedValue) && responderListChangedValue is GetEndpointResponderListChangeResponse getEndpointResponderListChangeResponse)
                        _endpoint.ResponderListChanged = getEndpointResponderListChangeResponse.ListChangeNumber;
                }
        }
        catch (Exception e)
        {
            Logger?.LogError(e);
        }
    }

    private void Endpoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Endpoint endpoint = (Endpoint)sender;

        switch (e.PropertyName)
        {
            case (nameof(Endpoint.Identify)):
                var newIdentifyValue = new GetSetIdentifyEndpoint(endpoint.EndpointId, endpoint.Identify.Value);
                this.identifyDict.AddOrUpdate(endpoint.EndpointId, (_) => newIdentifyValue, (_, _) => newIdentifyValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.IDENTIFY_ENDPOINT, this.identifyDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.Universe)):
                var newUniverseValue = new GetSetEndpointToUniverse(endpoint.EndpointId, endpoint.Universe.Value);
                this.universeDict.AddOrUpdate(endpoint.EndpointId, (_) => newUniverseValue, (_, _) => newUniverseValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_TO_UNIVERSE, this.universeDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.Mode)):
                var newModeValue = new GetSetEndpointMode(endpoint.EndpointId, endpoint.Mode.Value);
                this.modeDict.AddOrUpdate(endpoint.EndpointId, (_) => newModeValue, (_, _) => newModeValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_MODE, this.modeDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.Lable)):
                var newLableValue = new GetSetEndpointLabel(endpoint.EndpointId, endpoint.Lable);
                this.lableDict.AddOrUpdate(endpoint.EndpointId, (_) => newLableValue, (_, _) => newLableValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_LABEL, this.lableDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.RDMTraffic)):
                var newRDMTrafficValue = new GetSetEndpointRDMTrafficEnable(endpoint.EndpointId, endpoint.RDMTraffic.Value);
                this.rdmTraficDict.AddOrUpdate(endpoint.EndpointId, (_) => newRDMTrafficValue, (_, _) => newRDMTrafficValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.RDM_TRAFFIC_ENABLE, this.rdmTraficDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.DiscoveryState)):
            case (nameof(Endpoint.DiscoveryStateCount)):
                var newDiscoveryStateValue = new GetDiscoveryStateResponse(endpoint.EndpointId, endpoint.DiscoveryStateCount.Value, endpoint.DiscoveryState.Value);
                this.discoveryStateDict.AddOrUpdate(endpoint.EndpointId, (_) => newDiscoveryStateValue, (_, _) => newDiscoveryStateValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DISCOVERY_STATE, this.discoveryStateDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.BackgroundDiscovery)):
                var newBackgroundDiscoveryValue = new GetSetEndpointBackgroundDiscovery(endpoint.EndpointId, endpoint.BackgroundDiscovery.Value);
                this.backgroundDiscoveryDict.AddOrUpdate(endpoint.EndpointId, (_) => newBackgroundDiscoveryValue, (_, _) => newBackgroundDiscoveryValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.BACKGROUND_DISCOVERY, this.backgroundDiscoveryDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.Timing)):
                var newTimingValue = new GetEndpointTimingResponse(endpoint.EndpointId, endpoint.Timing.Value, (byte)this.TimingDescriptions.Count);
                this.timingDescriptionDict.AddOrUpdate(endpoint.EndpointId, (_) => newTimingValue, (_, _) => newTimingValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_TIMING, this.timingDescriptionDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.Responders)):
                var newRespondersValue = new GetEndpointRespondersResponse(endpoint.EndpointId, endpoint.ResponderListChanged, endpoint.Responders.ToArray());
                this.respondersDict.AddOrUpdate(endpoint.EndpointId, (_) => newRespondersValue, (_, _) => newRespondersValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_RESPONDERS, this.respondersDict, endpoint.EndpointId);
                break;
            case (nameof(Endpoint.ResponderListChanged)):
                var newResponderListChangedValue = new GetEndpointResponderListChangeResponse(endpoint.EndpointId, endpoint.ResponderListChanged);
                this.responderListChangedDict.AddOrUpdate(endpoint.EndpointId, (_) => newResponderListChangedValue, (_, _) => newResponderListChangedValue);
                this.ParentGeneratedDevice.setParameterValue(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, this.responderListChangedDict, endpoint.EndpointId);
                break;
        }
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        RemoteEndpoint? endpoint = null;
        if (index is not null && index is ushort ui)
            endpoint = _endpoints.Values.FirstOrDefault(epoint => epoint.EndpointId == ui) as RemoteEndpoint;

        if (newValue is ConcurrentDictionary<object, object> dict && index is not null)
            newValue = dict.GetValueOrDefault(index);

        switch (parameter)
        {
            case ERDM_Parameter.ENDPOINT_LIST:
                foreach (EndpointDescriptor endpointDescriptor in ((GetEndpointListResponse)newValue).Endpoints)
                {
                    ushort endpointId = endpointDescriptor.EndpointId;
                    RemoteEndpoint _endpoint = null;
                    if (!_endpoints.TryGetValue(endpointId, out Endpoint _epoint))
                    {
                        _endpoint = new RemoteEndpoint(this, endpointId, endpointDescriptor.EndpointType);
                        _endpoints.TryAdd(endpointId, _endpoint);
                    }
                }
                OnPropertyChanged(nameof(Endpoints));
                break;

            case ERDM_Parameter.IDENTIFY_ENDPOINT:
                if (endpoint is not null)
                    endpoint.Identify = ((GetSetIdentifyEndpoint)newValue).IdentifyState;
                break;

            case ERDM_Parameter.ENDPOINT_TO_UNIVERSE:
                if (endpoint is not null)
                    endpoint.Universe = ((GetSetEndpointToUniverse)newValue).Universe;
                break;

            case ERDM_Parameter.ENDPOINT_MODE:
                if (endpoint is not null)
                    endpoint.Mode = ((GetSetEndpointMode)newValue).EndpointMode;
                break;

            case ERDM_Parameter.ENDPOINT_LABEL:
                if (endpoint is not null)
                    endpoint.Lable = ((GetSetEndpointLabel)newValue).EndpointLabel;
                break;

            case ERDM_Parameter.RDM_TRAFFIC_ENABLE:
                if (endpoint is not null)
                    endpoint.RDMTraffic = ((GetSetEndpointRDMTrafficEnable)newValue).RDMTrafficEnabled;
                break;

            case ERDM_Parameter.DISCOVERY_STATE:
                if (endpoint is not null)
                    endpoint.DiscoveryState = ((GetDiscoveryStateResponse)newValue).DiscoveryState;
                break;

            case ERDM_Parameter.BACKGROUND_DISCOVERY:
                if (endpoint is not null)
                    endpoint.BackgroundDiscovery = ((GetSetEndpointBackgroundDiscovery)newValue).BackgroundDiscovery;
                break;

            case ERDM_Parameter.ENDPOINT_TIMING:
                if (endpoint is not null)
                    endpoint.Timing = ((GetEndpointTimingResponse)newValue).TimingId;
                break;

            case ERDM_Parameter.ENDPOINT_RESPONDERS:
                if (endpoint is not null)
                    endpoint.Responders = ((GetEndpointRespondersResponse)newValue).UIDs;
                break;

            case ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE:
                if (endpoint is not null)
                    endpoint.ResponderListChanged = ((GetEndpointResponderListChangeResponse)newValue).ListChangeNumber;
                break;

            case ERDM_Parameter.ENDPOINT_LIST_CHANGE:
                listChanged = (uint)newValue;
                break;

            case ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY:
                BackgroundQueuedStatusPolicy = ((GetBackgroundQueuedStatusPolicyResponse)newValue).PolicyId;
                break;
        }
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        switch (message.Parameter)
        {
            case ERDM_Parameter.IDENTIFY_ENDPOINT when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_TO_UNIVERSE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_MODE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_LABEL when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.RDM_TRAFFIC_ENABLE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.DISCOVERY_STATE when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.BACKGROUND_DISCOVERY when message.Command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_TIMING when message.Command is ERDM_Command.SET_COMMAND:
                if (message.Value is null)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                if (message.PDL >= 3)
                {
                    byte[] parameterData = message.ParameterData.Take(2).ToArray();
                    ushort endpointId = message.ParameterData.Length > 0 ? Tools.DataToUShort(ref parameterData) : (ushort)0;

                    if (_endpoints.Values.FirstOrDefault(epoint => epoint.EndpointId == endpointId) is not Endpoint epoint)
                        return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                    try
                    {
#if DEBUG
                        if (message.SourceUID.Equals(new UID(0xeeee, 0xf0f0f0f0)))
                            throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                        switch (message.Parameter)
                        {
                            case ERDM_Parameter.IDENTIFY_ENDPOINT when message.Value is GetSetIdentifyEndpoint setIdentifyEndpoint:
                                epoint.Identify = setIdentifyEndpoint.IdentifyState;
                                break;

                            case ERDM_Parameter.ENDPOINT_TO_UNIVERSE when message.Value is GetSetEndpointToUniverse setEndpointToUniverse:
                                epoint.Universe = setEndpointToUniverse.Universe;
                                break;

                            case ERDM_Parameter.ENDPOINT_MODE when message.Value is GetSetEndpointMode setEndpointMode:
                                epoint.Mode = setEndpointMode.EndpointMode;
                                break;

                            case ERDM_Parameter.ENDPOINT_LABEL when message.Value is GetSetEndpointLabel setEndpointLabel:
                                epoint.Lable = setEndpointLabel.EndpointLabel;
                                break;

                            case ERDM_Parameter.RDM_TRAFFIC_ENABLE when message.Value is GetSetEndpointRDMTrafficEnable setEndpointRDMTrafficEnable:
                                epoint.RDMTraffic = setEndpointRDMTrafficEnable.RDMTrafficEnabled;
                                break;

                            case ERDM_Parameter.DISCOVERY_STATE when message.Value is SetDiscoveryStateRequest setDiscoveryState:
                                epoint.DiscoveryState = setDiscoveryState.DiscoveryState;
                                break;

                            case ERDM_Parameter.BACKGROUND_DISCOVERY when message.Value is GetSetEndpointBackgroundDiscovery setBackgroundDiscovery:
                                epoint.BackgroundDiscovery = setBackgroundDiscovery.BackgroundDiscovery;
                                break;

                            case ERDM_Parameter.ENDPOINT_TIMING when message.Value is SetEndpointTimingRequest setEndpointTiming:
                                epoint.Timing = setEndpointTiming.TimingId;
                                break;
                        }

                        return new RDMMessage()
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(ex);
                    }
                }
                break;
            case ERDM_Parameter.BINDING_CONTROL_FIELDS when message.Command is ERDM_Command.GET_COMMAND:
                if (message.Value is null || message.Value is not GetBindingAndControlFieldsRequest getBindingAndControlFieldsRequest)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.GET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                if (this.Endpoints.FirstOrDefault(ep => ep.EndpointId == getBindingAndControlFieldsRequest.EndpointId) is not Endpoint endpoint)
                    return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.GET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                GetBindingAndControlFieldsResponse payload = null;
                if (endpoint.BindingControlFields.TryGetValue(getBindingAndControlFieldsRequest.UID, out Endpoint.BindingControlField bindingControlField))
                    payload = new GetBindingAndControlFieldsResponse(endpoint.EndpointId, bindingControlField.Uid, bindingControlField.ControlField, bindingControlField.BindingUid);
                if (payload is null)
                    payload = new GetBindingAndControlFieldsResponse(endpoint.EndpointId, getBindingAndControlFieldsRequest.UID, 0, new UID(0, 0));
                return new RDMMessage()
                {
                    SourceUID = message.DestUID,
                    DestUID = message.SourceUID,
                    Command = ERDM_Command.GET_COMMAND_RESPONSE,
                    Parameter = message.Parameter,
                    ParameterData = Tools.ValueToData(payload)
                };

            case ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY when message.Command is ERDM_Command.SET_COMMAND:
                if (message.Value is null || message.Value is not byte policyId)
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                if (policyId >= this.backgroundQueueStatusPolicyDescriptionDict.Count)
                    return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                try
                {
#if DEBUG
                    if (message.SourceUID.Equals(new UID(0xeeee, 0xf0f0f0f0)))
                        throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                    this.BackgroundQueuedStatusPolicy = policyId;
                    return new RDMMessage()
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };


                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex);
                }
                break;
            default:
                break;
        }
        return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
        {
            SourceUID = message.DestUID,
            DestUID = message.SourceUID,
            Command = message.Command | ERDM_Command.RESPONSE,
            Parameter = message.Parameter
        };
    }
    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        switch (parameter)
        {
            case ERDM_Parameter.IDENTIFY_ENDPOINT when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_TO_UNIVERSE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_MODE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_LABEL when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.RDM_TRAFFIC_ENABLE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.DISCOVERY_STATE when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.BACKGROUND_DISCOVERY when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.ENDPOINT_TIMING when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY when command is ERDM_Command.SET_COMMAND:
            case ERDM_Parameter.BINDING_CONTROL_FIELDS when command is ERDM_Command.GET_COMMAND:
                return true;
        }
        return base.IsHandlingParameter(parameter, command);
    }


    public async Task<bool> SetMode(ushort endpointId, ERDM_EndpointMode mode)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.ENDPOINT_MODE, new GetSetEndpointMode(endpointId, mode)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).Mode = mode;
            return true;
        }
        return false;
    }
    public async Task<bool> SetDiscoveryState(ushort endpointId, ERDM_DiscoveryState discoveryState)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.DISCOVERY_STATE, new SetDiscoveryStateRequest(endpointId, discoveryState)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).DiscoveryState = discoveryState;
            return true;
        }
        return false;
    }
    public async Task<bool> SetUniverse(ushort endpointId, ushort universe)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.ENDPOINT_TO_UNIVERSE, new GetSetEndpointToUniverse(endpointId, universe)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).Universe = universe;
            return true;
        }
        return false;
    }
    public async Task<bool> SetRDMTraffic(ushort endpointId, bool rdmTraffic)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.RDM_TRAFFIC_ENABLE, new GetSetEndpointRDMTrafficEnable(endpointId, rdmTraffic)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).RDMTraffic = rdmTraffic;
            return true;
        }
        return false;
    }
    public async Task<bool> SetIdentify(ushort endpointId, bool identify)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.IDENTIFY_ENDPOINT, new GetSetIdentifyEndpoint(endpointId, identify)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).Identify = identify;
            return true;
        }
        return false;
    }
    public async Task<bool> SetBackgroundDiscovery(ushort endpointId, bool backgroundDiscovery)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.BACKGROUND_DISCOVERY, new GetSetEndpointBackgroundDiscovery(endpointId, backgroundDiscovery)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).BackgroundDiscovery = backgroundDiscovery;
            return true;
        }
        return false;
    }
    public async Task<bool> SetTiming(ushort endpointId, byte timing)
    {
        if (await ParentRemoteDevice.SetParameter(ERDM_Parameter.ENDPOINT_TIMING, new SetEndpointTimingRequest(endpointId, timing)))
        {
            ((RemoteEndpoint)Endpoints.FirstOrDefault(e => e.EndpointId == endpointId)).Timing = timing;
            return true;
        }
        return false;
    }
}