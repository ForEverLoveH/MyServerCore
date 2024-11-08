using System.Net.Sockets;
using System.Text;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCore.Server.Ssl.Server;

public class CSslSession:SslSession
{
    public CSslSession(SslServer server) : base(server)
    {
        
    }
    protected override void OnConnected()
    {
        Console.WriteLine($"Chat SSL session with Id {Id} connected!");
    }

    protected override void OnHandshaked()
    {
        Console.WriteLine($"Chat SSL session with Id {Id} handshaked!");

        // Send invite message
        string message = "Hello from SSL chat! Please send a message or '!' to disconnect the client!";
        Send(message);
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat SSL session with Id {Id} disconnected!");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Console.WriteLine("Incoming: " + message);
        Server.Multicast(message);
        
        if (message == "!")
            Disconnect();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat SSL session caught an error with code {error}");
    }
}