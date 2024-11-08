using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;

namespace MyServerCore.Server.Tcp.Server;

public class CTcpSession:TcpSession
{
    
    private ConcurrentDictionary<Guid, Socket> _connectedClient = new ConcurrentDictionary<Guid, Socket>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tcpService"></param>
    public CTcpSession(TcpService tcpService) : base(tcpService)
    {
        
    }

    protected override void OnConnected()
    {
        System.Net.Sockets.Socket mscoSocket = Socket;
        Guid guid = Id;
        if (!_connectedClient.ContainsKey(guid))
            _connectedClient.TryAdd(Id, mscoSocket);
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        if (!string.IsNullOrWhiteSpace(message))
        {
            
        }
    }

    protected override void OnError(SocketError error)
    {
        
    }
}