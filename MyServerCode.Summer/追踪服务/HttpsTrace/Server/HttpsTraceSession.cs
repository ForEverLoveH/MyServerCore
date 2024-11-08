using System.Net.Sockets;
using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.Https;

namespace MyServerCode.Summer.Service.HttpTrace;

public class HttpsTraceSession:HttpsSession
{
    public HttpsTraceSession(HttpsServer server) : base(server)
    {
        
    }
    protected override void OnReceivedRequest(HttpRequest request)
    {
        // Process HTTP request methods
        if (request.Method == "TRACE")
            SendResponseAsync(Response.MakeTraceResponse(request));
        else
            SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
    }

    protected override void OnReceivedRequestError(HttpRequest request, string error)
    {
        Console.WriteLine($"Request error: {error}");
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Session caught an error with code {error}");
    }
}