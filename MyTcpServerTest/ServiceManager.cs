using MyServerCore.Server.Tcp.Server;

namespace MyTcpServerTest;

public class ServiceManager
{
    private CTcpService _tcpService;
    public void StartService()
    {
        StartTcpService();
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