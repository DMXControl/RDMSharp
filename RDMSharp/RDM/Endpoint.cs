using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp;

public class Endpoint : INotifyPropertyChanged, IEquatable<Endpoint>
{
    public event PropertyChangedEventHandler PropertyChanged;

    public readonly ushort EndpointId;
    public readonly ERDM_EndpointType Type;

    private string lable;
    public string Lable
    {
        get { return lable; }
        set
        {
            if (lable == value)
                return;

            lable = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Lable)));
        }
    }

    private bool identify;
    public bool Identify
    {
        get { return identify; }
        set
        {
            if (identify == value)
                return;
            identify = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Identify)));
        }
    }

    private ushort universe;
    public ushort Universe
    {
        get { return universe; }
        set
        {
            if (universe == value)
                return;
            universe = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Universe)));
        }
    }

    private ERDM_EndpointMode mode;
    public ERDM_EndpointMode Mode
    {
        get { return mode; }
        set
        {
            if (mode == value)
                return;
            mode = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Mode)));
        }
    }

    private bool rdmTraffic;
    public bool RDMTraffic
    {
        get { return rdmTraffic; }
        set
        {
            if (rdmTraffic == value)
                return;
            rdmTraffic = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(RDMTraffic)));
        }
    }

    private ERDM_DiscoveryState discoveryState;
    public ERDM_DiscoveryState DiscoveryState
    {
        get { return discoveryState; }
        set
        {
            if (discoveryState == value)
                return;
            discoveryState = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DiscoveryState)));
        }
    }

    private ushort discoveryStateCount;
    public ushort DiscoveryStateCount
    {
        get { return discoveryStateCount; }
        set
        {
            if (discoveryStateCount == value)
                return;
            discoveryStateCount = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DiscoveryStateCount)));
        }
    }

    private bool backgroundDiscovery;
    public bool BackgroundDiscovery
    {
        get { return backgroundDiscovery; }
        set
        {
            if (backgroundDiscovery == value)
                return;
            backgroundDiscovery = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(BackgroundDiscovery)));
        }
    }

    private byte timing;
    public byte Timing
    {
        get { return timing; }
        set
        {
            if (timing == value)
                return;
            timing = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Timing)));
        }
    }

    private HashSet<UID> responders = new HashSet<UID>();
    public IReadOnlyCollection<UID> Responders
    {
        get { return responders; }
    }
    protected void AddResponder(UID responder)
    {
        if (responders.Contains(responder))
            return;
        responders.Add(responder);
        ResponderListChanged++;
        this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Responders)));
    }
    protected void RemoveResponder(UID responder)
    {
        if (!responders.Contains(responder))
            return;
        responders.Remove(responder);
        ResponderListChanged++;
        this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Responders)));
    }
    protected void SetBindingControlField(UID uid, ushort controlField, UID bindingUid)
    {
        bindingControlFields.AddOrUpdate(uid, new BindingControlField(uid, controlField, bindingUid), (k, v) => new BindingControlField(uid, controlField, bindingUid));
    }
    protected void RemoveBindingControlField(UID uid)
    {
        bindingControlFields.TryRemove(uid, out _);
    }

    private uint responderListChanged;
    public uint ResponderListChanged
    {
        get { return responderListChanged; }
        private set
        {
            if (responderListChanged == value)
                return;
            responderListChanged = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(ResponderListChanged)));
        }
    }

    private ConcurrentDictionary<UID, BindingControlField> bindingControlFields = new ConcurrentDictionary<UID, BindingControlField>();

    public IReadOnlyDictionary<UID, BindingControlField> BindingControlFields
    {
        get { return bindingControlFields; }
    }

    public Endpoint(in ushort endpointId, in ERDM_EndpointType type)
    {
        this.EndpointId = endpointId;
        this.Type = type;
    }
    internal protected Endpoint(in ushort endpointId,
        in ERDM_EndpointType type,
        in string lable) : this(endpointId, type)
    {
        Lable = lable;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Endpoint: {this.EndpointId}");
        sb.AppendLine($"Lable: {this.Lable}");
        //sb.AppendLine($"CurrentIP: {this.CurrentIP}/{this.CurrentSubnetMask}");
        //sb.AppendLine($"MACAddress: {this.MACAddress}");
        //sb.AppendLine($"DHCP: {this.DHCP}");
        //sb.AppendLine($"ZeroConf: {this.ZeroConf}");

        return sb.ToString();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Sensor);
    }

    public bool Equals(Endpoint other)
    {
        return other is not null &&
               EndpointId == other.EndpointId &&
               Lable == other.Lable &&
               Identify == other.Identify;
    }

    public override int GetHashCode()
    {
#if !NETSTANDARD
        HashCode hash = new HashCode();
        hash.Add(EndpointId);
        hash.Add(Lable);
        hash.Add(Identify);
        return hash.ToHashCode();
#else
        int hashCode = 1916557116;
        hashCode = hashCode * -1521134295 + EndpointId.GetHashCode();
        hashCode = hashCode * -1521134295 + Lable.GetHashCode();
        hashCode = hashCode * -1521134295 + Identify.GetHashCode();
        return hashCode;
#endif
    }

    public readonly struct BindingControlField
    {
        public readonly UID Uid;
        public readonly ushort ControlField;
        public readonly UID BindingUid;

        public BindingControlField(in UID uid, in ushort controlField, in UID bindingUid)
        {
            this.Uid = uid;
            this.ControlField = controlField;
            this.BindingUid = bindingUid;
        }
    }
}