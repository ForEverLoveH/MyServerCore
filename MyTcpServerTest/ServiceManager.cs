using MyServerCode.Summer.Service.SSL;
using MyServerCore.Server.GenerateCertificate;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.Ssl.Server;
using MyServerCore.Server.Tcp.Server;
using MyServerCore.Server.Udp.Server;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MyTcpServerTest;

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
    public void StartService()
    {
        ServiceMessageRouter.GetInstance().StartService(10);
        if (type==0) StartTcpService();
        else if(type==1) StartUdpService();
        else if (type==2) StartCSSlService();
    }

    #region  tcp 服务
    /// <summary>
    
    /// </summary>
    private void StartCSSlService()
    {
        string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cert\\");
        string friendlyName = $"{Guid.NewGuid().ToString("N")}dpps.fun";
        string pfxPath = $"{file}{friendlyName}.pfx";
        string certPath = $"{file}{friendlyName}.cer";
        if(!Directory.Exists(file)) Directory.CreateDirectory(file);
        if (!File.Exists(pfxPath))
        {
            MyGenerateCertificate myGenerateCertificate= new MyGenerateCertificate(DateTime.Now.AddYears(1), "qwerty", friendlyName);
            myGenerateCertificate.CreateGenerateCertificate(certPath,pfxPath, friendlyName);
        }
        var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(pfxPath, "qwerty"));
        _sslServer = new CSslServer(context,IPAddress.Parse("127.0.0.1"), 9996);
        _sslServer.Start();
    }

    private void StartUdpService()
    {
        _udpServer = new CUdpServer("127.0.0.1", 9996);
        _udpServer.Start();

    }
     
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