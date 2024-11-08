using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Http;

namespace MyServerCore.Server.Http.Server;

public class CHttpServer:HttpService
{
    public CHttpServer(IPAddress ipAddress, int port) : base(ipAddress, port)
    {
        
    }
    
    protected override TcpSession CreateSession() { return new CHttpSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}