using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using UdpClient = MyServerCode.Summer.Service.UDP.UdpClient;

namespace MyServerCore.Server.Udp.Client;

public class CUdpClient:UdpClient
{
    public CUdpClient(IPAddress address, int port) : base(address, port)
    {
    }

    public CUdpClient(string address, int port) : base(address, port)
    {
    }

    public CUdpClient(DnsEndPoint endpoint) : base(endpoint)
    {
    }

    public CUdpClient(IPEndPoint endpoint) : base(endpoint)
    {
    }
    protected override void OnConnected()
    {
        Console.WriteLine($"Echo UDP client connected a new session with Id {Id}");

        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Echo UDP client disconnected a session with Id {Id}");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            Connect();
    }

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
            Console.WriteLine("收到服务端:"+message);
        }  
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Echo UDP client caught an error with code {error}");
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
    private bool _stop;
}