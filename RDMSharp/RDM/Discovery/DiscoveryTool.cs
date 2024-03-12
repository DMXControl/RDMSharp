﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractDiscoveryTool : INotifyPropertyChanged
    {
        private protected static ILogger Logger = null;
        private AsyncRDMRequestHelper asyncRDMRequestHelper;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool DiscoveryInProgress { get; private set; }

        public AbstractDiscoveryTool()
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(SendRDMMessage);
        }

        protected abstract Task<bool> SendRDMMessage(RDMMessage rdmMessage);

        protected void ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            asyncRDMRequestHelper.ReceiveMethode(rdmMessage);
        }

        public async Task<IReadOnlyCollection<RDMUID>> PerformDiscovery(IProgress<RDMDiscoveryStatus> progress, bool full = true)
        {
            if (DiscoveryInProgress) return new List<RDMUID>();
            DiscoveryInProgress = true;
            RDMMessage unmuteBroadcastMessage = new RDMMessage();
            unmuteBroadcastMessage.Parameter = ERDM_Parameter.DISC_UN_MUTE;
            unmuteBroadcastMessage.DestUID = RDMUID.Broadcast;
            await SendRDMMessage(unmuteBroadcastMessage);

            //Send DISC_UN_MUTE
            if (full)
            {
                bool unmuted = false;
                for (int t_ry = 0; t_ry < 10 && !unmuted; t_ry++)
                {
                    RDMMessage m = new RDMMessage()
                    {
                        Command = ERDM_Command.DISCOVERY_COMMAND,
                        Parameter = ERDM_Parameter.DISC_UN_MUTE,
                        DestUID = RDMUID.Broadcast,
                    };
                    unmuted = await SendRDMMessage(unmuteBroadcastMessage);
                }
                if (!unmuted)
                {
                    Logger.LogError("Unable do send DISC_UNMUTE Command.");
                    return null;
                }
            }


            RDMMessage discMessage = new RDMMessage();
            discMessage.Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH;
            await asyncRDMRequestHelper.RequestParameter(discMessage);

            //Start Binary Search for each
            var erg = new RDMDiscoveryContext(progress);
            await DiscoverDevicesBinarySearch((ulong)RDMUID.Empty, (ulong)RDMUID.Broadcast - 1, erg);

            return erg.FoundUIDs.ToList();
        }


        private async Task DiscoverDevicesBinarySearch(ulong uidStart, ulong uidEnd, RDMDiscoveryContext context)
        {
            //Robust Code Check
            if (uidStart > uidEnd) return;
            if (uidStart == uidEnd)
            {
                var uid = new RDMUID(uidStart);
                await TryMuteSingleDeviceAndAdd(uid, context);
                context.RemoveRange(uidStart, uidEnd);
                return;
            }

            string msg = String.Format("Doing Discovery for Range {0} - {1}", new RDMUID(uidStart), new RDMUID(uidEnd));
            Logger.LogDebug(msg);
            context.Status = msg;

            bool success = false;
            RDMMessage response = null;
            for (int t_ry = 0; t_ry < 3 && !success; t_ry++)
            {
                RDMMessage m = new RDMMessage()
                {
                    Command = ERDM_Command.DISCOVERY_COMMAND,
                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                    DestUID = RDMUID.Broadcast,
                    SubDevice = SubDevice.Root,
                    ParameterData = new DiscUniqueBranchRequest(new RDMUID(uidStart), new RDMUID(uidEnd)).ToPayloadData()
                };
                var res= await asyncRDMRequestHelper.RequestParameter(m);
                if (res.Success)
                {
                    success = res.Success;
                    response = res.Response;
                }
            }

            if (!success) //Timeout, Error, No Device Responded, whatever it is, we are done
            {
                context.RemoveRange(uidStart, uidEnd);
                return;
            }
            if (response != null && response?.ChecksumValid == true) //Great, just 1 Device responded
            {
                if (response.Parameter == ERDM_Parameter.DISC_UNIQUE_BRANCH && response.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                {
                    var muted = await TryMuteSingleDeviceAndAdd(response.SourceUID, context);
                    if (muted == true)
                    {
                        //According to Spec, check same Branch again.
                        await DiscoverDevicesBinarySearch(uidStart, uidEnd, context);
                    }
                    else if (muted == null) //An already muted Device answered again! We try to fix by carving out the UID
                    {
                        //Split Range at number of Device
                        ulong shittyDevice = (ulong)response.SourceUID;
                        if (shittyDevice == uidStart) await DiscoverDevicesBinarySearch(uidStart + 1, uidEnd, context);
                        else if (shittyDevice == uidEnd) await DiscoverDevicesBinarySearch(uidStart, uidEnd - 1, context);
                        else if (shittyDevice > uidStart && shittyDevice < uidEnd)
                        {
                            await DiscoverDevicesBinarySearch(uidStart, shittyDevice - 1, context);
                            await DiscoverDevicesBinarySearch(shittyDevice + 1, uidEnd, context);
                        }
                        else
                            Logger.LogWarning("Device {0} answered outside of its UID Range!!! Go, throw it into the trash.", response.SourceUID);
                    }
                }
                else
                    Logger.LogWarning("Strange Discovery Answer received {0}", response);

                return;
            }

            //Conflict Result, continue Binary search
            var delta = uidEnd - uidStart;
            var mid = delta / 2;
            await DiscoverDevicesBinarySearch(uidStart, uidStart + mid, context);
            await DiscoverDevicesBinarySearch(uidStart + mid + 1, uidEnd, context);

            //To detect off by one errors in Devices, we do the same discovery, split the Range in 3 Ranges
            //Only if there are at least 3 Devices to discover left (delta = 2)
            if (delta >= 2)
            {
                var mid3 = delta / 3;
                var offByOneContext = new RDMDiscoveryContext();

                //AL 2020-12-08: Not sure whether usefull
                //msg = String.Format("Doing OffByOne Check for Range {0} - {1}", RDMUID.FromULong(uidStart), RDMUID.FromULong(uidEnd));
                //context.Status = msg;

                await DiscoverDevicesBinarySearch(uidStart, uidStart + mid3, offByOneContext);
                await DiscoverDevicesBinarySearch(uidStart + mid3 + 1, uidStart + mid3 + 1 + mid3, offByOneContext);
                await DiscoverDevicesBinarySearch(uidStart + mid3 + 1 + mid3 + 1, uidEnd, offByOneContext);

                if (offByOneContext.FoundCount > 0) //Something was added!
                {
                    var found = offByOneContext.FoundUIDs;
                    context.AddFound(found);

                    //Find the Bad Devices
                    Logger.LogWarning("You are lucky to use DMXControl! Some Devices don't have a proper RDM implementation as they seam to have an off by one error, but we handled that for you: [{0}]",
                        String.Join(",", found));
                }
            }
        }

        private async Task<bool?> TryMuteSingleDeviceAndAdd(RDMUID uid, RDMDiscoveryContext context)
        {
            if (context.AlreadyFound(uid))
            {
                Logger.LogWarning("Faulty device {0} did not mute properly as it responded again although it was muted!", uid);
                return null;
            }

            //Mute Device (10 tries)
            bool muted = false;
            for (int t_ry = 0; t_ry < 10 && !muted; t_ry++)
            {
                RDMMessage muteResponse = null;
                RDMMessage n = new RDMMessage()
                {
                    Command = ERDM_Command.DISCOVERY_COMMAND,
                    Parameter = ERDM_Parameter.DISC_MUTE,
                    DestUID = uid
                };
                var res= await asyncRDMRequestHelper.RequestParameter(n);
                if (res.Success)
                {
                    muted = res.Success;
                    muteResponse = res.Response;
                }
                muted &= muteResponse != null && muteResponse.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE
                         && muteResponse.Parameter == ERDM_Parameter.DISC_MUTE
                         && muteResponse.SourceUID == uid;
            }
            if (muted)
                context.AddFound(uid);
            else
                Logger.LogWarning("Unable to Mute Device {0}. Not added to List of discovered Items. Hopefully discovery works anyway.", uid);

            return muted;
        }
    }
    internal class RDMDiscoveryContext
    {
        private readonly HashSet<RDMUID> _foundUids = new HashSet<RDMUID>();
        private ulong _rangeToSearch = (ulong)RDMUID.Broadcast - 1;
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

        internal ICollection<RDMUID> FoundUIDs => _foundUids.ToList();

        internal void RemoveRange(ulong uidStart, ulong uidEnd)
        {
            var delta = uidEnd - uidStart + 1;
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
