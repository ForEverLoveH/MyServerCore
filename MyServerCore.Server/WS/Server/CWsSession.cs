using MyServerCode.Summer.Service.WebSocket.Ws.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WS.Server
{
    public class CWsSession: WsSession
    {
        public CWsSession(WsServer server) : base(server) { }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            // Resend the message back to the client
            SendBinaryAsync(buffer, offset, size);
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Session caught an error with code {error}");
        }
    }
}
