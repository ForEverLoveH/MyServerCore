using Google.Protobuf;
using MyServerCode.Summer.Service;
using MyServerCore.Server.MessageRouter.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server.MessageRouter
{
    public class ServiceMessageRouter:Singleton<ServiceMessageRouter>
    {
        private AutoResetEvent _autoEvent = new AutoResetEvent(true);
        /// <summary>
        /// 线程总数
        /// </summary>
        private int threadCount;
        /// <summary>
        /// 工作线程数
        /// </summary>
        private int currentThreadCount;
        /// <summary>
        /// 
        /// </summary>
        private bool isRunning=false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
        }
        /// <summary>
        /// 
        /// </summary>
        private ConcurrentQueue<ClientBaseMessage> _messageQueue = new ConcurrentQueue<ClientBaseMessage>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="message"></param>
        public delegate void MessageHandler<T>(TcpSession session,T message) where T : IMessage;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,Delegate> _handlers = new Dictionary<string, Delegate>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="message"></param>
        public void AddMessageToQueue<T>(TcpSession session,T message) where T : IMessage
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(new ClientBaseMessage()
                {
                    session= session,
                    message= message

                });
            }
            if(_messageQueue.Count > 0)
            {
                _autoEvent.Set();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCount"></param>
        public void StartService(int threadCount)
        {
            if(isRunning) return;
            isRunning = true;
            this.threadCount = threadCount;
            this.threadCount = Math.Min(Math.Max(threadCount, 1), 200);
            for(int i = 0; i< threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem( new     WaitCallback(_HandlerCurrentMessageCallBack));
            }
            while(currentThreadCount < this.threadCount)
            {
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void _HandlerCurrentMessageCallBack(object? state)
        {
            try
            {
                currentThreadCount = Interlocked.Increment(ref currentThreadCount);
                while (IsRunning)
                {
                    if (_messageQueue.Count > 0)
                    {
                        ClientBaseMessage pMessage;
                        lock (_messageQueue)
                        {
                            if (_messageQueue.Count == 0) continue;
                            _messageQueue.TryDequeue(out pMessage);
                        }
                        if (pMessage != null)
                        {
                            var packMessage = pMessage.message;
                            if (packMessage != null)ExcuteLoopMessage(packMessage, pMessage.session);    
                        }
                    }
                    else
                    {
                        _autoEvent.WaitOne();
                        continue;
                    }
                }
            }
            finally
            {
               currentThreadCount  = Interlocked.Decrement(ref currentThreadCount);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="session"></param>
        private void ExcuteLoopMessage(IMessage message, TcpSession session)
        {
            var fireMethod = this.GetType().GetMethod("FireMessageData", BindingFlags.NonPublic | BindingFlags.Instance);
            var met = fireMethod.MakeGenericMethod(message.GetType());
            met.Invoke(this, new object[] { session, message });
            var t = message.GetType();
            foreach (var p in t.GetProperties())
            {
                // Log.Information($"{p.Name}");
                if (p.Name == "Parser" || p.Name == "Descriptor")
                    continue;
                //只要发现消息就可以订阅 递归思路实现
                var value = p.GetValue(message);
                if (value != null)
                {
                    //发现消息是否需要进一步递归 触发订阅
                    if (typeof(IMessage).IsAssignableFrom(value.GetType()))
                    {
                        //发现消息是否需要进一步递归 触发订阅
                        //继续递归
                        ExcuteLoopMessage((IMessage)value, session);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="messageData"></param>
        private void FireMessageData<T>(TcpSession session, T messageData)  where T: IMessage 
        {
            string type = typeof(T).FullName;
            if (_handlers.ContainsKey(type))
            {
                MessageHandler<T> handler = (MessageHandler<T>)_handlers[type];
                try
                {
                    handler?.Invoke(session, messageData);

                }
                catch (Exception ex)
                {
                   Console.WriteLine(ex.Message);
                    //打印错误日志
                    Console.WriteLine("messageRouter is error" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 关闭消息分发器
        /// </summary>
        public void StopService()
        {
            isRunning = false;
            _messageQueue.Clear();
            while (currentThreadCount > 0)
            {
                _autoEvent.Set();
            }
            Thread.Sleep(50);//考虑多线程，数据不一定同步
        }
    }
}
