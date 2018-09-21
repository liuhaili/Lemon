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
    internal class DB
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

        public void Clear(Type type)
        {
            if (!Tables.ContainsKey(type))
                return;
            Tables[type].Clear();
        }

        /// <summary>
        /// 获取区域下某个类型的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DBTable GetTable(Type type)
        {
            lock (tablesLock)
            {
                return Tables[type] as DBTable;
            }
        }

        public List<Type> GetAllType()
        {
            lock (tablesLock)
            {
                return new List<Type>(Tables.Keys);
            }
        }
    }
}
