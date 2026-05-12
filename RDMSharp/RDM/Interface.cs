using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp;

public class Interface : INotifyPropertyChanged, IEquatable<Interface>
{
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<EventArgs> CurrentIPChanged;
    public event EventHandler<EventArgs> StaticIPChanged;

    public readonly uint InterfaceId;
    public readonly EARP_HardwareTypes HardwareType;

    private string lable;
    public string Lable
    {
        get { return lable; }
        internal set
        {
            if (lable == value)
                return;

            lable = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Lable)));
        }
    }

    private ERDM_DHCPStatusMode currentIP_DHCPStatus;
    public ERDM_DHCPStatusMode CurrentIP_DHCPStatus
    {
        get { return currentIP_DHCPStatus; }
        internal set
        {
            if (currentIP_DHCPStatus == value)
                return;
            currentIP_DHCPStatus = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentIP_DHCPStatus)));
        }
    }

    private IPv4Address currentIP;
    public IPv4Address CurrentIP
    {
        get { return currentIP; }
        internal set
        {
            if (currentIP == value)
                return;

            currentIP = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentIP)));
        }
    }

    private byte currentSubnetMask;
    public byte CurrentSubnetMask
    {
        get { return currentSubnetMask; }
        internal set
        {
            if (currentSubnetMask == value)
                return;

            currentSubnetMask = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentSubnetMask)));
        }
    }
    private IPv4Address staticIP;
    public IPv4Address StaticIP
    {
        get { return staticIP; }
        internal set
        {
            if (staticIP == value)
                return;

            staticIP = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(StaticIP)));
            if (!dhcp)
            {
                currentIP_DHCPStatus = ERDM_DHCPStatusMode.INACTIVE;
                CurrentIP = staticIP;
            }
        }
    }

    private byte staticSubnetMask;
    public byte StaticSubnetMask
    {
        get { return staticSubnetMask; }
        internal set
        {
            if (staticSubnetMask == value)
                return;

            staticSubnetMask = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(StaticSubnetMask)));
            if (!dhcp)
            {
                currentIP_DHCPStatus = ERDM_DHCPStatusMode.INACTIVE;
                CurrentSubnetMask = staticSubnetMask;
            }
        }
    }

    public readonly IPv4Address DefaultIP;
    public readonly byte DefaultSubnetMask;

    private MACAddress macAddress;
    public MACAddress MACAddress
    {
        get { return macAddress; }
        internal set
        {
            if (macAddress == value)
                return;

            macAddress = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(MACAddress)));
        }
    }

    private bool dhcp;
    public bool DHCP
    {
        get { return dhcp; }
        set
        {
            if (dhcp == value)
                return;

            dhcp = value;
            if (!value)
            {
                CurrentIP = StaticIP;
                CurrentSubnetMask = StaticSubnetMask;
            }
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DHCP)));
        }
    }
    private bool zeroConf;
    public bool ZeroConf
    {
        get { return zeroConf; }
        set
        {
            if (zeroConf == value)
                return;

            zeroConf = value;
            this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(ZeroConf)));
        }
    }


    public Interface(in uint interfaceId, in EARP_HardwareTypes hardwareType)
    {
        this.InterfaceId = interfaceId;
        this.HardwareType = hardwareType;
    }
    internal protected Interface(in byte interfaceId,
        in EARP_HardwareTypes hardwareType,
        in string lable,
        in IPv4Address zeroconfIp,
        in byte zeroconfSubnetMask,
        in MACAddress macAddress) : this(interfaceId, hardwareType)
    {
        Lable = lable;
        DefaultIP = zeroconfIp;
        DefaultSubnetMask = zeroconfSubnetMask;
        staticIP = DefaultIP;
        staticSubnetMask = DefaultSubnetMask;
        currentIP = StaticIP;
        currentSubnetMask = StaticSubnetMask;
        MACAddress = macAddress;
        dhcp = false;
        zeroConf = true;
        currentIP_DHCPStatus = ERDM_DHCPStatusMode.INACTIVE;
    }

    public void SetStaticIP(in IPv4Address staticIP, in byte staticSubnetMask)
    {
        if (staticSubnetMask < 0 || staticSubnetMask > 32)
            throw new ArgumentOutOfRangeException(nameof(staticSubnetMask), "Subnet mask must be between 0 and 32");
        StaticIP = staticIP;
        StaticSubnetMask = staticSubnetMask;

        this.StaticIPChanged?.InvokeFailSafe(this, EventArgs.Empty);
        if (!dhcp)
            this.SetCurrentIP(staticIP, staticSubnetMask, false);
    }
    protected void SetCurrentIP(in IPv4Address currentIP, in byte currentSubnetMask, in bool isGivenByDHCP)
    {
        if (currentSubnetMask < 0 || currentSubnetMask > 32)
            throw new ArgumentOutOfRangeException(nameof(currentSubnetMask), "Subnet mask must be between 0 and 32");

        this.CurrentIP = currentIP;
        this.CurrentSubnetMask = currentSubnetMask;
        this.CurrentIP_DHCPStatus = isGivenByDHCP ? ERDM_DHCPStatusMode.ACTIVE : ERDM_DHCPStatusMode.INACTIVE;

        this.CurrentIPChanged?.InvokeFailSafe(this, EventArgs.Empty);
    }

    public virtual void RenewDHCP()
    {
    }

    public virtual void ReleaseDHCP()
    {
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Interface: {this.InterfaceId}");
        sb.AppendLine($"Lable: {this.Lable}");
        sb.AppendLine($"CurrentIP: {this.CurrentIP}/{this.CurrentSubnetMask}");
        sb.AppendLine($"MACAddress: {this.MACAddress}");
        sb.AppendLine($"DHCP: {this.DHCP}");
        sb.AppendLine($"ZeroConf: {this.ZeroConf}");

        return sb.ToString();
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Sensor);
    }

    public bool Equals(Interface other)
    {
        return other is not null &&
               InterfaceId == other.InterfaceId &&
               Lable == other.Lable &&
               DefaultIP == other.DefaultIP &&
               DefaultSubnetMask == other.DefaultSubnetMask &&
               StaticIP == other.StaticIP &&
               StaticSubnetMask == other.StaticSubnetMask &&
               CurrentIP == other.CurrentIP &&
               CurrentSubnetMask == other.CurrentSubnetMask &&
               MACAddress == other.MACAddress &&
               DHCP == other.DHCP &&
               ZeroConf == other.ZeroConf;
    }

    public override int GetHashCode()
    {
#if !NETSTANDARD
        HashCode hash = new HashCode();
        hash.Add(InterfaceId);
        hash.Add(Lable);
        hash.Add(DefaultIP);
        hash.Add(DefaultSubnetMask);
        hash.Add(StaticIP);
        hash.Add(StaticSubnetMask);
        hash.Add(CurrentIP);
        hash.Add(CurrentSubnetMask);
        hash.Add(MACAddress);
        hash.Add(DHCP);
        hash.Add(ZeroConf);
        return hash.ToHashCode();
#else
        int hashCode = 1916557166;
        hashCode = hashCode * -1521134295 + InterfaceId.GetHashCode();
        hashCode = hashCode * -1521134295 + Lable.GetHashCode();
        hashCode = hashCode * -1521134295 + DefaultIP.GetHashCode();
        hashCode = hashCode * -1521134295 + DefaultSubnetMask.GetHashCode();
        hashCode = hashCode * -1521134295 + StaticIP.GetHashCode();
        hashCode = hashCode * -1521134295 + StaticSubnetMask.GetHashCode();
        hashCode = hashCode * -1521134295 + CurrentIP.GetHashCode();
        hashCode = hashCode * -1521134295 + CurrentSubnetMask.GetHashCode();
        hashCode = hashCode * -1521134295 + MACAddress.GetHashCode();
        hashCode = hashCode * -1521134295 + DHCP.GetHashCode();
        hashCode = hashCode * -1521134295 + ZeroConf.GetHashCode();
        return hashCode;
#endif
    }
}