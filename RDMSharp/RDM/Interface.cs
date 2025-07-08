using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp
{
    public class Interface : INotifyPropertyChanged, IEquatable<Interface>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public readonly uint InterfaceId;

        private string lable;
        public string Lable
        {
            get { return lable; }
            private set
            {
                if (lable == value)
                    return;

                lable = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(Lable)));
            }
        }

        private IPv4Address currentIP;
        public IPv4Address CurrentIP
        {
            get { return currentIP; }
            private set
            {
                if (currentIP == value)
                    return;

                currentIP = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(CurrentIP)));
            }
        }

        private byte subnetMask;
        public byte SubnetMask
        {
            get { return subnetMask; }
            private set
            {
                if (subnetMask == value)
                    return;

                subnetMask = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(SubnetMask)));
            }
        }

        private MACAddress macAddress;
        public MACAddress MACAddress
        {
            get { return macAddress; }
            private set
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
            private set
            {
                if (dhcp == value)
                    return;

                dhcp = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(DHCP)));
            }
        }
        private bool zeroConf;
        public bool ZeroConf
        {
            get { return zeroConf; }
            private set
            {
                if (zeroConf == value)
                    return;

                zeroConf = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(ZeroConf)));
            }
        }

        private EARP_HardwareTypes hardwareType;
        public EARP_HardwareTypes HardwareType
        {
            get { return hardwareType; }
            private set
            {
                if (hardwareType == value)
                    return;

                hardwareType = value;
                this.PropertyChanged?.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(HardwareType)));
            }
        }

        public Interface(in byte interfaceId)
        {
            this.InterfaceId = interfaceId;
        }
        internal protected Interface(in byte interfaceId,
            in string lable,
            in IPv4Address currentIp,
            in byte subnetMask,
            in MACAddress macAddress,
            in bool dhcp,
            in bool zeroConf,
            in EARP_HardwareTypes hardwareType) : this(interfaceId)
        {
            Lable = lable;
            CurrentIP = currentIp;
            SubnetMask = subnetMask;
            MACAddress = macAddress;
            DHCP = dhcp;
            ZeroConf = zeroConf;
            HardwareType = hardwareType;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Interface: {this.InterfaceId}");
            sb.AppendLine($"Lable: {this.Lable}");
            sb.AppendLine($"CurrentIP: {this.CurrentIP}/{this.SubnetMask}");
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
                   CurrentIP == other.CurrentIP &&
                   SubnetMask == other.SubnetMask &&
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
            hash.Add(CurrentIP);
            hash.Add(SubnetMask);
            hash.Add(MACAddress);
            hash.Add(DHCP);
            hash.Add(ZeroConf);
            return hash.ToHashCode();
#else
            int hashCode = 1916557166;
            hashCode = hashCode * -1521134295 + InterfaceId.GetHashCode();
            hashCode = hashCode * -1521134295 + Lable.GetHashCode();
            hashCode = hashCode * -1521134295 + CurrentIP.GetHashCode();
            hashCode = hashCode * -1521134295 + SubnetMask.GetHashCode();
            hashCode = hashCode * -1521134295 + MACAddress.GetHashCode();
            hashCode = hashCode * -1521134295 + DHCP.GetHashCode();
            hashCode = hashCode * -1521134295 + ZeroConf.GetHashCode();
            return hashCode;
#endif
        }
    }
}