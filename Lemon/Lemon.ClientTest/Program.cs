using System;
using System.Collections.Generic;
using System.Text;
using Lemon.Extensions;
using Lemon.InvokeRoute;
using System.Reflection;
using Lemon.Communication;
using Newtonsoft.Json;
using LampMonitor.Entity;
using Lemon.CacheDB;
using Lemon.RawSocket;
using System.Threading.Tasks;
using System.Threading;
using LampMonitor;

namespace Lemon.Test
{
    public enum SType
    {
        [EnumDescriptionAttribute("新的")]
        New = 1,
        [EnumDescriptionAttribute("旧的")]
        Old = 2
    }
    class Program
    {
        static void Main(string[] args)
        {
            ////SType stype = SType.New;
            ////EnumInfo list = stype.GetOtherInfo(2);
            //RouteManager.RegistAssembly(Assembly.GetCallingAssembly());
            ////TestController tc = new TestController();
            ////RouteManager.RegistMethod("TestController/GetAge", tc, "GetAge");

            //object age = RouteManager.Action("TestController/GetAge", "haili", 35);
            //Console.WriteLine("返回值" + age);

            //Test t = new Test();
            //t.GetName();

            //DBTable table = DBManager.Instance.GetTable(t.GetType());


            //new Test().GetAge();

            //new Test().GetName();

            //new Test().TestLongSpeed();

            new Test().GetTest();
            Console.Read();
        }

        
    }

    public class Test
    {
        public async void GetName()
        {
            LemonClient client = new LemonClient("60.205.210.198", 4050, new JsonSerialize());
            List<ELamp> lampList = await client.Request<List<ELamp>>("LampService/ListLmapByConcentrator", "concentrator1");
            Console.WriteLine("back:");

            //ClientConnect client = new ClientConnect();
            //client.OnConnectEvent = c =>
            //{
            //    Console.WriteLine("连接到了服务器");
            //};
            //client.OnDisconnectEvent = c =>
            //{
            //    Console.WriteLine("和服务器的连接断开了");
            //};
            //bool ok = await client.Connect<LemonMessage>("127.0.0.1", 4506, true);
            //if (ok)
            //    Console.WriteLine("back:");
        }

        public async void TestAge()
        {
            DateTime ntime = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                await new Test().GetAge();
            }
            Console.WriteLine("总耗时：" + (DateTime.Now - ntime).TotalMilliseconds);
        }

        public async Task<bool> GetAge()
        {
            try
            {
                LemonClient client = new LemonClient("127.0.0.1", 4506, new JsonSerialize());
                string lampList = await client.Request<string>("TestController/GetAge", "haili", 35);
                Console.WriteLine("back:" + lampList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }
            return true;
        }

        public async void TestLongSpeed()
        {
            ClientConnect client = new ClientConnect(true);
            await client.Connect<Msg>("60.205.210.198", 4120);
            client.OnConnectEvent = c =>
            {
                Console.WriteLine("连接到服务器上了");
            };
            client.OnDisconnectEvent = c =>
            {
                Console.WriteLine("连接断开了");
            };

            client.OnReceiveEvent=(c, m) => 
            {
                Console.WriteLine("receive:"+m.ToBytes().ToHexString());
                //c.SendMessage(Msg.GetOpenLampBackMsg("22180117000000".ToHexByte()));
            };
        }

        public async void GetTest()
        {
            string back=await Task.Run(bb);
            Console.WriteLine(back);
        }

        private async Task<string> bb()
        {
            await Task.Delay(8000);
            return "haili";
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
