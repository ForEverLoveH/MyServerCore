using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpClient = MyServerCode.Summer.Service.Tcp.TcpClient;

namespace MyServerCore.Server.Tcp.Client;

public class CTcpClient:TcpClient
{
    private bool _stop;
    public CTcpClient(string ipaddress, int port) : base(ipaddress, port)
    {
        
    }

    public CTcpClient(IPEndPoint ipEndPoint) : base(ipEndPoint)
    {
        
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");
        // Wait for a while...
        Thread.Sleep(1000);
        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }
    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string code= Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        if (!string.IsNullOrWhiteSpace(code))
        {
            
        }
        Console.WriteLine(code);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat TCP client caught an error with code {error}");
    }
}