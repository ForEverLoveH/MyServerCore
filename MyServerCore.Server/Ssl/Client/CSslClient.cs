using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.SSL.Client;
using MyServerCore.Log.Log;
using MyServerCore.Server.CRC;
using MyServerCore.Server.MessageRouter.Client;
using MyServerCore.Server.ProtobufService;
using MyServerCore.Server.RSA;
using Newtonsoft.Json;

namespace MyServerCore.Server.Ssl.Client;

public class CSslClient:SslClient
{
    public CSslClient(SslContext context, string address, int port,bool isjson=false) : base(context, address, port)
    {
        this.isJson = isjson;   
    }
    private bool isJson = false;
    public void DisconnectAndStop()
    {
        _stop = true;
        DisconnectAsync();
        while (IsConnected)
            Thread.Yield();
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat SSL client connected a new session with Id {Id}");
    }

    protected override void OnHandshaked()
    {
        Console.WriteLine($"Chat SSL client handshaked a new session with Id {Id}");
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat SSL client disconnected a session with Id {Id}");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        if (!isJson)
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
                ///
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
                            if (ClientMessageRouter.GetInstance().IsRunning)
                            {
                                //ClientMessageRouter.GetInstance().AddMessageToQueue(this, packMessage);
                            }

                        }
                    }
                }
            }



        }
        else
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
                    if (!string.IsNullOrEmpty(mess))
                    {
                        MyLogTool.ColorLog(MyLogColor.Blue, string.Format("{0}:{1}", "收到服务端json 数据", mess));
                    }
                }


            }
        }
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat SSL client caught an error with code {error}");
    }
    #region json 发送

    public  void CSend(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        CSend(bytes );
    }
    public void CSend<T>(T message) where T : class
    {
        string json = JsonConvert.SerializeObject(message);
        CSend(json);
    }
    public void CSend(byte[] message)
    {
        SendAsync(message);
    }
    #endregion

    #region protobuf 发送
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public  void   CSendProtobufData<T>(T data) where T : IMessage<T>
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
    public  void  CSendProtobufData(byte[] message)
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
              SendAsync(result);
        }
    }
    #endregion

    private bool _stop;
}