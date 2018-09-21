using Lemon.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lemon.CacheDB
{
    public class DBTable
    {
        private readonly object TableLock = new object();
        private const string idname = "ID";
        private Dictionary<int, object> ObjectList;
        

        public Type ObjectType { get; set; }

        public DBTable(Type objtype)
        {
            ObjectType = objtype;
            ObjectList = new Dictionary<int, object>();
        }

        public void Load(List<object> data)
        {
            lock (TableLock)
            {
                ObjectList.Clear();
                int simulateId = 0;
                foreach (object d in data)
                {
                    try
                    {
                        if (d.GetProperty(idname) == null)
                        {
                            simulateId++;
                            ObjectList.Add(simulateId, d);
                        }
                        else
                            ObjectList.Add(int.Parse(d.GetProperty(idname).ToString()), d);
                    }
                    catch
                    {
                        int idvalue = (int)d.GetProperty(idname);
                        throw new Exception("请检查id" + idvalue + "是否重复！");
                    }
                }
            }
        }

        public List<object> GetAll()
        {
            lock (TableLock)
            {
                List<object> result = ObjectList.Values.ToList();
                List<object> newt = CopyObj(result) as List<object>;
                return newt;
            }
        }

        public T Add<T>(T t) where T : class
        {
            lock (TableLock)
            {
                T newt = CopyObj(t) as T;
                int idvalue = (int)newt.GetProperty(idname);
                ObjectList.Add(idvalue, newt);
                return newt;
            }
        }

        public object Add(object obj)
        {
            lock (TableLock)
            {
                object newt = CopyObj(obj);
                int idvalue = (int)newt.GetProperty(idname);
                if (ObjectList.ContainsKey(idvalue))
                    return ObjectList[idvalue];
                ObjectList.Add(idvalue, newt);
                return newt;
            }
        }

        public int Update<T>(T t) where T : class
        {
            lock (TableLock)
            {
                int idvalue = (int)t.GetProperty(idname);
                if (!ObjectList.ContainsKey(idvalue))
                    return -1;
                var oldobj = ObjectList[idvalue];
                T newt = CopyObj(t) as T;
                ObjectList[idvalue] = newt;
                return 0;
            }
        }

        public int Update(object obj)
        {
            lock (TableLock)
            {
                int idvalue = (int)obj.GetProperty(idname);
                if (!ObjectList.ContainsKey(idvalue))
                    return -1;
                var oldobj = ObjectList[idvalue];
                object newt = CopyObj(obj);
                ObjectList[idvalue] = newt;
                return 0;
            }
        }

        public int Delete(int id)
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return -1;
                var obj = ObjectList[id];
                ObjectList.Remove(id);
                return 0;
            }
        }

        public int Delete<T>(T t) where T : class
        {
            int idvalue = (int)t.GetProperty(idname);
            return Delete(idvalue);
        }

        public T Get<T>(int id) where T : class
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return null;
                T newt = CopyObj(ObjectList[id]) as T;
                return newt;
            }
        }

        public object Get(int id)
        {
            lock (TableLock)
            {
                if (!ObjectList.ContainsKey(id))
                    return null;
                return CopyObj(ObjectList[id]);
            }
        }

        public List<T> Select<T>(Func<T, bool> predicate = null) where T : class
        {
            lock (TableLock)
            {
                List<T> result = null;
                if (predicate != null)
                    result = ObjectList.Values.Select(c => c as T).Where(predicate).ToList<T>();
                else
                    result = ObjectList.Values.Select(c => c as T).ToList<T>();
                List<T> newt = CopyObj(result) as List<T>;
                return newt;
            }
        }

        public int Count<T>(Func<T, bool> predicate = null) where T : class
        {
            lock (TableLock)
            {
                if (predicate == null)
                    return ObjectList.Values.Count();
                return ObjectList.Values.Select(c => c as T).Count(predicate);
            }
        }

        private object CopyObj(object obj)
        {
            //if (DBManager.Instance.SerializeTool == null)
            //    throw new Exception("SerializeTool is NULL,please Set for DBManager");
            //byte[] bs = DBManager.Instance.SerializeTool.Serialize(obj);
            //return DBManager.Instance.SerializeTool.Deserialize(bs, obj.GetType());
            return obj;
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
