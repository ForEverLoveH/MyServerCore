using MyServerCore.Server.Tcp.Server;
using MyServerCore.Server.Udp.Server;

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
    public void StartService()
    {
        if(type==0) StartTcpService();
        else StartUdpService();
    }

    #region  tcp 服务


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