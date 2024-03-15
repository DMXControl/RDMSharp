using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public readonly struct RDMDiscoveryStatus : IEquatable<RDMDiscoveryStatus>
    {
        public readonly int FoundDevices;
        public readonly ulong RangeLeftToSearch;
        public readonly double RangeDoneInPercent = 0;
        public readonly string CurrentStatus;
        public readonly RDMUID? LastFoundUid;

        public RDMDiscoveryStatus(in int found, in ulong left2search, in string status, RDMUID? lastFoundUid)
        {
            this.FoundDevices = found;
            this.RangeLeftToSearch = left2search;
            var rangeLeftToSearchInPercent = left2search / (double)(ulong)(RDMUID.Broadcast - 1);
            this.RangeDoneInPercent = 1 - rangeLeftToSearchInPercent;
            this.CurrentStatus = status;
            this.LastFoundUid = lastFoundUid;
        }

        public override bool Equals(object obj)
        {
            return obj is RDMDiscoveryStatus status && Equals(status);
        }

        public bool Equals(RDMDiscoveryStatus other)
        {
            return FoundDevices == other.FoundDevices &&
                   RangeLeftToSearch == other.RangeLeftToSearch &&
                   CurrentStatus == other.CurrentStatus &&
                   EqualityComparer<RDMUID?>.Default.Equals(LastFoundUid, other.LastFoundUid);
        }

        public override int GetHashCode()
        {
#if !NETSTANDARD
            return HashCode.Combine(FoundDevices, RangeLeftToSearch, CurrentStatus, LastFoundUid);
#else
            int hashCode = -1756596593;
            hashCode = hashCode * -1521134295 + FoundDevices.GetHashCode();
            hashCode = hashCode * -1521134295 + RangeLeftToSearch.GetHashCode();
            hashCode = hashCode * -1521134295 + CurrentStatus.GetHashCode();
            hashCode = hashCode * -1521134295 + LastFoundUid.GetHashCode();
            return hashCode;
#endif
        }

        public override string ToString()
        {
            return $"{CurrentStatus} Progress: {RangeDoneInPercent:P3} LastFoundUid: {LastFoundUid} Found Devices: {FoundDevices}";
        }

        public static bool operator ==(RDMDiscoveryStatus left, RDMDiscoveryStatus right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RDMDiscoveryStatus left, RDMDiscoveryStatus right)
        {
            return !(left == right);
        }
    }
}
