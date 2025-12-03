using System;
using System.ComponentModel;

namespace RDMSharp;

public class RequestResponseHistoryEntry : INotifyPropertyChanged
{
    public readonly DateTime Timestamp = DateTime.Now;
    public readonly RDMMessage Request;

    private DateTime? _timestampRespone;
    public DateTime? TimestampResponse
    {
        get
        {
            return _timestampRespone;
        }
        private set
        {
            if (_timestampRespone == value)
                return;
            _timestampRespone = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimestampResponse)));
        }
    }

    private RDMMessage _response;
    public RDMMessage Response
    {
        get
        {
            return _response;
        }
        private set
        {
            if (_response == value)
                return;
            _response = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Response)));
        }
    }

    private string _state;
    public string State
    {
        get
        {
            return _state;
        }
        private set
        {
            if (_state == value)
                return;
            _state = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public RequestResponseHistoryEntry(RDMMessage request)
    {
        Request = request;
    }

    internal void SetResponse(RDMMessage response, string state = "Success")
    {
        Response = response;
        TimestampResponse = DateTime.Now;
        State = state;
    }
}