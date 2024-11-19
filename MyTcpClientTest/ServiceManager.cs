using Google.Protobuf;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.Tcp.Client;
using MyServerCore.Server.Udp.Client;

namespace MyTcpClientTest;

public class ServiceManager
{
     private int type = -1;
     private CTcpClient _cTcpClient;
     private CUdpClient _client;
    

     public ServiceManager(int type=0)
     {
          this.type = type;
     }
     public bool IsConnection
     {
          get
          {
               if (type == 0) return _cTcpClient.IsConnected;
               else return _client.IsConnected;
          }
     }

     /// <summary>
     /// 
     /// </summary>
     public void StartService()
     {
        
         
          if (type==0) StartTcpService();
          else
          {
               StartUdpService();
          }
        


     }

     private void StartUdpService()
     {
          _client = new CUdpClient("127.0.0.1", 9996);
          _client.Connect();
     }
     private void StartTcpService()
     {
          _cTcpClient = new CTcpClient("127.0.0.1", 9996);
          _cTcpClient.ConnectAsync();
     }

     /// <summary>
     /// 
     /// </summary>
     /// <param name="json"></param>
     public void SendData(string json)
     {
        if(type==0) _cTcpClient.CSendJsonData(json);
        else  _client.CSend(json);
     }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public void SendData<T>(T data)  where T : IMessage<T>
    {
        if (type == 0) _cTcpClient.CSendProtobufData(data);
    }
}