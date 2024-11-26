// See https://aka.ms/new-console-template for more information

using MyTcpServerTest;

Console.WriteLine("Hello, World!");
ServiceManager serviceManager = new ServiceManager(2);
serviceManager.StartService();


while (true)
{
    Thread.Sleep(1000);
}