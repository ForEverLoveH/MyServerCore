using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.Tcp;
using MyServerCode.Summer.Service.UDP;
using MyServerCore.Server.Tcp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.MessageRouter.Server
{
    public class ClientBaseMessage
    {
        public IMessage message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MyService session { get; set; }
    }
    public class MyService
    {
        public TcpService tcpSession { get; set; }
        public UdpServer udpServer { get; set; }
        public  SslServer sslSession { get; set; }
    }
}
