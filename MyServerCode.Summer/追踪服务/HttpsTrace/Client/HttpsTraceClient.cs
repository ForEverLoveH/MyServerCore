using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.Https;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCode.Summer.Service.HttpsTrace.Client;

public class HttpsTraceClient:HttpsClient
{
    public static DateTime TimestampStart = DateTime.UtcNow;
    public static DateTime TimestampStop = DateTime.UtcNow;
    public static long TotalErrors;
    public static long TotalBytes;
    public static long TotalMessages;
    public HttpsTraceClient(SslContext context, string address, int port, int messages) : base(context, address, port)
    {
        _messages = messages;
    }

    public void SendMessage() { SendRequestAsync(Request.MakeTraceRequest("/")); }

    protected override void OnHandshaked()
    {
        for (long i = _messages; i > 0; i--)
            SendMessage();
    }
    protected override void OnSent(long sent, long pending)
    {
        _sent += sent;
        base.OnSent(sent, pending);
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        _received += size;
         TimestampStop = DateTime.UtcNow;
        TotalBytes += size;
        base.OnReceived(buffer, offset, size);
    }

    protected override void OnReceivedResponse(HttpResponse response)
    {
        if (response.Status == 200)
           TotalMessages++;
        else
            TotalErrors++;
        SendMessage();
    }

    protected override void OnReceivedResponseError(HttpResponse response, string error)
    {
        Console.WriteLine($"Response error: {error}");
       TotalErrors++;
        SendMessage();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Client caught an error with code {error}");
       TotalErrors++;
    }

    private long _sent = 0;
    private long _received = 0;
    private long _messages = 0;
}