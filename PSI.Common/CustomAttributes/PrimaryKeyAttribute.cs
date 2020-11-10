using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI.Common.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute:Attribute
    {
        public string Name { get; protected set; }
        public bool AutoIncrement = false;
        public PrimaryKeyAttribute(string primaryKey)
        {
            Name = primaryKey;
        }
    }
}
