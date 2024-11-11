using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MyServerCode.Summer.Service;
using MyServerCode.Summer.Service.Tcp;
using Newtonsoft.Json;

namespace MyServerCore.Server.Tcp.Server;

public class CTcpService:TcpService
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="port"></param>
    public CTcpService(IPAddress ipAddress, int port) : base(ipAddress, port)
    {
        
    }

    public CTcpService(string ipaddress, int port) : base(ipaddress, port)
    {
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipEndPoint"></param>
    public CTcpService(IPEndPoint ipEndPoint) : base(ipEndPoint)
    {
        
    }

    protected override TcpSession CreateSession()
    {
        return new CTcpSession(this);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="error"></param>
    protected override void OnError(SocketError error)
    {
        
    }

    #region  发送
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    public  void CSendData<T>(T data) where T : class
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
            Multicast(result);
        }
    }
    #endregion
}