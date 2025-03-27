using MyServerCode.Summer.Service.Tcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.JsonMessage
{
    public class ClientJsonMessage
    {
        public TcpClient tcpClient { get; set; }

        public CJsonMessage jsonMessage { get; set; }
    }
    /// <summary>
    /// 客户端消息格式
    /// </summary>
    public class CJsonMessage
    {
        public JsonMessage message { get; set; }
        /// <summary>
        /// 消息数据类型
        /// </summary>
        public CMessageType messageType { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class JsonMessage
    {
        public Req_Login req_Login { get; set; }

        public Req_Register req_Register { get; set; }

        public Req_HeartBeat req_HeartBeat { get; set; }

        public Rsp_Login rsp_Login { get; set; }

        public Rsp_Register rsp_Register { get; set; }

        public Rsp_HeartBeat rsp_HeartBeat{get; set; }
        /// <summary>
        /// 消息数据的回复
        /// </summary>
        public Rsp_Message rspMessage { get; set; }
    }
    public enum CMessageType
    {
        Req_Login = 1,
        Rsp_Login = 2,
        Req_Register = 3,
        Rsp_Register = 4,
        Req_HeartBeat = 5,
        Rsp_HeartBeat = 6
    }
     
    /// <summary>
    /// 心跳请求
    /// </summary>
    public class Req_HeartBeat
    {

    }

    /// <summary>
    /// 注册请求
    /// </summary>
    public class Req_Register
    {
    }

    /// <summary>
    /// 登录请求
    /// </summary>
    public class Req_Login
    {
        public string account { get; set; }
        public string password { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Rsp_Message
    {
        
    }
    /// <summary>
    ///  心跳的回复
    /// </summary>
    public class Rsp_HeartBeat
    {
    }
    /// <summary>
    /// 注册的回复
    /// </summary>
    public class Rsp_Register
    {
    }
    /// <summary>
    /// 登录的回复
    /// </summary>
    public class Rsp_Login
    {
        
    }
}
