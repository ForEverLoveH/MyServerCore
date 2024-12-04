using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service.Https;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCore.Server.Https.Server;
/// <summary>
/// 
/// </summary>
public class CHttpsServer:HttpsServer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="address"></param>
    /// <param name="port"></param>
    public CHttpsServer(SslContext context, IPAddress address, int port) : base(context, address, port) {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ipaddress"></param>
    /// <param name="port"></param>
    public CHttpsServer(SslContext context,string ipaddress,int port):base(context,ipaddress,port)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override SslSession CreateSession() { return new CHttpsSession(this); }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTPS server caught an error: {error}");
    }
}