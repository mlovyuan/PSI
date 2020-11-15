using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSI.Common
{
    public class DbConvert
    {
        /// <summary>
        /// 將DataRow轉換成Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private static T DataRowToModel<T>(DataRow dr, string columns)
        {
            T model = Activator.CreateInstance<T>();
            Type type = typeof(T);
            if (dr != null)
            {
                var properties = PropertyHelper.GetTypeProperties<T>(columns);
                foreach (var p in properties)
                {
                    string columnName = p.GetColumnName();
                    if (dr[columnName] is DBNull) // 若該行在DB的直為null，就也給null
                    {
                        p.SetValue(model, null);
                    }
                    else
                    {
                        SetPropertyValue<T>(model, dr[columnName], p);
                    }
                }
                return model;
            }
            return default(T);
        }

        /// <summary>
        /// 將DataTable轉換成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt, string columns)
        {
            List<T> list = new List<T>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T model = DataRowToModel<T>(dr, columns);
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// 將SqlDataReader轉換成Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static T SqlDataReaderToModel<T>(SqlDataReader reader, string columns)
        {
            T model = Activator.CreateInstance<T>();
            Type type = typeof(T);
            var properties = PropertyHelper.GetTypeProperties<T>(columns);
            if (reader.Read())
            {
                foreach (var p in properties)
                {
                    string columnName = p.GetColumnName();
                    if (reader[columnName] is DBNull)
                    {
                        p.SetValue(model, null);
                    }
                    else
                    {
                        SetPropertyValue<T>(model, reader[columnName], p);
                    }
                }
                return model;
            }
            return default(T);
        }

        /// <summary>
        /// 將SqlDataReader轉換成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<T> SqlDataReaderToList<T>(SqlDataReader reader, string columns)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            var properties = PropertyHelper.GetTypeProperties<T>(columns);
            while (reader.Read())
            {
                T model = Activator.CreateInstance<T>();
                foreach (var p in properties)
                {
                    string columnName = p.GetColumnName();
                    if (reader[columnName] is DBNull)
                    {
                        p.SetValue(model, null);
                    }
                    else
                    {
                        SetPropertyValue<T>(model, reader[columnName], p);
                    }
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 設定屬性的類型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        private static void SetPropertyValue<T>(T model, object obj, PropertyInfo property)
        {
            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                property.SetValue(model, Convert.ChangeType(obj, Nullable.GetUnderlyingType(property.PropertyType)));
            }
            else
            {
                property.SetValue(model, Convert.ChangeType(obj, property.PropertyType));
            }
        }
    }
}
