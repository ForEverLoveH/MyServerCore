using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;

namespace MyServerCore.Server.Tcp.Server;

public class CTcpService:TcpService
{

   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="port"></param>
    public CTcpService(IPAddress ipAddress, int port) : base(ipAddress, port)
    {
        
    }

    public CTcpService(string ipaddress, int port) : base(ipaddress, port)
    {
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipEndPoint"></param>
    public CTcpService(IPEndPoint ipEndPoint) : base(ipEndPoint)
    {
        
    }

    protected override TcpSession CreateSession()
    {
        return new CTcpSession(this);
    }

    protected override void OnError(SocketError error)
    {
        
    }
}