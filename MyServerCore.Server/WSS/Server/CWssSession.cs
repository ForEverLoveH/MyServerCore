using MyServerCode.Summer.Service.WebSocket.WSS;
using MyServerCode.Summer.Service.WebSocket.WSS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WSS.Server
{
    public class CWssSession : WssSession
    {
        public CWssSession(WssServer server) : base(server)
        {
        }
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
