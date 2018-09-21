using System;
using System.Text;
using Lemon.RawSocket;
using System.Reflection;
using System.Threading.Tasks;

namespace Lemon.Communication
{
    /// <summary>
    /// 需要引用库System.Net.Sockets
    /// </summary>
    public class LemonClient
    {
        private string IP { get; set; }
        private int Port { get; set; }
        private ISerializeObject SerializeObject { get; set; }

        public LemonClient(string ip, int port, ISerializeObject serializeObject)
        {
            IP = ip;
            Port = port;
            SerializeObject = serializeObject;
        }

        public async Task<T> Request<T>(string command, params object[] pars)
        {
            ClientConnect client = new ClientConnect(false);
            bool ret = await client.Connect<LemonMessage>(IP, Port);
            if (!ret)
            {
                throw new Exception("网络连接失败");
            }
            string sendParStr = ParameterConverter.PackParameter(command, SerializeObject, pars);
            LemonMessage message = (LemonMessage)await client.SendAndBack(new LemonMessage() { Body = sendParStr });
            if (message == null)
                return default(T);
            return ParameterConverter.UnpackOneParameter<T>(message.Body, SerializeObject);
        }
    }
}
