using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RDMSharp.RDM.Device;

public class ParameterMetadata : INotifyPropertyChanged
{
    public readonly ERDM_Parameter Parameter;

    public event PropertyChangedEventHandler PropertyChanged;

    private EQueuedParameterCapabilitiesStatus _queuedCapabilitiesStatus = EQueuedParameterCapabilitiesStatus.Unknown;
    public EQueuedParameterCapabilitiesStatus QueuedCapabilitiesStatus
    {
        get
        {
            return _queuedCapabilitiesStatus;
        }
        internal set
        {
            if (_queuedCapabilitiesStatus == value)
                return;
            _queuedCapabilitiesStatus = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QueuedCapabilitiesStatus)));
        }
    }

    private DateTime _lastUpdated = DateTime.MinValue;
    public DateTime LastUpdated
    {
        get
        {
            return _lastUpdated;
        }
        private set
        {
            if (_lastUpdated == value)
                return;
            _lastUpdated = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastUpdated)));
        }
    }

    private List<PeerToPeerProcess> peerToPeerProcesses = new List<PeerToPeerProcess>();
    public IReadOnlyCollection<PeerToPeerProcess> PeerToPeerProcesses => peerToPeerProcesses;

    private List<RequestResponseHistoryEntry> requestResponseHistory = new List<RequestResponseHistoryEntry>();
    public IReadOnlyCollection<RequestResponseHistoryEntry> RequestResponseHistory => requestResponseHistory;

    public event EventHandler<RequestResponseHistoryEntry> OnRequestResponseHistoryAdded;

    public ParameterMetadata(ERDM_Parameter parameter)
    {
        Parameter = parameter;
    }

    internal void AddPeerToPeerProcess(PeerToPeerProcess process)
    {
        process.PropertyChanged += Process_PropertyChanged;
        process.OnRequestResponseHistoryAdded += Process_OnRequestResponseHistoryAdded;
        peerToPeerProcesses.Add(process);
    }

    private void Process_OnRequestResponseHistoryAdded(object sender, RequestResponseHistoryEntry entry)
    {
        const int maxHistoryEntries = 20;
        requestResponseHistory.Add(entry);

        if (requestResponseHistory.Count > maxHistoryEntries)
        {
            int excessEntries = requestResponseHistory.Count - maxHistoryEntries;
            requestResponseHistory.RemoveRange(0, excessEntries);
        }
        OnRequestResponseHistoryAdded?.Invoke(this, entry);
    }

    private void Process_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PeerToPeerProcess process = sender as PeerToPeerProcess;
        switch (e.PropertyName)
        {
            case nameof(process.State):
                switch (process.State)
                {
                    case PeerToPeerProcess.EPeerToPeerProcessState.Finished:
                        peerToPeerProcesses.Remove(process);
                        LastUpdated = DateTime.Now;
                        break;
                    case PeerToPeerProcess.EPeerToPeerProcessState.Failed:
                        peerToPeerProcesses.Remove(process);
                        break;
                }
                break;

        }
    }
}
[Flags]
public enum EQueuedParameterCapabilitiesStatus : byte
{
    Unknown = 0b00000000,
    ParameterNotSupported = 0b00000010,
    Try_1 = 0b00000100,
    Try_2 = 0b00001000,
    Try_3 = 0b00010000,
    Pending = 0b00100000,
    Supported = 0b01000000,
    NotSupported = 0b10000000,
}