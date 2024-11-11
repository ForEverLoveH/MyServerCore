using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpClient = MyServerCode.Summer.Service.Tcp.TcpClient;

namespace MyServerCore.Server.Tcp.Client;

public class CTcpClient:TcpClient
{
    private bool _stop;
    public CTcpClient(string ipaddress, int port) : base(ipaddress, port)
    {
        
    }

    public CTcpClient(IPEndPoint ipEndPoint) : base(ipEndPoint)
    {
        
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat TCP client connected a new session with Id {Id}");
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
            string message = Encoding.UTF8.GetString(infactMessage, (int)offset, (int)size);
            Console.WriteLine(message);
        }
    }
    #region 发送
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    public void CSend<T>(T message) where T:class 
    {
        if(message == null)return;
        string json= JsonConvert.SerializeObject(message);
        CSend(json);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSend(string message)
    {
        if(string.IsNullOrEmpty(message))return;
        byte[] data= Encoding.UTF8.GetBytes(message);
        CSend(data);

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void CSend(byte[] message)
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
    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat TCP client caught an error with code {error}");
    }
}