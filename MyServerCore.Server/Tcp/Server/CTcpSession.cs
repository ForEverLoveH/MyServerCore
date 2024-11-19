using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;
using MyServerCore.Server.CRC;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.ProtobufService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyServerCore.Server.Tcp.Server;

public class CTcpSession:TcpSession
{
    

    private ConcurrentDictionary<Guid, Socket> _connectedClient = new ConcurrentDictionary<Guid, Socket>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tcpService"></param>
    public CTcpSession(TcpService tcpService,bool isjson=true) : base(tcpService)
    {
        
    }

    protected override void OnConnected()
    {
        System.Net.Sockets.Socket mscoSocket = Socket;
        Guid guid = Id;
        if (!_connectedClient.ContainsKey(guid))
            _connectedClient.TryAdd(Id, mscoSocket);
        Console.WriteLine($"客户端：{guid}已连接");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    protected override void OnReceived(byte[] buffer, long offset, long size)
    {

        //byte[] lengthBytes = new byte[4];
        //Array.Copy(buffer, 0, lengthBytes, 0, 4);
        //if(!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
        //int length = BitConverter.ToInt32(lengthBytes, 0);
        //if(length > 0)
        //{
        //    byte[] infactMessage = new byte[length];
        //    Array.Copy(buffer, 4, infactMessage, 0, length);
        //byte[] dataCodes = new byte[data.Length - 2];
        //ushort crc = BitConverter.ToUInt16(dataCodes, dataCodes.Length - 2);
        //Array.Copy(data, dataCodes, dataCodes.Length);
        //ushort computedCrc = CRCService.ComputeChecksum(data);
        //if (computedCrc == crc)
        //    string message = Encoding.UTF8.GetString(infactMessage);
        //    Console.WriteLine("收到客户端:"+message);
        //}


        byte[] lengthBytes = new byte[4];
        Array.Copy(buffer, 0, lengthBytes, 0, 4);
        if (!BitConverter.IsLittleEndian) Array.Reverse(lengthBytes);
        int length = BitConverter.ToInt32(lengthBytes, 0);
        if (length > 0)
        {
            byte[] infactMessage = new byte[length];
            Array.Copy(buffer, 4, infactMessage, 0, length);
            int code = BitConverter.ToInt32(infactMessage, 0);
            Type type =ProtobufSession.SeqType(code);
            if (type.IsClass && typeof(IMessage).IsAssignableFrom(type))
            {
                byte[] data = new byte[length - 4];
                Array.Copy(infactMessage, 4, data, 0, length - 4);
                byte[] dataCodes = new byte[data.Length - 2];
                ushort crc = BitConverter.ToUInt16(dataCodes, dataCodes.Length - 2);
                Array.Copy(data, dataCodes, dataCodes.Length);
                ushort computedCrc = CRCService.ComputeChecksum(data);
                if (computedCrc == crc)
                {
                    IMessage message = ProtobufSession.ParseFrom(code, dataCodes, 0, dataCodes.Length);
                    if(ServiceMessageRouter.GetInstance().IsRunning)
                    {
                        ServiceMessageRouter.GetInstance().AddMessageToQueue(this,message);
                    }
                     
                    Console.WriteLine($"收到客户端:{message}");

                }
            }

        }

    }

    protected override void OnError(SocketError error)
    {
        
    }
}