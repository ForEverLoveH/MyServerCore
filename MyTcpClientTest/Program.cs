// See https://aka.ms/new-console-template for more information

using MyServerCore.Server.JsonMessage;
using MyTcpClientTest;

Console.WriteLine("Hello, World!");
Thread.Sleep(1000);
ServiceManager _service = new ServiceManager(0);
_service.StartService();
Thread .Sleep(2000);
if (_service.IsConnection)
{
    //string json =
    //    "{\"Id\":79,\"Rybh\":\"ry078\",\"Xm\":\"李羽雅\",\"Bmbh\":\"bm001\",\"Bmmc\":\"一支队\",\"Sfzhm\":\"0000076\",\"Xb\":\"男\",\"Rylx\":1,\"Sr\":\"1982/1/1\",\"Ryimages\":\"E:\\\\net project\\\\WorkProject\\\\Products\\\\IntegratedTrainingPlatform\\\\bin\\\\Debug\\\\net6.0-windows\\\\RyImages\\\\36e112c7953a37bb528eea6975419f7.jpg\",\"Rwsj\":\"2024-10-17T00:00:00\",\"Sg\":181.0,\"Bmi\":null,\"Tz\":71.0,\"Cdbh\":\"0\",\"Tzl\":null,\"CreateTime\":\"2024-10-25T17:31:33\",\"UpdateTime\":\"2024-10-25T17:31:33\",\"State\":null}";
    //UserLoginRequest json = new UserLoginRequest()
    //{
    //    Username = "admin",
    //    Password = "admin"

    //};
    Req_Login json = new Req_Login()
    {
       account="admin",
        password = "admin"
    };
    


    _service.SendData(json);

}

while (true)
{
    Thread.Sleep(1000);
}