 

using System.Net.Sockets;
using TcpClient = MyServerCode.Summer.Service.Tcp.TcpClient;

namespace MyServerCode.Summer.MuticastClient;

public class TcpMuticastClient:TcpClient
{
    public static long TotalErrors;
    public static long TotalBytes;
    public TcpMuticastClient(string address, int port) : base(address, port) {}

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        TotalBytes += size;
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Client caught an error with code {error}");
       TotalErrors++;
    }
}