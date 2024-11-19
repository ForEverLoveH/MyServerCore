using Google.Protobuf;
using MyServerCore.Server.CRC;
using MyServerCore.Server.ProtobufService;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using TcpClient = MyServerCode.Summer.Service.Tcp.TcpClient;

namespace MyServerCore.Server.Tcp.Client;

public class CTcpClient:TcpClient
{
    private bool _stop;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipaddress"></param>
    /// <param name="port"></param>
    public CTcpClient(string ipaddress, int port) : base(ipaddress, port)
    {
        
    }

    public CTcpClient(IPEndPoint ipEndPoint) : base(ipEndPoint)
    {
        
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
        StartHeartBeatService();
    }

     

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat TCP client disconnected a session with Id {Id}");
        // Wait for a while...
        Thread.Sleep(1000);
        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        byte[] lengthBytes = new byte[4];
        Array.Copy(buffer, 0, lengthBytes, 0, 4);
        if (!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
        int length = BitConverter.ToInt32(lengthBytes, 0);
        if (length > 0)
        {
            byte[] infactMessage = new byte[length];
            Array.Copy(buffer, 4, infactMessage, 0, length);
            byte[]   data= new byte[infactMessage.Length-2];
            ushort crc = BitConverter.ToUInt16(infactMessage, infactMessage.Length - 2);
            Array.Copy(infactMessage, data, data.Length);
            ushort computedCrc = CRCService.ComputeChecksum(data);
            if (computedCrc == crc)
            {
                string message = Encoding.UTF8.GetString(data, (int)offset, (int)size);
                Console.WriteLine("收到服务端:" + message);
            }
        }
    }
    #region 发送
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    public void CSendJsonData<T>(T message) where T:class 
    {
        if(message == null)return;
        string json= JsonConvert.SerializeObject(message);
        CSendJsonData(json);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSendJsonData(string message)
    {
        if(string.IsNullOrEmpty(message))return;
        byte[] data= Encoding.UTF8.GetBytes(message);
        CSendJsonData(data);

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
            Send(result);   
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
        byte[]crc=BitConverter.GetBytes(CRCService.ComputeChecksum(message));
        byte[] result = typeCode.Concat(message.Concat(crc)).ToArray();
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
            Send(result);
        }
    }
    #endregion

    #region 心跳
    /// <summary>
    /// 
    /// </summary>
    private Timer _heartBeatTimer;
    /// <summary>
    /// 
    /// </summary>
    private void StartHeartBeatService()
    {
        _heartBeatTimer= new Timer(_TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    private void _TimerCallback(object state)
    {

    }
    #endregion
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat TCP client caught an error with code {error}");
    }
}