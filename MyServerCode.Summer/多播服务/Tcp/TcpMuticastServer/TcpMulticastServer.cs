using System.Net;
using System.Net.Sockets;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;

namespace MyServerCode.Summer.MuticastServer;

public class TcpMulticastServer:TcpService
{
    public TcpMulticastServer(IPAddress address, int port) : base(address, port) {}

    protected override TcpSession CreateSession() { return new MulticastSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Server caught an error with code {error}");
    }
}