using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Lemon.RawSocket
{
    public abstract class ConnectBase
    {
        public Socket Socket;
        public string ConnectID { get; set; }
        public Type MessageModel { get; set; }
        public Type MessageModel2 { get; set; }
        public Action<ConnectBase, IMessage> OnReceiveEvent { get; set; }
        public Action<ConnectBase> OnConnectEvent { get; set; }
        public Action<ConnectBase> OnDisconnectEvent { get; set; }
        public Action<ConnectBase, Exception> OnErrorEvent { get; set; }

        protected BufferStream BufferStream;
        protected SocketAsyncEventArgs ReceiveSocketAsyncEventArgs;

        private readonly object SendLock = new object();

        public ConnectBase()
        {
            BufferStream = new BufferStream();
            ReceiveSocketAsyncEventArgs = SocketAsyncEventArgsPool.Pop();
        }

        protected void BeginLoopReceive(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (Socket == null || !Socket.Connected)
                return;
            socketAsyncEventArgs.Completed += OnReceived;
            if (!Socket.ReceiveAsync(socketAsyncEventArgs))
            {
                OnReceived(Socket, socketAsyncEventArgs);
            }
        }

        protected void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            e.Completed -= OnReceived;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                List<IMessage> msgList = null;
                try
                {
                    this.BufferStream.Write(e.Buffer, 0, e.BytesTransferred);
                    msgList = this.BufferStream.ReadMessage(MessageModel, MessageModel2);
                }
                catch (Exception ex)
                {
                    if (OnErrorEvent != null)
                        OnErrorEvent(this, ex);
                    //error:接收信息出错
                }
                if (msgList != null)
                {
                    foreach (var msg in msgList)
                    {
                        try
                        {
                            if (OnReceiveEvent != null)
                                OnReceiveEvent(this, msg);
                        }
                        catch (Exception ex)
                        {
                            if (OnErrorEvent != null)
                                OnErrorEvent(this, ex);
                            //error:接收信息后，事件处理出错
                        }
                    }
                }
                BeginLoopReceive(e);
            }
            else
            {
                Disconnect();
            }
        }

        public virtual void Connected()
        {
            try
            {
                if (OnConnectEvent != null)
                    OnConnectEvent(this);
            }
            catch (Exception ex)
            {
                if (OnErrorEvent != null)
                    OnErrorEvent(this, ex);
                //error: 连接后，事件处理出错
            }
            BeginLoopReceive(ReceiveSocketAsyncEventArgs);
        }

        public virtual void SendMessage(IMessage message)
        {
            lock (SendLock)
            {
                if (!Socket.Connected)
                {
                    throw new Exception("连接已断开，发送信息出错");
                }
                Socket.Send(message.ToBytes());
            }
        }

        public virtual void SendData(byte[] data)
        {
            lock (SendLock)
            {
                if (!Socket.Connected)
                {
                    throw new Exception("连接已断开，发送信息出错");
                }
                Socket.Send(data);
            }
        }

        public virtual void Disconnect()
        {
            SocketAsyncEventArgsPool.Push(ReceiveSocketAsyncEventArgs);
            try
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Dispose();
            }
            catch (Exception ex)
            {
                if (OnErrorEvent != null)
                    OnErrorEvent(this, ex);
                //error:断开连接出错
            }
            try
            {
                if (OnDisconnectEvent != null)
                    OnDisconnectEvent(this);
            }
            catch (Exception ex)
            {
                if (OnErrorEvent != null)
                    OnErrorEvent(this, ex);
                //error:断开连接事件处理出错
            }
        }
    }
}
