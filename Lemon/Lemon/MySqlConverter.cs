using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lemon.Extensions;

namespace Lemon
{
    public class NotDataField : Attribute
    {
    }
    /// <summary>
    /// 数据库表名字必须和类名相同，数据库字段名、数目必须和类的字段名、数目相同
    /// </summary>
    public static class MySqlConverter
    {
        private static Dictionary<Type, PropertyInfo[]> typeInfoCache = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 生成INSERT语句
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        public static string ToInsertSQL(object tObj, bool ignoreID = false, string tableSuffix = "")
        {
            string tableName = GetTableName(tObj.GetType()) + tableSuffix;
            Dictionary<string, string> fieldsAndValues = getFieldsAndValues(tObj);
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();
            foreach (KeyValuePair<string, string> fv in fieldsAndValues)
            {
                if (!ignoreID && fv.Key.ToLower() == "id")
                    continue;
                fields.Append(fv.Key).Append(",");
                values.Append(fv.Value).Append(",");
            }
            StringBuilder insertSql = new StringBuilder();
            insertSql.Append("INSERT INTO ").Append(tableName).Append("(").Append(fields.ToString().TrimEnd(',')).Append(") VALUES(").Append(values.ToString().TrimEnd(',')).Append(")");
            return insertSql.ToString();
        }

        /// <summary>
        /// 生成UPDATE语句
        /// </summary>
        /// <param name="tObj"></param>
        /// <returns></returns>
        public static string ToUpdateSQL(object tObj, string wherestr = null, string tableSuffix = "")
        {
            string tableName = GetTableName(tObj.GetType()) + tableSuffix;
            Dictionary<string, string> fieldsAndValues = getFieldsAndValues(tObj);
            //采用stringbuilder可以提高效率
            StringBuilder updateSQL = new StringBuilder();
            updateSQL.Append("UPDATE ").Append(tableName).Append(" SET ");
            foreach (KeyValuePair<string, string> fv in fieldsAndValues)
            {
                updateSQL.Append(fv.Key).Append("=").Append(fv.Value).Append(",");
            }
            updateSQL.Remove(updateSQL.Length - 1, 1);  //移除最后一个 ”，“
            if (!String.IsNullOrEmpty(wherestr))
                updateSQL.Append(" WHERE ").Append(wherestr);
            else
                updateSQL.Append(" WHERE ID=").Append(tObj.GetProperty("ID"));
            return updateSQL.ToString();
        }

        /// <summary>
        /// 生成DELETE语句
        /// </summary>
        /// <param name="tObj"></param>
        /// <param name="wherenames"></param>
        /// <returns></returns>
        public static string ToDeleteSQL(Type t, int key, string wherestr = null, string tableSuffix = "")
        {
            string tableName = GetTableName(t) + tableSuffix;
            StringBuilder deleteSQL = new StringBuilder();
            deleteSQL.Append("DELETE FROM ").Append(tableName);
            if (!String.IsNullOrEmpty(wherestr))
                deleteSQL.Append(" WHERE ").Append(wherestr);
            else
                deleteSQL.Append(" WHERE ID=").Append(key);
            return deleteSQL.ToString();
        }

        /// <summary>
        /// 查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wherestr"></param>
        /// <returns></returns>
        public static string ToSelectSQL(Type t, string wherestr = null, string tableSuffix = "")
        {
            string tableName = GetTableName(t) + tableSuffix;
            StringBuilder selectSQL = new StringBuilder();
            selectSQL.Append("SELECT * FROM ").Append(tableName);
            if (!String.IsNullOrEmpty(wherestr))
                selectSQL.Append(" WHERE ").Append(wherestr);
            return selectSQL.ToString();
        }

        /// <summary>
        /// 生成字段 和 values的列表，用Dictionary形式存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tObj"></param>
        /// <returns></returns>
        private static Dictionary<string, string> getFieldsAndValues(object tObj)
        {
            Dictionary<string, string> fieldsAndValues = new Dictionary<string, string>();
            Type t = tObj.GetType();
            PropertyInfo[] properties = null;
            if (typeInfoCache.ContainsKey(t))
                properties = typeInfoCache[t];
            else
            {
                properties = t.GetRuntimeProperties().ToArray();
                typeInfoCache.Add(t, properties);
            }

            foreach (PropertyInfo propertyInfo in properties)
            {
                Attribute notData = propertyInfo.GetCustomAttribute(typeof(NotDataField));
                if (notData != null)
                    continue;
                //如果该实体对象某个property是空值的话，直接跳过该property的处理  
                object val = propertyInfo.GetValue(tObj, null);
                if (val == null)
                    continue;
                string propertyValue = string.Empty;
                Type propertyType = propertyInfo.PropertyType;
                TypeInfo propertyTypeInfo = propertyType.GetTypeInfo();
                //对于字符和datetime，要加上引号
                if (propertyType == typeof(System.Boolean) || propertyType == typeof(System.String) || propertyType == typeof(System.DateTime) || propertyType == typeof(System.DateTime?) || propertyTypeInfo.IsEnum)
                {
                    if (propertyType == typeof(System.DateTime))
                        propertyValue = "'" + ((DateTime)val).ToString("yyyy-MM-dd hh:mm:ss") + "'";
                    else if (propertyType == typeof(System.DateTime?))
                        propertyValue = "'" + ((DateTime?)val).Value.ToString("yyyy-MM-dd hh:mm:ss") + "'";
                    else if (propertyTypeInfo.IsEnum)
                        propertyValue = ((int)val).ToString();
                    else if (propertyType == typeof(System.Boolean))
                        propertyValue = val.ToString();
                    else
                        propertyValue = "'" + val.ToString().Replace("'", "\\'") + "'";
                }
                else
                {
                    propertyValue = val.ToString();
                }
                fieldsAndValues.Add("`" + propertyInfo.Name + "`", propertyValue);
            }
            return fieldsAndValues;
        }

        /// <summary>
        /// 获取条件语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tObj"></param>
        /// <param name="fieldsAndValues"></param>
        /// <param name="wherenames"></param>
        /// <returns></returns>
        private static string getWhereSQL(object tObj, Dictionary<string, string> fieldsAndValues, List<string> wherenames)
        {
            StringBuilder whereSQL = new StringBuilder();
            whereSQL.Append(" WHERE ");
            if (wherenames == null)
            {
                string tObjID = tObj.GetProperty("ID").ToString();
                whereSQL.Append("ID= ").Append(tObjID);
            }
            else
            {
                int i = 1;
                foreach (KeyValuePair<string, string> fv in fieldsAndValues)
                {
                    if (wherenames.Contains(fv.Key))
                        whereSQL.Append(fv.Key).Append(" = ").Append(fv.Value);
                    if (i < fieldsAndValues.Count)
                        whereSQL.Append(" AND ");
                    i++;
                }
            }
            return whereSQL.ToString();
        }

        public static string GetTableName(Type type)
        {
            return type.Name.ToLower().TrimStart('e');
        }
    }
}