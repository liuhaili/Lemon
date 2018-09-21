using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lemon.Extensions
{
    public static class ByteExtension
    {
        public static string ToHexString(this byte[] obj)
        {
            string returnStr = "";
            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    returnStr += obj[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
