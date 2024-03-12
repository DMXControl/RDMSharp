using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp
{
    internal class RDMDiscoveryContext
    {
        private readonly HashSet<RDMUID> _foundUids = new HashSet<RDMUID>();
        private ulong _rangeToSearch = (ulong)(RDMUID.Broadcast - 1);
        private string _status;

        private readonly IProgress<RDMDiscoveryStatus> _progress;

        public RDMDiscoveryContext(IProgress<RDMDiscoveryStatus> progress = null)
        {
            this._progress = progress;
        }

        internal bool AlreadyFound(RDMUID uid) => _foundUids.Contains(uid);

        internal void AddFound(RDMUID uid)
        {
            _foundUids.Add(uid);
            _progress?.Report(GetStatus());
        }

        internal void AddFound(IEnumerable<RDMUID> uid)
        {
            _foundUids.UnionWith(uid);
            _progress?.Report(GetStatus());
        }

        internal int FoundCount => _foundUids.Count;

        internal IReadOnlyCollection<RDMUID> FoundUIDs => _foundUids.ToList();

        internal void RemoveRange(RDMUID uidStart, RDMUID uidEnd)
        {
            var delta = (ulong)(uidEnd - uidStart) + 1;
            _rangeToSearch -= delta;
            _progress?.Report(GetStatus());
        }

        internal string Status
        {
            get => _status;
            set
            {
                _status = value;
                _progress?.Report(GetStatus());
            }
        }

        private RDMDiscoveryStatus GetStatus()
        {
            return new RDMDiscoveryStatus(FoundCount, _rangeToSearch, _status);
        }
    }
}
