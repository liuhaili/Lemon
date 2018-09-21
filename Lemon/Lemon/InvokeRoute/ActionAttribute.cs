using System;

namespace Lemon.InvokeRoute
{
    /// <summary>
    /// 将一个方法标记为一个Action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ActionAttribute : Attribute
    {
    }
}
