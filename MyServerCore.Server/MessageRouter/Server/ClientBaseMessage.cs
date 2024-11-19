using Google.Protobuf;
using MyServerCode.Summer.Service;
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
        public   TcpSession session { get; set; }
    }
}
