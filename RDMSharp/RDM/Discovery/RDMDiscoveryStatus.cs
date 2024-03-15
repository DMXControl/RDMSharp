namespace RDMSharp
{
    public readonly struct RDMDiscoveryStatus
    {
        public readonly int FoundDevices;
        public readonly ulong RangeLeftToSearch;
        public readonly double RangeDoneInPercent = 0;
        public readonly string CurrentStatus;

        public RDMDiscoveryStatus(in int found, in ulong left2search, in string status)
        {
            this.FoundDevices = found;
            this.RangeLeftToSearch = left2search;
            var rangeLeftToSearchInPercent = left2search / (double)(ulong)(RDMUID.Broadcast - 1);
            this.RangeDoneInPercent = 1 - rangeLeftToSearchInPercent;
            this.CurrentStatus = status;
        }
    }
}
