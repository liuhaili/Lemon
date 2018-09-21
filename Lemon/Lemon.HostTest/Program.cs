using Lemon.Communication;
using Lemon.RawSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lemon.HostTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LemonServer.Instance.Start(4506, new JsonSerialize(), Assembly.GetCallingAssembly());
            LemonServer.Instance.GetServer().SetOnConnectEvent(s =>
            {
                Console.WriteLine("有个连接连接上来了");
            });
            LemonServer.Instance.GetServer().SetOnDisconnectEvent(s =>
            {
                Console.WriteLine("有个连接断开了");
            });

            LemonServer.Instance.GetServer().SetOnErrorEvent((s,e) =>
            {
                Console.WriteLine("出错了"+e.Message);
            });


            //SocketServer server = new SocketServer();
            //server.Start<LemonMessage>(4506);

            Console.Read();
        }
    }

    public class JsonSerialize : ISerializeObject
    {
        public object Deserialize(byte[] data, Type type)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFromString(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type);
        }

        public byte[] Serialize(object obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
