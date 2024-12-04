using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.WebSocket.WSS.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WSS.Server
{
    public class CWssServer:WssServer
    {
        public CWssServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }
        public CWssServer(SslContext context ,string ipaddress,int port):base (context,ipaddress,port) { }

        protected override SslSession CreateSession() { return new CWssSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }
}
