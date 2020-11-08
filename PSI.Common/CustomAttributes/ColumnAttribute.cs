using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI.Common.CustomAttributes
{
    // 指定用在某個類的屬性
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        public string ColName { get; protected set; }
        public ColumnAttribute(string colName)
        {
            ColName = colName;
        }
    }
}
