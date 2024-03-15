using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp
{
    internal class RDMDiscoveryContext
    {
        private readonly HashSet<RDMUID> _foundUids = new HashSet<RDMUID>();
        private readonly HashSet<RDMUID> _falseOnUids = new HashSet<RDMUID>();
        private readonly ConcurrentDictionary<ulong,RemovedUIDRange> removedRange= new ConcurrentDictionary<ulong,RemovedUIDRange>();
        private ulong rangeToSearch = (ulong)(RDMUID.Broadcast - 1);
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
        internal void AddFalseOn(RDMUID uid)
        {
            _falseOnUids.Add(uid);
        }
        internal bool IsFalseOn(RDMUID uid)
        {
            return _falseOnUids.Contains(uid);
        }

        internal int FoundCount => _foundUids.Count;

        internal IReadOnlyCollection<RDMUID> FoundUIDs => _foundUids.ToList();

        internal void RemoveRange(RDMUID uidStart, RDMUID uidEnd)
        {
            var newRemovedRange = new RemovedUIDRange(uidStart, uidEnd);
            var overlap = removedRange.FirstOrDefault(r => areRangesOverlapping(r.Value.StartUID, r.Value.EndUID, newRemovedRange.StartUID, newRemovedRange.EndUID));
            if (overlap.Value != null)
            {
                bool updated = removedRange.TryUpdate(overlap.Key, RemovedUIDRange.Merge(overlap.Value, newRemovedRange), overlap.Value);
            }
            else
            {
                ulong key = 0;
                if (removedRange.Keys.Count != 0)
                    key = removedRange.Keys.Max() + 1;
                removedRange.TryAdd(key, newRemovedRange);
            }

            ulong sumDelta = 0;
            foreach (var r in removedRange)
                sumDelta += (ulong)r.Value.Delta;

            rangeToSearch = (ulong)(RDMUID.Broadcast - 1) - sumDelta;
            _progress?.Report(GetStatus());

            bool areRangesOverlapping(RDMUID start1, RDMUID end1, RDMUID start2, RDMUID end2)
            {
                // Check for overlap
                if (start1 <= end2 && end1 >= start2)
                    return true; // Ranges overlap
                else if (start1 == (end2 + 1) || (end1 + 1) == start2)
                    return true;
                else
                    return false; // Ranges don't overlap
            }
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
            return new RDMDiscoveryStatus(FoundCount, rangeToSearch, _status);
        }
    }
    internal class RemovedUIDRange
    {
        public readonly RDMUID StartUID;
        public readonly RDMUID EndUID;
        public readonly RDMUID Delta;

        public RemovedUIDRange(in RDMUID startUID, in RDMUID endUID)
        {
            StartUID = startUID;
            EndUID = endUID;
            Delta = EndUID - StartUID;
        }
        public static RemovedUIDRange Merge(RemovedUIDRange one, RemovedUIDRange other)
        {
            return new RemovedUIDRange(new RDMUID(Math.Min((ulong)one.StartUID, (ulong)other.StartUID)), new RDMUID(Math.Max((ulong)one.EndUID, (ulong)other.EndUID)));
        }
        public override string ToString()
        {
            return $"{StartUID} - {EndUID}";
        }
    }
}
