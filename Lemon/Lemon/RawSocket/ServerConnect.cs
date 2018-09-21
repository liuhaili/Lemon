using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Lemon.RawSocket
{
    public class ServerConnect : ConnectBase
    {
        private SocketServer SocketServer;
        public ServerConnect(SocketServer socketServer, Socket socket, Type type,
            Action<ConnectBase, IMessage> onReceiveEvent,
            Action<ConnectBase> onConnectEvent,
            Action<ConnectBase> onDisconnectEvent,
            Action<ConnectBase,Exception> onErrorEvent) : base()
        {
            SocketServer = socketServer;
            MessageModel = type;
            Socket = socket;
            OnReceiveEvent = onReceiveEvent;
            OnConnectEvent = onConnectEvent;
            OnDisconnectEvent = onDisconnectEvent;
            OnErrorEvent = onErrorEvent;
            ReceiveSocketAsyncEventArgs.RemoteEndPoint = Socket.RemoteEndPoint;
        }

        public SocketServer GetServer()
        {
            return SocketServer;
        }

        public override void Connected()
        {
            SocketServer.AddConnect(this);
            base.Connected();
        }

        public override void Disconnect()
        {
            base.Disconnect();
            SocketServer.RemoveConnect(this);
        }
    }
}
