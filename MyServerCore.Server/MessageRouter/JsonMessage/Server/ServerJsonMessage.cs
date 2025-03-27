using MyServerCode.Summer.Service;
using MyServerCore.Server.JsonMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.MessageRouter.JsonMessage.Server
{
    public class ServerJsonMessage
    {
        public TcpSession tcpSession { get; set; }

         public CJsonMessage jsonMessage { get; set; }
    }
}
