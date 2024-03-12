namespace RDMSharp
{
    public readonly struct RDMDiscoveryStatus
    {
        public readonly int FoundDevices;
        public readonly ulong RangeLeftToSearch;
        public readonly string CurrentStatus;

        public RDMDiscoveryStatus(in int found, in ulong left2search, in string status)
        {
            this.FoundDevices = found;
            this.RangeLeftToSearch = left2search;
            this.CurrentStatus = status;
        }

    }
}
