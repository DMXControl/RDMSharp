using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractDiscoveryTool : INotifyPropertyChanged, IDisposable
    {
        private protected static ILogger Logger = Logging.CreateLogger<AbstractDiscoveryTool>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        public event PropertyChangedEventHandler PropertyChanged;
        public bool discoveryInProgress;
        public bool DiscoveryInProgress
        {
            get => discoveryInProgress;
            private set
            {
                if (discoveryInProgress == value)
                    return;
                discoveryInProgress = value;
                PropertyChanged?.InvokeFailSafe(nameof(DiscoveryInProgress));
            }
        }

        public AbstractDiscoveryTool()
        {
        }

        public async Task<IReadOnlyCollection<UID>> PerformDiscovery(IProgress<RDMDiscoveryStatus> progress = null, bool full = true)
        {
            if (DiscoveryInProgress) return new List<UID>();
            DiscoveryInProgress = true;

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
                        DestUID = UID.Broadcast,
                    };
                    await RDMSharp.Instance.SendMessage(m).WaitAsync(cts.Token);
                    unmuted = true;
                }
                if (!unmuted)
                {
                    Logger?.LogError("Unable do send DISC_UNMUTE Command.");
                    return null;
                }
            }
            //Start Binary Search for each
            var erg = new RDMDiscoveryContext(progress);
            await DiscoverDevicesBinarySearch(UID.Empty, UID.Broadcast - 1, erg).WaitAsync(cts.Token);

            DiscoveryInProgress = false;
            return erg.FoundUIDs.ToList();
        }


        private async Task DiscoverDevicesBinarySearch(UID uidStart, UID uidEnd, RDMDiscoveryContext context)
        {
            //Robust Code Check
            if (uidStart > uidEnd) return;
            if (uidStart == uidEnd)
            {
                await TryMuteSingleDeviceAndAdd(uidStart, context);
                context.RemoveRange(uidStart, uidEnd);
                return;
            }

            string msg = String.Format("Doing Discovery for Range {0} - {1}", uidStart, uidEnd);
            Logger?.LogDebug(msg);
            context.StatusString = msg;

            bool success = false;
            RDMMessage response = null;
            for (int t_ry = 0; t_ry < 3 && !success; t_ry++)
            {
                RDMMessage m = new RDMMessage()
                {
                    Command = ERDM_Command.DISCOVERY_COMMAND,
                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                    DestUID = UID.Broadcast,
                    SubDevice = SubDevice.Root,
                    ParameterData = new DiscUniqueBranchRequest(uidStart, uidEnd).ToPayloadData()
                };
                context.IncreaseMessageCounter();
                var res = await RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(m);
                if (res.Success)
                {
                    success = res.Success;
                    response = res.Response;
                }
            }

            if (response == null) //Timeout, Error, No Device Responded, whatever it is, we are done
            {
                context.RemoveRange(uidStart, uidEnd);
                return;
            }
            if (response != null && response?.ChecksumValid == true) //Great, just 1 Device responded
            {
                if (context.IsFalseOn(response.SourceUID) && !uidStart.Equals(response.SourceUID) && !uidEnd.Equals(response.SourceUID))
                {
                    // do Nothing
                }
                else if (response.Parameter == ERDM_Parameter.DISC_UNIQUE_BRANCH && response.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                {
                    var muted = await TryMuteSingleDeviceAndAdd(response.SourceUID, context);
                    if (muted == true)
                    {
                        //According to Spec, check same Branch again.
                        await DiscoverDevicesBinarySearch(uidStart, uidEnd, context);
                        return;
                    }
                    else if (muted == null) //An already muted Device answered again! We try to fix by carving out the UID
                    {
                        //Split Range at number of Device
                        UID shittyDevice = response.SourceUID;
                        if (shittyDevice == uidStart) await DiscoverDevicesBinarySearch(uidStart + 1, uidEnd, context);
                        else if (shittyDevice == uidEnd) await DiscoverDevicesBinarySearch(uidStart, uidEnd - 1, context);
                        else if (shittyDevice > uidStart && shittyDevice < uidEnd)
                        {
                            await DiscoverDevicesBinarySearch(uidStart, shittyDevice - 1, context);
                            await DiscoverDevicesBinarySearch(shittyDevice + 1, uidEnd, context);
                        }
                        else
                            Logger?.LogWarning($"Device {response.SourceUID} answered outside of its UID Range!!! Go, throw it into the trash.");

                        return;
                    }
                    else
                        context.AddFalseOn(response.SourceUID);
                }
                else
                    Logger?.LogWarning($"Strange Discovery Answer received {response}");
            }

            //Conflict Result, continue Binary search
            var delta = uidEnd - uidStart;
            var mid = delta / 2;
            await DiscoverDevicesBinarySearch(uidStart, uidStart + mid, context);
            await DiscoverDevicesBinarySearch(uidStart + mid + 1, uidEnd, context);

            //To detect off by one errors in Devices, we do the same discovery, split the Range in 3 Ranges
            //Only if there are at least 3 Devices to discover left (delta = 2)

            if ((ulong)delta >= 2)
            {
                var mid3 = delta / 3;
                var offByOneContext = new RDMDiscoveryContext();

                //AL 2020-12-08: Not sure whether usefull
                //msg = String.Format("Doing OffByOne Check for Range {0} - {1}", UID.FromULong(uidStart), UID.FromULong(uidEnd));
                //context.Status = msg;

                await DiscoverDevicesBinarySearch(uidStart, uidStart + mid3, offByOneContext);
                await DiscoverDevicesBinarySearch(uidStart + mid3 + 1, uidStart + mid3 + 1 + mid3, offByOneContext);
                await DiscoverDevicesBinarySearch(uidStart + mid3 + 1 + mid3 + 1, uidEnd, offByOneContext);

                if (offByOneContext.FoundCount > 0) //Something was added!
                {
                    var found = offByOneContext.FoundUIDs;
                    context.AddFound(found);

                    //Find the Bad Devices
                    Logger?.LogWarning($"You are lucky to use RDMSharp! Some Devices don't have a proper RDM implementation as they sem to have an off by one error, but we handled that for you: [{String.Join(",", found)}]");
                }
            }
        }

        private async Task<bool?> TryMuteSingleDeviceAndAdd(UID uid, RDMDiscoveryContext context)
        {
            if (context.AlreadyFound(uid))
            {
                Logger?.LogWarning($"Faulty device {uid} did not mute properly as it responded again although it was muted!");
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
                var res = await RDMSharp.Instance.AsyncRDMRequestHelper.RequestMessage(n);
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
                Logger?.LogWarning($"Unable to Mute Device {uid}. Not added to List of discovered Items. Hopefully discovery works anyway.");

            return muted;
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
