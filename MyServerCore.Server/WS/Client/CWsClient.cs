using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.WebSocket.Ws.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WS.Client
{
    public class CWSClient : WsClient
    {
        private long _sent;
        private long _received;
        private long _messages;
        private   byte[] MessageToSend;
        private   DateTime TimestampStart = DateTime.UtcNow;
        private DateTime TimestampStop = DateTime.UtcNow;
        private long TotalErrors;
        private long TotalBytes;
        public CWSClient(IPAddress address, int port) : base(address, port)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="port"></param>
        public CWSClient(string ipaddress,int port):this(IPAddress.Parse(ipaddress), port) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="messages"></param>
        public CWSClient(IPAddress address,int port ,int messages) : base(address, port)
        {
            this._messages = messages;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public override void OnWsConnecting(HttpRequest request)
        {
            request.SetBegin("GET", "/");
            request.SetHeader("Host", "localhost");
            request.SetHeader("Origin", "http://localhost");
            request.SetHeader("Upgrade", "websocket");
            request.SetHeader("Connection", "Upgrade");
            request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
            request.SetHeader("Sec-WebSocket-Protocol", "chat, superchat");
            request.SetHeader("Sec-WebSocket-Version", "13");
            request.SetBody();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        public override void OnWsConnected(HttpResponse response)
        {
            for (long i = _messages; i > 0; i--)
                SendMessage();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sent"></param>
        /// <param name="pending"></param>
        protected override void OnSent(long sent, long pending)
        {
            _sent += sent;
        }
        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            _received += size;
            while (_received >= MessageToSend.Length)
            {
                SendMessage();
                _received -=  MessageToSend.Length;
            }

            TimestampStop = DateTime.UtcNow;
            TotalBytes += size;
        }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Client caught an error with code {error}");
            TotalErrors++;
        }

        public void SendMessage()
        {
            SendBinaryAsync(MessageToSend, 0,MessageToSend.Length);
        }
    }
}
