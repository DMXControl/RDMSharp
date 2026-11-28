namespace RDMSharpTests.Devices.Mock;

/// <summary>
/// Simulates a shared RDM line (one DMX cable) with several responders.
/// <para>
/// A request is put on the line, every responder processes it and puts its response on the same line.
/// On a real cable responses that are sent at the same time collide (the bits get mixed), which is
/// exactly what the discovery (DISC_UNIQUE_BRANCH) relies on.
/// </para>
/// <para>
/// The line works in rounds instead of time windows: a round is opened before the request is handed
/// to the responders and closed when all of them have processed it. All responses put on the line
/// during the round are merged (bitwise OR, like colliding signals) and delivered exactly once.
/// This makes the result independent of thread scheduling and CPU load.
/// </para>
/// <para>
/// Requirement: responders answer synchronously while the request is processed
/// (true for the generated mock devices). A response that arrives outside of a round is delivered on its own.
/// </para>
/// </summary>
internal static class SendReceivePipelineImitateRealConditions
{
    // Only one request can be on the line at a time.
    private static readonly object lineLock = new object();
    // Protects the responses of the currently open round, responders run in parallel.
    private static readonly object roundLock = new object();
    private static List<byte[]>? openRound;

    public static Task RDMMessageSend(RDMMessage rdmMessage)
    {
        if (rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
            putResponseOnLine(rdmMessage.BuildMessage());
        else
        {
            if (!rdmMessage.SourceUID.IsValidDeviceUID)
                rdmMessage.SourceUID = new UID(0x1fff, 0x44444444);
            sendRequest(rdmMessage);
        }
        return Task.CompletedTask;
    }

    private static void sendRequest(RDMMessage request)
    {
        List<byte[]> responses;
        lock (lineLock)
        {
            lock (roundLock)
                openRound = new List<byte[]>();
            try
            {
                // All responders process the request here and put their responses on the line.
                RDMMessageReceivedRequest?.InvokeFailSafe(null, request);
            }
            finally
            {
                lock (roundLock)
                {
                    responses = openRound!;
                    openRound = null;
                }
            }
        }

        byte[]? lineData = mergeCollidingResponses(responses);
        if (lineData != null)
            RDMMessageReceivedResponse?.InvokeFailSafe(null, lineData);
    }

    private static void putResponseOnLine(byte[] data)
    {
        lock (roundLock)
        {
            if (openRound != null)
            {
                openRound.Add(data);
                return;
            }
        }
        // Not part of a round (e.g. an asynchronous responder), deliver it on its own.
        RDMMessageReceivedResponse?.InvokeFailSafe(null, data);
    }

    /// <summary>
    /// Responses sent at the same time collide on the line, simulated by a bitwise OR of all responses.
    /// A single response stays unchanged, no response means silence on the line.
    /// </summary>
    private static byte[]? mergeCollidingResponses(List<byte[]> responses)
    {
        if (responses.Count == 0)
            return null;
        if (responses.Count == 1)
            return responses[0];

        byte[] line = new byte[responses.Max(r => r.Length)];
        foreach (byte[] response in responses)
            for (int i = 0; i < response.Length; i++)
                line[i] |= response[i];
        return line;
    }

    public static event EventHandler<RDMMessage>? RDMMessageReceivedRequest;
    public static event EventHandler<byte[]>? RDMMessageReceivedResponse;
}
