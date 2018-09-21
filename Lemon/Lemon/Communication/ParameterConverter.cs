using System;
using System.Reflection;
using System.Text;

namespace Lemon.Communication
{
    public class ParameterConverter
    {
        public const string SplitString = "*|*";
        public static string PackParameter(string command, ISerializeObject serializeObject, params object[] pars)
        {
            StringBuilder sendParameter = new StringBuilder();
            sendParameter.Append(command).Append(SplitString);
            for (int i = 0; i < pars.Length; i++)
            {
                sendParameter.Append(PackOneParameter(pars[i], serializeObject)).Append(SplitString);
            }
            return sendParameter.ToString().TrimEnd(SplitString.ToCharArray());
        }

        public static string PackOneParameter(object obj, ISerializeObject serializeObject)
        {
            if (obj == null)
            {
                return "null";
            }
            string backStr = "";
            Type type = obj.GetType();
            if (!type.GetTypeInfo().IsClass
                || type == typeof(string)
                || type == typeof(DateTime)
                || type.GetTypeInfo().BaseType == typeof(System.Type)
                || type.GetTypeInfo().IsEnum
                || type == typeof(int)
                || type == typeof(float)
                )
                backStr = obj.ToString();
            else
            {
                if (serializeObject == null)
                    backStr = obj.ToString();
                else
                    backStr = serializeObject.SerializeToString(obj);
            }
            return backStr;
        }

        public static object UnpackOneParameter(string msg, Type type, ISerializeObject serializeObject)
        {
            if (type == typeof(string)
                || type == typeof(DateTime)
                || type.GetTypeInfo().BaseType == typeof(System.Type)
                || type == typeof(int)
                || type == typeof(float)
                )
                return Convert.ChangeType(msg, type);
            else if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, msg);
            else
            {
                if (msg == null)
                    return null;
                else
                {
                    if (serializeObject == null)
                        return null;
                    object data = serializeObject.DeserializeFromString(msg, type);
                    return data;
                }
            }
        }

        public static T UnpackOneParameter<T>(string msg, ISerializeObject serializeObject)
        {
            return (T)UnpackOneParameter(msg, typeof(T), serializeObject);
        }
    }
}
