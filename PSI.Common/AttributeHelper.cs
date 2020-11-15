using PSI.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSI.Common
{
    public static class AttributeHelper
    {
        /// <summary>
        /// 取得映射的表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            string tableName = string.Empty;
            object[] attrs = type.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr is TableAttribute)
                {
                    TableAttribute tableAttribute = attr as TableAttribute;
                    tableName = tableAttribute.Name;
                }
            }
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = type.Name;
            }
            return tableName;
        }

        /// <summary>
        /// 取得映射的欄位名
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetColumnName(this PropertyInfo property)
        {
            string columnName = string.Empty;
            object[] attrs = property.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr is ColumnAttribute)
                {
                    ColumnAttribute columnAttribute = attr as ColumnAttribute;
                    columnName = columnAttribute.ColName;
                }
            }
            if (string.IsNullOrEmpty(columnName))
            {
                columnName = property.Name;
            }
            return columnName;
        }

        /// <summary>
        /// 判斷PK是否會自動增加
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIncrement(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                PrimaryKeyAttribute primaryKeyAttribute = attribute as PrimaryKeyAttribute;
                return primaryKeyAttribute.AutoIncrement;
            }
            return false;
        }

        /// <summary>
        /// 取得PK名稱
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetPrimary(this Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute is PrimaryKeyAttribute)
                {
                    PrimaryKeyAttribute primaryKeyAttribute = attribute as PrimaryKeyAttribute;
                    return primaryKeyAttribute.Name;
                }
            }
            return null;
        }

        /// <summary>
        /// 判斷是否為PK
        /// </summary>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPrimary(this Type type, PropertyInfo property)
        {
            string primaryKeyName = type.GetPrimary();
            string columnName = property.GetColumnName();
            return primaryKeyName == columnName;
        }
    }
}
