using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.WebSocket.Ws.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WS.Server
{
    public class CWsServer:WsServer
    {
        public CWsServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new CWsSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }
}
