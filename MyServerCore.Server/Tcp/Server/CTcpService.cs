using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;
using MyServerCore.Server.CRC;
using MyServerCore.Server.ProtobufService;
using Newtonsoft.Json;

namespace MyServerCore.Server.Tcp.Server;
/// <summary>
/// 
/// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override TcpSession CreateSession()
    {
        return new CTcpSession(this);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    protected override void OnError(SocketError error)
    {
        
    }

     



}