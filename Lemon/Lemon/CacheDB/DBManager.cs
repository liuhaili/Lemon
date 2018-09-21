using Lemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Lemon.CacheDB
{
    /// <summary>
    /// 内存数据库管理者
    /// 3.调用SetSerialization设置序列化工具，可以不设，默认json序列化
    /// </summary>
    public class DBManager : SingletonBase<DBManager>
    {
        private const string defaultDBName = "defaultdb";
        private readonly object dblistLock = new object();
        private Dictionary<string, DB> dbs = new Dictionary<string, DB>();

        private readonly object ObjectIndexLock = new object();
        private Dictionary<string, List<object>> ObjectIndexList = new Dictionary<string, List<object>>();

        public void LoadData(Type type, List<object> data)
        {
            DBTable table = GetTable(type);
            if (table == null)
                return;
            table.Load(data);
        }

        public DBTable GetTable(Type type, string dbname = defaultDBName)
        {
            lock (dblistLock)
            {
                if (!dbs.ContainsKey(dbname))
                    return null;
                DBTable table = dbs[dbname].GetTable(type);
                return table;
            }
        }

        private List<T> SelectByIndex<T>(string indexName, string dbname = defaultDBName, params Func<T, object>[] attrFunc) where T : class
        {
            lock (ObjectIndexLock)
            {
                Type type = typeof(T);
                DBTable table = GetTable(type, dbname);
                if (table == null)
                    return new List<T>();
                if (ObjectIndexList.Count == 0)
                {
                    List<T> allobj = table.Select<T>();
                    List<KeyValuePair<string, T>> newList = new List<KeyValuePair<string, T>>();
                    for (int i = 0; i < allobj.Count; i++)
                    {
                        T obj = allobj[i];
                        string oldkey = "";
                        for (int f = 0; f < attrFunc.Length; f++)
                        {
                            oldkey += attrFunc[f](obj).ToString() + "_";
                        }
                        newList.Add(new KeyValuePair<string, T>(oldkey, obj));

                    }
                    foreach (var g in newList.GroupBy(c => c.Key))
                    {
                        ObjectIndexList.Add(g.Key, g.Select(c => c.Value as object).ToList());
                    }
                }
                if (!ObjectIndexList.ContainsKey(indexName))
                    return new List<T>();
                return ObjectIndexList[indexName].Select(c => c as T).ToList();
            }
        }
    }
}
