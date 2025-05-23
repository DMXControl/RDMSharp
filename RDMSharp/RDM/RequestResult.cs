using System;

namespace RDMSharp
{
    public readonly struct RequestResult
    {
        public readonly RDMMessage Request;
        public readonly RDMMessage Response;
        public readonly bool Success;
        public readonly bool Cancel;
        public readonly TimeSpan? ElapsedTime;

        public RequestResult(in RDMMessage request, in bool cancle = false)
        {
            Request = request;
            Response = null;
            Success = false;
            Cancel = cancle;
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
}