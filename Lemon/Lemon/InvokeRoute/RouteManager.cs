using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Lemon.Communication;

namespace Lemon.InvokeRoute
{
    /// <summary>
    /// 自动注册所有程序集，需类继承IActionController，方法需要标记属性ActionAttribute
    /// </summary>
    public class RouteManager
    {
        private static Dictionary<string, ActionInfo> ActionDictionary = new Dictionary<string, ActionInfo>();

        public static void RegistAssembly(string assemblyname)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyname));
            RegistAssembly(assembly);
        }

        public static void RegistAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.ExportedTypes)
            {
                RegistController(t);
            }
        }

        public static void RegistController(Type t)
        {
            if (!t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IActionController)))
                return;
            object controller = Activator.CreateInstance(t);
            string controllername = t.Name;
            foreach (MethodInfo m in t.GetRuntimeMethods())
            {
                ActionAttribute actionAttr = m.GetCustomAttribute<ActionAttribute>();
                if (actionAttr != null)
                {
                    RegistMethodInfo(controllername + "/" + m.Name, controller, m);
                }
            }
        }

        public static void RegistMethod(string command, object controller, string methodName)
        {
            Type t = controller.GetType();
            foreach (MethodInfo m in t.GetRuntimeMethods())
            {
                if (m.Name != methodName)
                    continue;
                RegistMethodInfo(command, controller, m);
                break;
            }
        }

        public static void UnRegistMethod(string command)
        {
            if (ActionDictionary.ContainsKey(command))
                ActionDictionary.Remove(command);
        }

        private static void RegistMethodInfo(string command, object controller, MethodInfo method)
        {
            ActionInfo action = new ActionInfo() { ControllerObject = controller, MethodInfo = method };
            ActionDictionary.Add(command, action);
        }

        public static object Action(string command, params object[] pars)
        {
            if (!ActionDictionary.ContainsKey(command))
                return null;
            ActionInfo actionInfo = ActionDictionary[command];
            if (actionInfo.ControllerObject == null)
                return null;
            object[] convertByTypePars = GetActionCallParameters(actionInfo, pars);
            return actionInfo.MethodInfo.Invoke(actionInfo.ControllerObject, convertByTypePars);
        }

        public static object ActionStringPars(string command, string[] pars, ISerializeObject serializeObject, ref bool needBack)
        {
            if (!ActionDictionary.ContainsKey(command))
            {
                needBack = false;
                return null;
            }
            ActionInfo actionInfo = ActionDictionary[command];
            if (actionInfo.ControllerObject == null)
            {
                needBack = false;
                return null;
            }
            object[] convertByTypePars = GetActionCallParameters(actionInfo, pars, serializeObject);
            if (actionInfo.MethodInfo.ReturnType == typeof(void))
                needBack = false;
            else
                needBack = true;
            return actionInfo.MethodInfo.Invoke(actionInfo.ControllerObject, convertByTypePars);
        }

        private static object[] GetActionCallParameters(ActionInfo action, object[] pars, ISerializeObject serializeObject = null)
        {
            if (pars == null || pars.Length == 0)
                return null;
            object[] parameters = new object[pars.Length];
            ParameterInfo[] methodParameters = action.MethodInfo.GetParameters();
            for (int i = 0; i < pars.Length; i++)
            {
                if (i >= methodParameters.Length)
                    continue;
                ParameterInfo p = methodParameters[i];
                if (p.IsOut)
                    continue;
                if (p.ParameterType == typeof(void))
                    continue;
                Type paramterType = p.ParameterType;
                object val = pars[i];
                if (val != null)
                {
                    if (serializeObject == null)
                        parameters[i] = val;
                    else
                        parameters[i] = ParameterConverter.UnpackOneParameter(val.ToString(), paramterType, serializeObject);
                }
                else
                {
                    throw new ArgumentException("未能找到指定的参数值：" + p.Name);
                }
            }
            return parameters;
        }
    }
}
