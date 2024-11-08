using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service.Https;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCore.Server.Https.Server;

public class CHttpsServer:HttpsServer
{
    public CHttpsServer(SslContext context, IPAddress address, int port) : base(context, address, port) {}

    protected override SslSession CreateSession() { return new CHttpsSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTPS server caught an error: {error}");
    }
}