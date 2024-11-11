using MyServerCore.Server.Tcp.Client;

namespace MyTcpClientTest;

public class ServiceManager
{
     private CTcpClient _cTcpClient;
     public bool IsConnection
     {
          get => _cTcpClient.IsConnected;
     }

     /// <summary>
     /// 
     /// </summary>
     public void StartService()
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
         _cTcpClient.CSend(json);
     }
}