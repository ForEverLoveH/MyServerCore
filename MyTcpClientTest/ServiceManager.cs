using Google.Protobuf;
using MyServerCode.Summer.Service.SSL;
using MyServerCore.Server.GenerateCertificate;
using MyServerCore.Server.Http.Client;
using MyServerCore.Server.Https.Client;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.MessageRouter.Client;
using MyServerCore.Server.Ssl.Client;
using MyServerCore.Server.Tcp.Client;
using MyServerCore.Server.Udp.Client;
using MyServerCore.Server.WS.Client;
using MyServerCore.Server.WSS.Client;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MyTcpClientTest;

public class ServiceManager
{
    /// <summary>
    /// 
    /// </summary>
    private int type = -1;
    /// <summary>
    /// 
    /// </summary>
    private CTcpClient _cTcpClient;
    /// <summary>
    /// 
    /// </summary>
    private CUdpClient _udpclient;
     /// <summary>
     /// 
     /// </summary>
    private CSslClient _csslClient;
    private  CWSClient _cwSClient;
    private CWssClient  _cWssClient;
    private CHttpClient _cHttpClient;

    private CHttpsClient _cHttpsClient;
    
     /// <summary>
     /// 
     /// </summary>
     /// <param name="type"></param>
     public ServiceManager(int type=0)
     {
          this.type = type;
     }
    /// <summary>
    /// 
    /// </summary>
     public bool IsConnection
     {
          get
          {
               if (type == 0) return _cTcpClient.IsConnected;
               else  if(type==1)return _udpclient.IsConnected;
               else  if(type==2) return _csslClient.IsConnected;
               else if(type==3) return _cwSClient.IsConnected;
               else if(type==4) return _cWssClient.IsConnected;
               else if(type==5) return _cHttpClient.IsConnected;
               else if(type==6) return _cHttpsClient.IsConnected;
               else return false;   
          }
     }

     /// <summary>
     /// 
     /// </summary>
     public void StartService()
     {

        ClientMessageRouter.GetInstance().StartService(4);
        if (type == 0) StartTcpService();
        else if (type == 1) StartUdpService();
        else if (type == 2) StarSSLClientService();
        else if (type == 3) StarWsClientService();
        else if (type == 4) StartWssCleintService();
        else if (type == 5) StartHttpClientService();
        else
        {
            StartHttpsClientService();
        }
     }
    /// <summary>
    /// 
    /// </summary>
    private void StartHttpsClientService()
    {
        var context = CreateGenerateCertificate();
        _cHttpsClient = new CHttpsClient(context, "127.0.0.1", 9996);
        _cHttpsClient.ConnectAsync();
    }
    /// <summary>
    /// 
    /// </summary>
    private void StartHttpClientService()
    {
        _cHttpClient = new CHttpClient("127.0.0.1", 9996);
        _cHttpClient.ConnectAsync();
    }
    /// <summary>
    /// 
    /// </summary>
    private void StartWssCleintService()
    {
        var context = CreateGenerateCertificate();
        _cWssClient = new CWssClient(context, "127.0.0.1", 9996);

    }

         
     private void StarWsClientService()
    {
        _cwSClient = new CWSClient("127.0.0.1", 9996);
        _cwSClient.ConnectAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StarSSLClientService()
    {
        var context = CreateGenerateCertificate();
        _csslClient = new CSslClient(context, "127.0.0.1", 9996);
        _csslClient.ConnectAsync();
    }
    /// <summary>
    /// 
    /// </summary>
     private void StartUdpService()
     {
          _udpclient = new CUdpClient("127.0.0.1", 9996);
          _udpclient.Connect();
     }
    /// <summary>
    /// 
    /// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SslContext CreateGenerateCertificate()
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert\\");
        //string friendlyName = $"{Guid.NewGuid().ToString("N")}dpps.fun";
        string friendlyName = "Client_dpps.fun";
        string pfxPath = $"{file}{friendlyName}.pfx";
        string certPath = $"{file}{friendlyName}.cer";
        if (!Directory.Exists(file)) Directory.CreateDirectory(file);
        MyGenerateCertificate myGenerateCertificate = new MyGenerateCertificate(DateTime.Now.AddYears(1), "qwerty", friendlyName);
        bool exist = true;
        if (!File.Exists(pfxPath)) { myGenerateCertificate.CreateGenerateCertificate(certPath, pfxPath, friendlyName); exist = false; }
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
        return context;
    }
}