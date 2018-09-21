using System;

namespace Lemon
{
    /// <summary>
    /// 序列化工具接口
    /// </summary>
    public interface ISerializeObject
    {
        Byte[] Serialize(object obj);
        object Deserialize(Byte[] data, Type type);
        string SerializeToString(object obj);
        object DeserializeFromString(string data, Type type);
    }
}
