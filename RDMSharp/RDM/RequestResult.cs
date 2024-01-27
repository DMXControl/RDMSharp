namespace RDMSharp
{
    public readonly struct RequestResult
    {
        public readonly RDMMessage Request;
        public readonly RDMMessage Response;
        public readonly bool Success;
        public readonly bool Cancel;

        public RequestResult(in RDMMessage request, in bool cancle = false)
        {
            Request = request;
            Response = null;
            Success = false;
            Cancel = cancle;
        }

        public RequestResult(in RDMMessage request, in RDMMessage response)
        {
            Request = request;
            Response = response;
            Success = true;
        }
    }
}