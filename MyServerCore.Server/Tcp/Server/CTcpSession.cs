﻿using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;
using MyServerCore.Log.Log;
using MyServerCore.Server.CRC;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.MessageRouter.JsonMessage;
using MyServerCore.Server.MessageRouter.JsonMessage.Server;
using MyServerCore.Server.MessageRouter.Server;
using MyServerCore.Server.ProtobufService;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyServerCore.Server.Tcp.Server;

public class CTcpSession:TcpSession
{
    

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tcpService"></param>
    public CTcpSession(TcpService tcpService,bool isjson=true):base(tcpService)
    {
        this.isjson = isjson;
        this.tcpService = tcpService;
    }
    private TcpService tcpService;
    bool isjson ;
    protected override void OnConnected()
    {
        
        MyLogTool.ColorLog(MyLogColor.Blue,$"客户端：{Id}已连接");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        try
        {
            if (isjson) // json 发送数据 格式 4字节长度+ (数据+ 流水码)+ 2 个字节的 crc16检验码
            {
                byte[] lengthBytes = new byte[4];
                Array.Copy(buffer, 0, lengthBytes, 0, 4);
                if (!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
                int length = BitConverter.ToInt32(lengthBytes, 0);
                if (length > 0)
                {
                    byte[] infactMessage = new byte[length];
                    Array.Copy(buffer, 4, infactMessage, 0, length);
                    byte[] messageCode = new byte[infactMessage.Length - 2];
                    byte[] crcCode = new byte[2];
                    Array.Copy(infactMessage, 0, messageCode, 0, messageCode.Length);
                    Array.Copy(infactMessage, infactMessage.Length - 2, crcCode, 0, crcCode.Length);
                    ushort currentCrcCode = BitConverter.ToUInt16(crcCode, 0);
                    ushort computedCrc = CRCService.ComputeChecksum(messageCode);
                    if (currentCrcCode == computedCrc)
                    {
                        byte[] messages = new byte[messageCode.Length - 2];
                        Array.Copy(messageCode, 0, messages, 0, messages.Length);
                        string mess = Encoding.UTF8.GetString(messages);
                        Console.WriteLine(mess);
                        int len = mess.Length;
                        if (len == 4) return;

                        if (ServerJsonMessageRouter.GetInstance().Running)
                        {
                            ServerJsonMessageRouter.GetInstance().AddMessageDataToQueue(this, mess);
                        }


                    }
                }
            }
            else   // protobuf 发送数据 格式 4字节长度+ (int 表示 type 数据 + 数据+ 流水码)  +2 个字节的 crc16检验码
            {
                byte[] lengthBytes = new byte[4];
                Array.Copy(buffer, 0, lengthBytes, 0, 4);
                if (!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
                int length = BitConverter.ToInt32(lengthBytes, 0);
                if (length > 0)
                {
                    byte[] infactMessage = new byte[length];
                    Array.Copy(buffer, 4, infactMessage, 0, length);
                    byte[] messageCode = new byte[infactMessage.Length - 2];
                    byte[] crcCode = new byte[2];
                    Array.Copy(infactMessage, 0, messageCode, 0, messageCode.Length);
                    Array.Copy(infactMessage, infactMessage.Length - 2, crcCode, 0, crcCode.Length);
                    ushort currentCrcCode = BitConverter.ToUInt16(crcCode, 0);
                    ushort computedCrc = CRCService.ComputeChecksum(messageCode);
                    if (currentCrcCode == computedCrc)
                    {
                        byte[] messages = new byte[messageCode.Length - 2];
                        Array.Copy(messageCode, 0, messages, 0, messages.Length);
                        int code = BitConverter.ToInt32(messages, 0);
                        Type type = ProtobufSession.SeqType(code);
                        if (type.IsClass && typeof(IMessage).IsAssignableFrom(type))
                        {
                            byte[] data = new byte[messages.Length - 4];
                            Array.Copy(messages, 4, data, 0, data.Length);
                            IMessage packMessage = ProtobufSession.ParseFrom(code, data, 0, data.Length);
                            if (packMessage != null)
                            {
                                if (MessageRouter.ServiceMessageRouter.GetInstance().IsRunning)
                                {
                                    MyService session = new MyService()
                                    {
                                        tcpSession = this
                                    };
                                    MessageRouter.ServiceMessageRouter.GetInstance().AddMessageToQueue(session, packMessage);
                                }
                                Console.WriteLine("收到客户端:" + packMessage);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            MyLogTool.ColorLog(MyLogColor.Red, e.ToString());

        }
    }

    protected override void OnError(SocketError error)
    {
        
    }


    #region  json 发送

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    public void CSendJsonData<T>(T data) where T : class
    {
        if (data == null) return;
        string json = JsonConvert.SerializeObject(data);
        CSendJsonData(json);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    public void CSendJsonData(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            byte[] message = Encoding.UTF8.GetBytes(json);
            message = message.Concat(CRCService.CreateWaterByte()).ToArray();
            byte[] crcCode = BitConverter.GetBytes(CRCService.ComputeChecksum(message));
            message = message.Concat(crcCode).ToArray();
            CSendJsonData(message);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSendJsonData(byte[] message)
    {
        int length = message.Length;
        byte[] buffer = BitConverter.GetBytes(length);
        if (buffer.Length > 0)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            byte[] result = buffer.Concat(message).ToArray();
           tcpService. Multicast(result);
        }
    }
    #endregion

    #region protobuf 发送
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public void CSendProtobufData<T>(T data) where T : IMessage<T>
    {
        if (data == null) return;
        int code = ProtobufSession.SeqCode(data.GetType());
        byte[] typeCode = BitConverter.GetBytes(code);
        byte[] message = ProtobufSession.Serialize(data);
        byte[] mess = typeCode.Concat(message).ToArray();
        byte[] waterCode = CRCService.CreateWaterByte();
        byte[] m = mess.Concat(waterCode).ToArray();
        byte[] crc = BitConverter.GetBytes(CRCService.ComputeChecksum(m));
        byte[] result = m.Concat(crc).ToArray();
        CSendProtobufData(result);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSendProtobufData(byte[] message)
    {
        int length = message.Length;
        byte[] buffer = BitConverter.GetBytes(length);
        if (buffer.Length > 0)
        {
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);

            byte[] result = buffer.Concat(message).ToArray();
           tcpService. Multicast(result);
        }
    }

    #endregion
}