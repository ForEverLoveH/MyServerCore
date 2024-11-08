using System.Net.Sockets;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;

namespace MyServerCode.Summer.MuticastServer;

public class MulticastSession:TcpSession
{
    public MulticastSession(TcpService _tcpService) : base(_tcpService)
    {
        
    }
    public override bool SendAsync(byte[] buffer, long offset, long size)
    {
        // Limit session send buffer to 1 megabyte
        const long limit = 1 * 1024 * 1024;
        long pending = BytesPending + BytesSending;
        if ((pending + size) > limit)
            return false;
        if (size > (limit - pending))
            size = limit - pending;

        return base.SendAsync(buffer, offset, size);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Session caught an error with code {error}");
    }
}