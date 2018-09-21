namespace Lemon
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBase<T> where T : class, new()
    {
        private static T _Instance = null;
        private readonly static object _lock = new object();
        
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lock)
                    {
                        if (_Instance == null)
                            _Instance = new T();
                    }
                }
                return _Instance;
            }
        }
    }
}
