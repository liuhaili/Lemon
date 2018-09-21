using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lemon.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string obj)
        {
            return String.IsNullOrEmpty(obj);
        }

        /// <summary>
        /// 判断二个字符串是否相等，忽略大小写的比较方式。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsSame(this string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// 截取固定长度的字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">长度</param>
        /// <param name="dot">是否显示“...”</param>
        /// <returns></returns>
        public static string GetFixLenString(this string obj, int len, bool dot)
        {
            if (obj.IsNullOrEmpty())
                return "";
            obj = obj.Trim();
            int bytes = len * 2;
            len = 0;
            StringBuilder s = new StringBuilder(bytes);
            foreach (char c in obj)
            {
                len += Encoding.UTF8.GetByteCount(c.ToString());
                if (len <= bytes)
                    s.Append(c);
                else
                    break;
            }
            if (dot)
            {
                if (s.Length != obj.Length)
                    s.Append(" …");
            }
            return s.ToString();
        }

        public static byte[] ToHexByte(this string obj)
        {
            obj = obj.Replace(" ", "");
            if ((obj.Length % 2) != 0)
                obj += " ";
            byte[] returnBytes = new byte[obj.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(obj.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string[] SplitString(this string obj, string split)
        {
            return obj.Split(new string[] { split }, System.StringSplitOptions.None);
        }

        #region "验证扩展"
        /// <summary>
        /// 检查是否是正确的Email地址
        /// </summary>
        public static bool IsEmail(this string obj)
        {
            return Regex.IsMatch(obj, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        /// <summary>
        /// 检查是否是正确的URL地址
        /// </summary>
        public static bool IsUrl(this string obj)
        {
            return Regex.IsMatch(obj, @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
        }

        /// <summary>
        /// 检查是否是英文
        /// </summary>
        public static bool IsEnglish(this string obj)
        {
            return Regex.IsMatch(obj, @"^[A-Za-z]+$");
        }

        /// <summary>
        /// 检查是否是合法的用户名:字母开头，允许5-20字节，允许字母数字下划线
        /// </summary>
        public static bool IsUserName(this string obj)
        {
            return Regex.IsMatch(obj, @"^[a-zA-Z][a-zA-Z0-9_.]{5,9}$");
        }

        /// <summary>
        /// 检查是否合法的密码：除，6－16个字
        /// </summary>
        public static bool IsPassword(this string obj)
        {
            return Regex.IsMatch(obj, @"^[a-zA-Z0-9]{6,9}$");
        }
        /// <summary>
        /// 检查是否合法的名称：字母＋汉字＋数字，1－20个字
        /// </summary>
        public static bool IsCommonName(this string obj)
        {
            return Regex.IsMatch(obj, @"^[\u4e00-\u9fa5a-zA-Z0-9\s]{1,20}$");
        }

        /// <summary>
        /// 检查是否是中文
        /// </summary>
        public static bool IsChinese(this string obj)
        {
            return Regex.IsMatch(obj, @"^[\u4e00-\u9fa5]{1,9}$");
        }

        /// <summary>
        /// 检查是否是数字
        /// </summary>
        public static bool IsNumeric(this string obj)
        {
            foreach (char c in obj)
            {
                if (c > '9' || c < '0')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否是IP地址
        /// </summary>
        public static bool IsIP(this string obj)
        {
            if (Regex.IsMatch(obj, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$") == true)
            {
                string[] arr_IP = Regex.Split(obj, @"[.]+");
                if ((arr_IP[0].CompareTo("0") > 0 && (arr_IP[0].Length < 3 || arr_IP[0].CompareTo("256") < 0)) &&
                    (arr_IP[1].CompareTo("0") >= 0 && (arr_IP[1].Length < 3 || arr_IP[1].CompareTo("256") < 0)) &&
                    (arr_IP[2].CompareTo("0") >= 0 && (arr_IP[2].Length < 3 || arr_IP[2].CompareTo("256") < 0)) &&
                    (arr_IP[3].CompareTo("0") >= 0 && (arr_IP[3].Length < 3 || arr_IP[3].CompareTo("256") < 0))
                    )
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 判断IP地址是否为内网IP地址
        /// 私有IP：
        /// A类  10.0.0.0-10.255.255.255
        /// B类  172.16.0.0-172.31.255.255
        /// C类  192.168.0.0-192.168.255.255
        /// 当然，还有127这个网段是环回地址
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        private static bool IsInnerIP(this string obj)
        {
            bool isInnerIp = false;
            long ipNum = GetIpNum(obj);
            long aBegin = GetIpNum("10.0.0.0");
            long aEnd = GetIpNum("10.255.255.255");
            long bBegin = GetIpNum("172.16.0.0");
            long bEnd = GetIpNum("172.31.255.255");
            long cBegin = GetIpNum("192.168.0.0");
            long cEnd = GetIpNum("192.168.255.255");
            isInnerIp = IsInner(ipNum, aBegin, aEnd) || IsInner(ipNum, bBegin, bEnd) || IsInner(ipNum, cBegin, cEnd) || obj.Equals("127.0.0.1");
            return isInnerIp;
        }

        /// <summary>
        /// 把IP地址转换为Long型数字
        /// </summary>
        /// <param name="ipAddress">IP地址字符串</param>
        /// <returns></returns>
        private static long GetIpNum(String ipAddress)
        {
            String[] ip = ipAddress.Split('.');
            long a = int.Parse(ip[0]);
            long b = int.Parse(ip[1]);
            long c = int.Parse(ip[2]);
            long d = int.Parse(ip[3]);
            long ipNum = a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d;
            return ipNum;
        }

        /// <summary>
        /// 判断用户IP地址转换为Long型后是否在内网IP地址所在范围
        /// </summary>
        /// <param name="userIp"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static bool IsInner(long userIp, long begin, long end)
        {
            return (userIp >= begin) && (userIp <= end);
        }
        #endregion
    }
}
