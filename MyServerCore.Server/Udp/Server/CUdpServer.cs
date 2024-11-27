using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyServerCore.Server.CRC;
using MyServerCore.Server.ProtobufService;
using MyServerCore.Server.RSA;
using Newtonsoft.Json;

namespace MyServerCore.Server.Udp.Server;

public class CUdpServer:MyServerCode.Summer.Service.UDP.UdpServer
{
    public CUdpServer(IPAddress address, int port) : base(address, port) { }
    public CUdpServer(string address,int port):base(address,port) { }

    protected override void OnStarted()
    {     
        ReceiveAsync();
    }
    protected override void OnSent(EndPoint endpoint, long sent)
    {     
        ReceiveAsync();
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        byte[] lengthBytes = new byte[4];
        Array.Copy(buffer, 0, lengthBytes, 0, 4);
        if(!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
        int length = BitConverter.ToInt32(lengthBytes, 0);
        if(length > 0)
        {
            byte[] infactMessage = new byte[length];
            Array.Copy(buffer, 4, infactMessage, 0, length);
            string message = Encoding.UTF8.GetString(infactMessage);
            Console.WriteLine("收到客户端:"+message);
        }  
    }

    
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Echo UDP server caught an error with code {error}");
    }
    #region 发送 json数据
    public void CSendData<T>(T data) where T : class
    {
        if(data == null)return  ;
        string json = JsonConvert.SerializeObject(data);
        CSendData(json);     
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    public void CSendData(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            byte[] message = Encoding.UTF8.GetBytes(json);
            CSendData(message);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSendData(byte[] message)
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
            MulticastAsync(result);
        }
    }
    #endregion

    #region protobuf 发送数据
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
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            byte[] result = buffer.Concat(message).ToArray();
            MulticastAsync(result);
        }
    }
    #endregion

}