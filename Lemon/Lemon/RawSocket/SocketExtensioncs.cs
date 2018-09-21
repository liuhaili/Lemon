using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Lemon.RawSocket
{
    public static class SocketExtensioncs
    {
        public static void KeepAlive(this Socket socket, int keepAliveTime, int keepAliveInterval)
        {
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);//是否启用Keep-Alive
            BitConverter.GetBytes((uint)keepAliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间开始第一次探测
            BitConverter.GetBytes((uint)keepAliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);//探测时间间隔            
            //socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, 1);
            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }
    }
}
