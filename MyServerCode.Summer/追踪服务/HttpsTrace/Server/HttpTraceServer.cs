using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service.Https;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCode.Summer.Service.HttpTrace.Server;

public class HttpTraceServer :HttpsServer
{
    public HttpTraceServer(SslContext context, IPAddress address, int port) : base(context, address, port)
    {
        
    }
    protected override  void OnError(SocketError error)
    {
        
    }
}