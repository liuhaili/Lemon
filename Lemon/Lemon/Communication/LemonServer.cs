using Lemon.InvokeRoute;
using Lemon.RawSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Lemon.Communication
{
    /// <summary>
    /// 需要引用库System.Net.Sockets
    /// </summary>
    public class LemonServer : SingletonBase<LemonServer>
    {
        private SocketServer SocketServer;
        private ISerializeObject SerializeObject { get; set; }

        public void Start(int port, ISerializeObject serializeObject, Assembly assembly = null)
        {
            if (assembly != null)
                RouteManager.RegistAssembly(assembly);
            SerializeObject = serializeObject;

            SocketServer = new SocketServer();
            SocketServer.Start<LemonMessage>(port);
            SocketServer.SetOnReceiveEvent((s, m) =>
            {
                LemonMessage oldmsg = (LemonMessage)m;
                List<string> pars = oldmsg.Body.Split(new string[] { ParameterConverter.SplitString }, StringSplitOptions.None).ToList();
                string command = pars[0];
                pars.RemoveAt(0);
                try
                {
                    bool needBack = false;
                    object ret = RouteManager.ActionStringPars(command, pars.ToArray(), SerializeObject, ref needBack);
                    if (needBack)
                    {
                        LemonMessage msg = new LemonMessage();
                        msg.Body = ParameterConverter.PackOneParameter(ret, serializeObject);
                        msg.StateCode = 0;
                        s.SendMessage(msg);
                    }
                }
                catch (Exception ex)
                {
                    LemonMessage msg = new LemonMessage();
                    msg.Body = ex.Message;
                    msg.StateCode = -1;
                    s.SendMessage(msg);
                }
            });
        }

        public SocketServer GetServer() { return SocketServer; }
    }
}
