using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Http;

namespace MyServerCode.Summer.HttpTrace;

public class HttpTraceServer:HttpService
{
    public HttpTraceServer(IPAddress address, int port) : base(address, port) {}

    protected override TcpSession CreateSession() { return new HttpTraceSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Server caught an error with code {error}");
    }
}