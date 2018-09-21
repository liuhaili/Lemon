using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Lemon.RawSocket
{
    public class BufferStream
    {
        private List<byte> Buffer = new List<byte>();
        private DateTime? LastWriteTime = null;

        public void Write(byte[] inbuffer, int offset, int count)
        {
            //清除过时的信息
            if (LastWriteTime.HasValue)
            {
                if ((DateTime.Now - LastWriteTime.Value).TotalSeconds > 10)
                    Buffer.Clear();
            }
            LastWriteTime = DateTime.Now;

            for (int i = offset; i < count; i++)
            {
                if (i >= inbuffer.Length)
                    break;
                Buffer.Add(inbuffer[i]);
            }
        }

        public List<IMessage> ReadMessage(Type type)
        {
            List<IMessage> msgList = new List<IMessage>();
            while (true)
            {
                IMessage msg = (IMessage)Activator.CreateInstance(type);
                int count = msg.FromBytes(Buffer);
                if (count == 0)
                    break;
                Remove(count);
                msgList.Add(msg);
            }
            return msgList;
        }

        public List<IMessage> ReadMessage(Type type, Type type2)
        {
            List<IMessage> msgList = new List<IMessage>();
            while (true)
            {
                IMessage msg = (IMessage)Activator.CreateInstance(type);
                int count = msg.FromBytes(Buffer);
                if (count != 0)
                {
                    Remove(count);
                    msgList.Add(msg);
                }
                else if (type2 != null)
                {
                    IMessage msg2 = (IMessage)Activator.CreateInstance(type2);
                    int count2 = msg2.FromBytes(Buffer);
                    if (count2 != 0)
                    {
                        Remove(count2);
                        msgList.Add(msg2);
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return msgList;
        }

        private void Remove(int count)
        {
            Buffer.RemoveRange(0, count);
        }
    }
}
