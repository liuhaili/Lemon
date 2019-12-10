using Lemon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lemon.CacheDB
{
    public class DBTable
    {
        private readonly object TableLock = new object();
        private Dictionary<int, IData> ObjectList;

        public DBTable()
        {
            ObjectList = new Dictionary<int, IData>();
        }

        public void Load<T>(List<T> data) where T : IData
        {
            lock (TableLock)
            {
                ObjectList.Clear();
                int simulateId = 0;
                foreach (T d in data)
                {
                    try
                    {
                        if (d.ID == 0)
                        {
                            simulateId++;
                            ObjectList.Add(simulateId, d);
                        }
                        else
                            ObjectList.Add(d.ID, d);
                    }
                    catch
                    {
                        throw new Exception("请检查" + typeof(T) + " ID" + d.ID + "是否重复！");
                    }
                }
            }
        }

        public void Load(List<object> data)
        {
            lock (TableLock)
            {
                ObjectList.Clear();
                int simulateId = 0;
                foreach (var da in data)
                {
                    IData d = da as IData;
                    try
                    {
                        if (d.ID == 0)
                        {
                            simulateId++;
                            ObjectList.Add(simulateId, d);
                        }
                        else
                            ObjectList.Add(d.ID, d);
                    }
                    catch
                    {
                        throw new Exception("请检查" + d.GetType() + " ID" + d.ID + "是否重复！");
                    }
                }
            }
        }

        public IEnumerable<T> GetAll<T>() where T : IData
        {
            lock (TableLock)
            {
                return ObjectList.Values.Select(c => c as T).AsEnumerable();
            }
        }

        public IEnumerable<object> GetAll()
        {
            lock (TableLock)
            {
                return ObjectList.Values;
            }
        }

        public void Add<T>(T obj) where T : IData
        {
            lock (TableLock)
            {
                if (ObjectList.ContainsKey(obj.ID))
                    return;
                ObjectList.Add(obj.ID, obj);
            }
        }

        public void Save(object obj)
        {
            IData data = obj as IData;
            lock (TableLock)
            {
                if (ObjectList.ContainsKey(data.ID))
                {
                    ObjectList[data.ID] = data;
                }
                else
                    ObjectList.Add(data.ID, data);
            }
        }

        public int Update<T>(T t) where T : IData
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(t.ID))
                    return -1;
                ObjectList[t.ID] = t;
                return 0;
            }
        }

        public int Delete(int id)
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return -1;
                ObjectList.Remove(id);
                return 0;
            }
        }

        public int Delete<T>(T t) where T : IData
        {
            return Delete(t.ID);
        }

        public T Get<T>(int id) where T : IData
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return null;
                T newt = ObjectList[id] as T;
                return newt;
            }
        }

        public object Get(int id)
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return null;
                return ObjectList[id];
            }
        }

        public IEnumerable<T> Select<T>(Func<T, bool> predicate = null) where T : IData
        {
            lock (TableLock)
            {
                IEnumerable<T> result = null;
                if (predicate != null)
                    result = ObjectList.Values.Select(c => c as T).Where(predicate);
                else
                    result = ObjectList.Values.Select(c => c as T);
                return result;
            }
        }

        public int Count<T>(Func<T, bool> predicate = null) where T : IData
        {
            lock (TableLock)
            {
                if (predicate == null)
                    return ObjectList.Values.Count();
                return ObjectList.Values.Select(c => c as T).Count(predicate);
            }
        }

        public void Clear()
        {
            lock (TableLock)
            {
                ObjectList.Clear();
            }
        }
    }
}
