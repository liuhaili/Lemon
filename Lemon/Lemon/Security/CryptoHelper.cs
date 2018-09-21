using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lemon.Security
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public class CryptoHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, Encoding encoding, string addKey = null)
        {
            if (addKey != null && addKey.Length > 0)
            {
                source += addKey;
            }
            byte[] bytes = encoding.GetBytes(source);
            return MD5_Encrypt(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string MD5_Encrypt(byte[] bytes)
        {
            using (MD5 mD = MD5.Create())
            {
                byte[] array = mD.ComputeHash(bytes);
                string text = null;
                for (int i = 0; i < array.Length; i++)
                {
                    string text2 = array[i].ToString("x");
                    if (text2.Length == 1)
                    {
                        text2 = "0" + text2;
                    }
                    text += text2;
                }
                return text;
            }
        }

        /// <summary>
        /// 获取文件的MD5 Hash值
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ToFileMd5Hash(string fileName)
        {
            String hashMD5 = String.Empty;
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    //计算文件的MD5值
                    MD5 calculator = MD5.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Dispose();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashMD5 = stringBuilder.ToString();
                }
            }
            return hashMD5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMd5Hash(string str)
        {
            return ToMd5Hash(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToMd5Hash(byte[] bytes)
        {
            String hashMD5;
            //计算文件的MD5值
            MD5 calculator = MD5.Create();
            Byte[] buffer = calculator.ComputeHash(bytes);
            calculator.Dispose();
            //将字节数组转换成十六进制的字符串形式
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                stringBuilder.Append(buffer[i].ToString("x2"));
            }
            hashMD5 = stringBuilder.ToString();
            return hashMD5;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SHA1_Encrypt(string input, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input", "Sha1加密的字符串不能为空！");

            if (encoding == null)
                encoding = Encoding.UTF8;

            var data = encoding.GetBytes(input);
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("bytes", "Sha1加密的字节不能为空！");

            using (SHA1 sha1Hash = SHA1.Create())
            {
                var encryData = sha1Hash.ComputeHash(data);
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < encryData.Length; i++)
                {
                    sBuilder.Append(encryData[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HttpBase64Encode(string source)
        {
            if (source == null || source.Length == 0)
            {
                return "";
            }
            string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
            text = text.Replace("+", "~");
            text = text.Replace("/", "@");
            return text.Replace("=", "$");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HttpBase64Decode(string source)
        {
            if (source == null || source.Length == 0)
            {
                return "";
            }
            string text = source.Replace("~", "+");
            text = text.Replace("@", "/");
            text = text.Replace("$", "=");
            return Encoding.UTF8.GetString(Convert.FromBase64String(text));
        }
    }
}