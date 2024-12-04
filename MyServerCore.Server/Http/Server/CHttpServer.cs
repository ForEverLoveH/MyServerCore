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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipaddress"></param>
    /// <param name="port"></param>
    public CHttpServer(string ipaddress ,int port) : this(IPAddress.Parse(ipaddress), port)
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override TcpSession CreateSession() { return new CHttpSession(this); }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}