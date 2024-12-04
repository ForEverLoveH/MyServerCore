using MyServerCode.Summer.Service.SSL;
using MyServerCore.Server.GenerateCertificate;
using MyServerCore.Server.Http.Server;
using MyServerCore.Server.Https.Server;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.Ssl.Server;
using MyServerCore.Server.Tcp.Server;
using MyServerCore.Server.Udp.Server;
using MyServerCore.Server.WS.Server;
using MyServerCore.Server.WSS.Server;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MyTcpServerTest;
/// <summary>
/// 注意 WSS 服务和ssl 类似。就不做调用测试
/// </summary>
public class ServiceManager
{
    private int type = 0;
    public ServiceManager(int type = 0)
    {
        this.type = type;
    }
    private CTcpService _tcpService;
    private CUdpServer _udpServer;
    private CSslServer _sslServer;
    private CWssServer _wssServer;
    private CWsServer _wsServer;
    private CHttpServer _cHttpServer;
    private CHttpsServer _chttpsServer;
    /// <summary>
    /// 
    /// </summary>
    public void StartService()
    {
        ServiceMessageRouter.GetInstance().StartService(10);
        if (type == 0) StartTcpService();
        else if (type == 1) StartUdpService();
        else if (type == 2) StartCSSlService();
        else if (type == 3) StartWSSService();
        else if (type == 4) StartWsService();
        else if (type == 5) StartHttpService();
        else if (type == 6) StartHttpsService();
    }
    #region http 服务
    /// <summary>
    /// 
    /// </summary>
    private void StartHttpsService()
    {
        var context = CreateGenerateCertificate();
        _chttpsServer = new CHttpsServer(context, "127.0.0.1", 9996);
        _chttpsServer.Start();
        
    }
    /// <summary>
    /// 
    /// </summary>
    private void StartHttpService()
    {
        _cHttpServer = new CHttpServer("127.0.0.1", 9996);
        _cHttpServer.Start();
    }
    #endregion

    #region websocket 服务
    /// <summary>
    /// 
    /// </summary>
    private void StartWsService()
    {
        _wsServer = new CWsServer("127.0.0.1", 9966);
        _wsServer.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartWSSService()
    {
        var context=CreateGenerateCertificate();    
         _wssServer = new CWssServer(context,"127.0.0.1", 9966);
        _wssServer.Start();
    }
    #endregion
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private SslContext CreateGenerateCertificate()
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert\\");
        //string friendlyName = $"{Guid.NewGuid().ToString("N")}dpps.fun";
        string friendlyName = "Server_dpps.fun";
        string pfxPath = $"{file}{friendlyName}.pfx";
        string certPath = $"{file}{friendlyName}.cer";
        MyGenerateCertificate myGenerateCertificate = new MyGenerateCertificate(DateTime.Now.AddYears(1), "qwerty", friendlyName);
        bool exist = true;
        if (!Directory.Exists(file)) Directory.CreateDirectory(file);
        if (!File.Exists(pfxPath))
        {
            exist = false;
            myGenerateCertificate.CreateGenerateCertificate(certPath, pfxPath, friendlyName);
        }
        if (exist)
        {
            //证书过期
            DateTime now = DateTime.Now;
            Tuple<DateTime, DateTime> tuples = myGenerateCertificate.LoadingPfxCertificateTime(pfxPath, "qwerty");
            if (now > tuples.Item2)
            {
                File.Delete(pfxPath);
                File.Delete(certPath);
                Thread.Sleep(1000);
                myGenerateCertificate.CreateGenerateCertificate(certPath, pfxPath, friendlyName);
                exist = true;
            }
        }
        var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(pfxPath, "qwerty"));
        return context;
    }
    /// <summary>
    ///SSL 服务
    /// </summary>
    private void StartCSSlService()
    {
        var context=CreateGenerateCertificate();
        
        _sslServer = new CSslServer(context,  "127.0.0.1", 9996);
        _sslServer.Start();
    }
    /// <summary>
    /// udp 服务
    /// </summary>
    private void StartUdpService()
    {
        _udpServer = new CUdpServer("127.0.0.1", 9996);
        _udpServer.Start();

    }
    #region  tcp 服务


    /// <summary>
    /// tcp 服务
    /// </summary>
    private void StartTcpService()
    {
        _tcpService = new CTcpService("127.0.0.1", 9996);
        _tcpService.Start();
    }
    
    
    #endregion
}