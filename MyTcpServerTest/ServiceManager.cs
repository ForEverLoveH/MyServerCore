﻿using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.WebSocket.Ws.Server;
using MyServerCore.Log.Log;
using MyServerCore.Server.GenerateCertificate;
using MyServerCore.Server.Http.Server;
using MyServerCore.Server.Https.Server;
using MyServerCore.Server.MessageRouter;
using MyServerCore.Server.MessageRouter.JsonMessage;
using MyServerCore.Server.MessageRouter.JsonMessage.Server;
using MyServerCore.Server.MessageRouter.Server;
using MyServerCore.Server.Ssl.Server;
using MyServerCore.Server.Tcp.Server;
using MyServerCore.Server.Udp.Server;
using MyServerCore.Server.WS.Server;
using MyServerCore.Server.WSS.Server;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

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
        ServerJsonMessageRouter.GetInstance().StartService(4);
        if (type == 0) StartTcpService();
        else if (type == 1) StartUdpService();
        else if (type == 2) StartCSSlService();
        else if (type == 3) StartWsService();
        else if (type == 4)  StartWSSService();
        else if (type == 5) StartHttpService();
        else if (type == 6) StartHttpsService();
        //StartHeartbeatService();
    }
    /// <summary>
    /// 记录链接对象的最后一次心跳时间
    /// </summary>
    private ConcurrentDictionary<MyService, DateTime> HeartBeatPairs = new ConcurrentDictionary<MyService, DateTime>();
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
        _wsServer = new CWsServer(IPAddress.Any, 8080);

        string www =Path.Combine(  AppDomain.CurrentDomain.BaseDirectory,"www/ws");
        _wsServer.AddStaticContent(www, "/chat");
        MyLogTool.ColorLog(MyLogColor.Red,$"WebSocket server static content path: {www}");
        MyLogTool.ColorLog(MyLogColor.Red,$"WebSocket server website: https://localhost:{8443}/chat/index.html");
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

    #region 心跳
    /// <summary>
    /// 
    /// </summary>
    private Timer CheackHeartTimer;
    /// <summary>
    /// 
    /// </summary>
    private void StartHeartbeatService()
    {
        ServiceMessageRouter.GetInstance().OnMessage<HeartBeatRequest>(_HeartBeatRequest);
        if (type != 1)
        {
            CheackHeartTimer = new Timer(_TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="message"></param>
    private void _HeartBeatRequest(MyService session, HeartBeatRequest message)
    {
        if (HeartBeatPairs.ContainsKey(session))
        {
            HeartBeatPairs[session] = DateTime.Now;
        } else
        {
            HeartBeatPairs.TryAdd(session, DateTime.Now);
        }
        // Thread.Sleep(300);
        HeartBeatResponse messages = new HeartBeatResponse();
        if(session.tcpSession != null)
        {
            CTcpSession service= (CTcpSession)session.tcpSession;
            service.CSendProtobufData(messages);
        }
        else if (session.udpServer != null)
        {
            CUdpServer server= (CUdpServer)session.udpServer;
            server.CSendProtobufData(messages);
        }
        else  if(session.sslSession != null)
        {
            CSslServer server= (CSslServer) session.sslSession;
            server.CSendProtobufData(messages);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="state"></param>
    private void _TimerCallback(object state)
    {
        MyLogTool.ColorLog(MyLogColor.Blue,  "执行心跳检查");
        var now = DateTime.Now;
        if (HeartBeatPairs != null && HeartBeatPairs.Count > 0)
        {
            foreach (var pair in HeartBeatPairs)
            {
                var cha = now - pair.Value;
                if (cha.TotalMilliseconds > 3000)
                {
                    //关闭超时链接
                    var service = pair.Key;
                    if (service.tcpSession != null)
                    {
                        service.tcpSession.Service.Stop();
                        HeartBeatPairs.TryRemove(service,out DateTime  lastTime);
                    }
                     
                }
            }
        }
    }
    #endregion
}