using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.RawSocket
{
    public class ClientConnect : ConnectBase
    {
        protected string IP;
        protected int Port;
        protected bool IsLongConnect;

        private IPEndPoint IPEndPoint;
        private SocketAsyncEventArgs ConnectArgs = new SocketAsyncEventArgs();

        public ClientConnect(bool isLongConnect) :base()
        {
            IsLongConnect = isLongConnect;
        }

        public async Task<bool> Connect<T>(string ip, int port) where T : IMessage
        {
            IP = ip;
            Port = port;
            MessageModel = typeof(T);
            
            IPEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            ConnectArgs.RemoteEndPoint = IPEndPoint;
            Socket = NewSocket();
            //开始自动连接
            return await AutoConnect();
        }

        private Socket NewSocket()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (IsLongConnect)
                socket.KeepAlive(1000, 1000);
            return socket;
        }

        private async Task<bool> AutoConnect()
        {
            if (Socket.Connected)
                return true;
            Func<Task<bool>> connectAction = LoopConnect;
            bool ret = await Task<bool>.Run(connectAction);
            if (ret)
            {
                base.Connected();
            }
            return ret;
        }

        private async Task<bool> LoopConnect()
        {
            while (true)
            {
                bool ret =await OnceConnect();
                if (!ret)
                {
                    Socket.Dispose();
                    if (IsLongConnect)
                    {
                        await Task.Delay(1000);
                        Socket = NewSocket();
                        //重新再连一次
                        continue;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
        }

        private async Task<bool> OnceConnect()
        {
            try
            {
                Socket.ConnectAsync(ConnectArgs);
            }
            catch (Exception ex)
            {
                if (OnErrorEvent != null)
                    OnErrorEvent(this, ex);
            }
            System.DateTime startConnectTime = DateTime.Now;
            while (true)
            {
                if (Socket.Connected || (DateTime.Now - startConnectTime).TotalSeconds > 2)
                    break;
                await Task.Delay(1);
            }
            return Socket.Connected;
        }

        public async Task<IMessage> SendAndBack(IMessage msg)
        {
            if (!Socket.Connected)
                return null;
            IMessage message = await Task.Run(async () =>
             {
                 IMessage backMsg = null;
                 bool isBack = false;
                 SetOnReceiveEvent((s, m) =>
                 {
                     backMsg = m;
                     isBack = true;
                 });
                 base.SendMessage(msg);
                 DateTime startTime = DateTime.Now;
                 while (true)
                 {
                     await Task.Delay(1);
                     //超时处理
                     if ((DateTime.Now - startTime).TotalSeconds > 10)
                     {
                         throw new Exception("通讯返回超时");
                     }
                     if (isBack)
                         return backMsg;
                 }
             });
            return message;
        }

        public void SetOnReceiveEvent(Action<ConnectBase, IMessage> onReceiveEvent)
        {
            if (IsLongConnect)
                OnReceiveEvent = onReceiveEvent;
            else
            {
                OnReceiveEvent = (s, m) =>
                {
                    onReceiveEvent(s, m);
                    Disconnect();
                };
            }
        }

        public override void Disconnect()
        {
            base.Disconnect();
            if (IsLongConnect)
            {
                //服务端断开了连接，重新开始连接
                AutoConnect();
            }
        }
    }
}
