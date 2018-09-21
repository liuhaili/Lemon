using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Lemon.RawSocket
{
    public class SocketServer
    {
        private Socket Socket;
        private Type MessageModel;
        private Type MessageModel2;
        private List<ConnectBase> ConnectList = new List<ConnectBase>();
        private readonly object ClientSocketListLock = new object();

        protected Action<ConnectBase, IMessage> OnReceiveEvent;
        protected Action<ConnectBase> OnConnectEvent;
        protected Action<ConnectBase> OnDisconnectEvent;
        protected Action<ConnectBase, Exception> OnErrorEvent;

        public void Start<T>(int port) where T : IMessage
        {
            MessageModel = typeof(T);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Socket.KeepAlive(5000, 5000);
            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                Socket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {
                Socket.Bind(localEndPoint);
            }
            Socket.Listen(5000);
            StartAccept();
        }

        public void Start<T,D>(int port) where T : IMessage
        {
            MessageModel = typeof(T);
            MessageModel2= typeof(D);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Socket.KeepAlive(5000, 5000);
            if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                Socket.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
            }
            else
            {
                Socket.Bind(localEndPoint);
            }
            Socket.Listen(5000);
            StartAccept();
        }

        void StartAccept()
        {
            Task acceptTask = new Task(AcceptConnect);
            acceptTask.Start();
        }

        void AcceptConnect()
        {
            while (true)
            {
                Socket client = Socket.Accept();
                ServerConnect serverConnect = new ServerConnect(this, client, MessageModel, OnReceiveEvent, OnConnectEvent, OnDisconnectEvent, OnErrorEvent);
                serverConnect.MessageModel2 = MessageModel2;
                serverConnect.Connected();
            }
        }

        internal void AddConnect(ConnectBase connect)
        {
            lock (ClientSocketListLock)
            {
                ConnectList.Add(connect);
            }
        }

        internal void RemoveConnect(ConnectBase connect)
        {
            lock (ClientSocketListLock)
            {
                ConnectList.Remove(connect);
            }
        }

        public ConnectBase[] AllConnect()
        {
            lock (ClientSocketListLock)
            {
                return ConnectList.ToArray();
            }
        }

        public ConnectBase GetConnect(string connectID)
        {
            lock (ClientSocketListLock)
            {
                return ConnectList.LastOrDefault(c => c.ConnectID == connectID);
            }
        }

        /// <summary>
        /// 此方法临时使用
        /// </summary>
        /// <param name="connect"></param>
        public void ClearOtherSameConnect(string connectid)
        {
            List<ConnectBase> willremove;
            lock (ClientSocketListLock)
            {
                willremove = ConnectList.Where(c => c.ConnectID == connectid).ToList();
            }
            foreach (var s in willremove)
            {
                s.Disconnect();
            }

        }

        public void SendMessage(string connectID, IMessage message)
        {
            ConnectBase socketSession = GetConnect(connectID);
            if (socketSession == null)
            {
                throw new Exception("未找到链接"+connectID);
            }
            socketSession.SendMessage(message);
        }

        public void SendData(string connectID, byte[] data)
        {
            ConnectBase socketSession = GetConnect(connectID);
            if (socketSession == null)
            {
                throw new Exception("未找到链接" + connectID);
            }
            socketSession.SendData(data);
        }

        public void SetOnReceiveEvent(Action<ConnectBase, IMessage> onReceiveEvent)
        {
            OnReceiveEvent = onReceiveEvent;
        }

        public void SetOnConnectEvent(Action<ConnectBase> onConnectEvent)
        {
            OnConnectEvent = onConnectEvent;
        }

        public void SetOnDisconnectEvent(Action<ConnectBase> onDisconnectEvent)
        {
            OnDisconnectEvent = onDisconnectEvent;
        }

        public void SetOnErrorEvent(Action<ConnectBase, Exception> onErrorEvent)
        {
            OnErrorEvent = onErrorEvent;
        }
    }
}
