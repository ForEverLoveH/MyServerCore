using Google.Protobuf;
using MyServerCore.Server.Tcp.Client;
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
        public CTcpClient tcpClient { get; set; }
    }
}
