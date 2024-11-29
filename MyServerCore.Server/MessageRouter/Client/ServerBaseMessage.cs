using Google.Protobuf;
using MyServerCore.Server.Ssl.Client;
using MyServerCore.Server.Tcp.Client;
using MyServerCore.Server.Udp.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.MessageRouter.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class ServerBaseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public IMessage message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MyBaseClient myBaseClient { get; set; }
    }
    public class MyBaseClient
    {
        public CTcpClient tcpClient { get; set; }
        public CUdpClient udpClient { get; set; } 
        public CSslClient sslClient { get; set; }

    }
}
