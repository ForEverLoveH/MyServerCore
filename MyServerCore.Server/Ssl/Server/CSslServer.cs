using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCore.Server.Ssl.Server;

public class CSslServer:SslServer
{
    public CSslServer(SslContext context, IPAddress address, int port) : base(context, address, port) {}

    protected override SslSession CreateSession() { return new  CSslSession(this); 
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat SSL server caught an error with code {error}");
    }
}