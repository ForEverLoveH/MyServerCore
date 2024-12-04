using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.WebSocket.WSS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.WSS.Client
{
    public class CWssClient:WssClient
    {

        public CWssClient(SslContext context,string address,int port) : base(context, address, port)
        {

        }
        public CWssClient(SslContext context, string address, int port, int messages) : base(context, address, port)
        {
            _messages = messages;
        }

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

        public override void OnWsConnected(HttpResponse response)
        {
            for (long i = _messages; i > 0; i--)
                SendMessage();
        }

        protected override void OnSent(long sent, long pending)
        {
            _sent += sent;
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            _received += size;
            while (_received >=  MessageToSend.Length)
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
            SendBinaryAsync(MessageToSend, 0, MessageToSend.Length);
        }

        private long _sent;
        private long _received;
        private long _messages;
        private byte[] MessageToSend;
        private DateTime TimestampStart = DateTime.UtcNow;
        private DateTime TimestampStop = DateTime.UtcNow;
        private long TotalErrors;
        private long TotalBytes;
    }
}
