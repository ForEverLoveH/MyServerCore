using Google.Protobuf;
using MyServerCode.Summer.Service.SSL;
using MyServerCore.Server.GenerateCertificate;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.MessageRouter.Client;
using MyServerCore.Server.Ssl.Client;
using MyServerCore.Server.Tcp.Client;
using MyServerCore.Server.Udp.Client;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MyTcpClientTest;

public class ServiceManager
{
     private int type = -1;
     private CTcpClient _cTcpClient;
     private CUdpClient _udpclient;
    
     private CSslClient _csslClient; 
    

     public ServiceManager(int type=0)
     {
          this.type = type;
     }
     public bool IsConnection
     {
          get
          {
               if (type == 0) return _cTcpClient.IsConnected;
               else  if(type==1)return _udpclient.IsConnected;
               else return _csslClient.IsConnected;
          }
     }

     /// <summary>
     /// 
     /// </summary>
     public void StartService()
     {

           ClientMessageRouter.GetInstance().StartService(4);
          if (type==0) StartTcpService();
          else if(type==1)
          {
               StartUdpService();
          }
         else if (type == 2)
        {
             StarSSLClientService();
        }


     }

    private void StarSSLClientService()
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert\\");
        //string friendlyName = $"{Guid.NewGuid().ToString("N")}dpps.fun";
        string friendlyName = "Client_dpps.fun";
        string pfxPath = $"{file}{friendlyName}.pfx";
        string certPath = $"{file}{friendlyName}.cer";
        if (!Directory.Exists(file)) Directory.CreateDirectory(file);
        MyGenerateCertificate myGenerateCertificate = new MyGenerateCertificate(DateTime.Now.AddYears(1), "qwerty", friendlyName);
        bool exist = true;
        if (!File.Exists(pfxPath)) { myGenerateCertificate.CreateGenerateCertificate(certPath, pfxPath, friendlyName);   exist = false; }
        if (exist)
        {
            //当前证书过期
            DateTime now = DateTime.Now;
            Tuple<DateTime, DateTime> tuples = myGenerateCertificate.LoadingPfxCertificateTime(pfxPath, "qwerty");
            if (now > tuples.Item2)
            {
                File.Delete(pfxPath);
                File.Delete(certPath);
                exist = false;
                myGenerateCertificate.CreateGenerateCertificate(certPath, pfxPath, friendlyName);
            }
        }
        var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(pfxPath, "qwerty"), (sender, certificate, chain, sslPolicyErrors) => true);
        _csslClient = new CSslClient(context, "127.0.0.1", 9996);
        _csslClient.ConnectAsync();
    }
    private void StartUdpService()
     {
          _udpclient = new CUdpClient("127.0.0.1", 9996);
          _udpclient.Connect();
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
        else if(type==1)  _udpclient.CSend(json);
        else if(type==2)_csslClient.CSend(json);
     }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public void SendData<T>(T data)  where T : IMessage<T>
    {
        if (type == 0) _cTcpClient.CSendProtobufData(data);
        else if (type == 1) _udpclient.CSendProtobufData(data);
        else if (type == 2) _csslClient.CSendProtobufData(data);

    }
}