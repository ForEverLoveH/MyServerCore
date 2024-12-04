using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.UDP;
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
        public MySession session { get; set; }
    }
    public class MySession
    {
        public TcpSession tcpSession { get; set; }
        public UdpServer udpServer { get; set; }
        public  SslSession sslSession { get; set; }
    }
}
