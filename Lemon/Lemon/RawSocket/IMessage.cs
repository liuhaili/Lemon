using System.Collections.Generic;

namespace Lemon.RawSocket
{
    public interface IMessage
    {
        /// <summary>
        /// 如果不成功返回0，成功返回具体的数字
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        int FromBytes(List<byte> bytes);
        byte[] ToBytes();
    }
}
