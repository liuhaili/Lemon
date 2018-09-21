using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Lemon.Extensions
{
    public static class ObjectExtension
    {
        public static int ToInt(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static int ToInt(this object obj, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal ToDecimal(this object obj, int defaultValue)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long ToLong(this object obj)
        {
            return Convert.ToInt64(obj);
        }

        public static long ToLong(this object obj, long defaultValue)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime ToDateTime(this object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static bool ToBoolean(this object obj, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static object GetProperty(this object obj, string name)
        {
            PropertyInfo pInfo = obj.GetType().GetRuntimeProperty(name);
            if (pInfo == null)
                throw new Exception("请检查" + obj.GetType().Name + "属性！" + name);
            return pInfo.GetMethod.Invoke(obj, null);
        }

        public static void SetProperty(this object obj, string name, object val)
        {
            obj.GetType().GetRuntimeProperty(name).SetValue(obj, val, null);
        }

        public static string GetName<T>(this T obj, Expression<Func<T, T>> exp)
        {
            return ((MemberExpression)exp.Body).Member.Name;
        }
    }
}
