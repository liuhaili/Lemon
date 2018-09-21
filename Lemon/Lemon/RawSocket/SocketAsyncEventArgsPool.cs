using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Lemon.RawSocket
{
    public class SocketAsyncEventArgsPool
    {
        private static readonly object PoolLock = new object();
        private static Queue<SocketAsyncEventArgs> Pool = new Queue<SocketAsyncEventArgs>();

        static SocketAsyncEventArgsPool()
        {
            lock (PoolLock)
            {
                for (int i = 0; i < 1000; i++)
                {
                    Pool.Enqueue(NewSocketAsyncEventArgs());
                }
            }
        }

        public static SocketAsyncEventArgs Pop()
        {
            lock (PoolLock)
            {
                if (Pool.Count == 0)
                {
                    Pool.Enqueue(NewSocketAsyncEventArgs());
                }
                return Pool.Dequeue();
            }
        }

        public static void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
                return;
            lock (PoolLock)
            {
                item.UserToken = null;
                item.AcceptSocket = null;
                Pool.Enqueue(item);
            }
        }

        private static SocketAsyncEventArgs NewSocketAsyncEventArgs()
        {
            SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
            socketAsyncEventArgs.SetBuffer(new byte[1024], 0, 1024);
            return socketAsyncEventArgs;
        }
    }
}
