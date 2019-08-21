using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.Attributes
{
    /// <summary>
    /// 数据库表和实体类不一样
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {
        public string Name { get; set; }
        public TableAttribute(string Name=null)
        {
            this.Name = Name;
        }
    }
}
