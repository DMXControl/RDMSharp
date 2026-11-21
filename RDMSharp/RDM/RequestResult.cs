using System;

namespace RDMSharp;

public readonly struct RequestResult
{
    public readonly RDMMessage Request;
    public readonly RDMMessage Response;
    public readonly bool Success;
    public readonly bool Timeout;
    public readonly bool Disposing;
    public readonly TimeSpan? ElapsedTime;

    public RequestResult(in RDMMessage request, in bool timeout = false, in bool disposing = false)
    {
        Request = request;
        Response = null;
        Success = false;
        Timeout = timeout;
        Disposing = disposing;
        ElapsedTime = null;
    }

    public RequestResult(in RDMMessage request, in RDMMessage response, TimeSpan elapsedTime)
    {
        Request = request;
        Response = response;
        Success = true;
        ElapsedTime = elapsedTime;
    }
}