using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lemon.CacheDB
{
    /// <summary>
    /// 一个数据库
    /// </summary>
    public class DB
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Name { get; set; }
        protected Dictionary<Type, DBTable> Tables { get; set; }
        private readonly object tablesLock = new object();

        public DB(string name)
        {
            this.Name = name;
            Tables = new Dictionary<Type, DBTable>();
        }

        public void ClearAll()
        {
            Tables.Clear();
        }

        public DBTable GetTable(Type type)
        {
            if (!Tables.ContainsKey(type))
                return null;
            return Tables[type];
        }

        public IEnumerable<Type> GetAllType()
        {
            return Tables.Keys;
        }

        public bool HasTable(Type type)
        {
            return Tables.ContainsKey(type);
        }

        public void RemoveTable(Type type)
        {
            if (!Tables.ContainsKey(type))
                return;
            Tables.Remove(type);
        }

        public void AddTable<T>(List<T> datas) where T : IData
        {
            Type type = typeof(T);
            if (Tables.ContainsKey(type))
            {
                Tables.Remove(type);
            }
            var dbtable = new DBTable();
            dbtable.Load<T>(datas);
            Tables.Add(type, dbtable);
        }

        public void AddTable(Type type,List<object> datas)
        {
            if (Tables.ContainsKey(type))
            {
                Tables.Remove(type);
            }
            var dbtable = new DBTable();
            dbtable.Load(datas);
            Tables.Add(type, dbtable);
        }
    }
}
