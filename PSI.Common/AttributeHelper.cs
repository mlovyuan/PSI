using PSI.Common.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static string GetTName(this Type type)
        {
            string tableName = string.Empty;
            object[] attrs = type.GetCustomAttributes(false);
            foreach(var attr in attrs)
            {
                if(attr is TableAttribute)
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
    }
}
