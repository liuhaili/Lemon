using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lemon.Extensions
{
    public class EnumInfo
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    public static class EnumExtension
    {
        public static Dictionary<Type, List<EnumInfo>> EnumCache = new Dictionary<Type, List<EnumInfo>>();

        public static int ToInt(this Enum obj)
        {
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 枚举字段需要标记EnumDescriptionAttribute("xxx")
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<EnumInfo> EnumMap(this Enum obj)
        {
            Type type = obj.GetType();
            MakeSureInCache(obj, type);
            return EnumCache[type];
        }

        public static EnumInfo GetMyInfo(this Enum obj)
        {
            Type type = obj.GetType();
            MakeSureInCache(obj, type);
            return EnumCache[type].FirstOrDefault(c => c.Value == Convert.ToInt32(obj));
        }

        public static EnumInfo GetOtherInfo(this Enum obj, string description)
        {
            Type type = obj.GetType();
            MakeSureInCache(obj, type);
            return EnumCache[type].FirstOrDefault(c => c.Description == description);
        }

        public static EnumInfo GetOtherInfo(this Enum obj, int val)
        {
            Type type = obj.GetType();
            MakeSureInCache(obj, type);
            return EnumCache[type].FirstOrDefault(c => c.Value == val);
        }

        private static void MakeSureInCache(Enum obj, Type type)
        {
            if (!EnumCache.ContainsKey(type))
            {
                List<EnumInfo> list = new List<EnumInfo>();
                foreach (FieldInfo field in type.GetRuntimeFields())
                {
                    if (!field.IsStatic)
                        continue;
                    EnumInfo enumInfo = new EnumInfo();
                    var attrs = field.GetCustomAttribute<EnumDescriptionAttribute>();
                    if (attrs != null)
                        enumInfo.Description = attrs.Description;
                    enumInfo.Name = field.Name;
                    enumInfo.Value = (int)field.GetValue(obj);
                    list.Add(enumInfo);
                }
                EnumCache.Add(type, list);
            }
        }
    }
}
