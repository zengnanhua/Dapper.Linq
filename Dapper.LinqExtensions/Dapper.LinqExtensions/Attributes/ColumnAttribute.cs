using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.Attributes
{
    /// <summary>
    /// 实体字段对应的表字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        /// <summary>
        /// 数据库的字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库的字段是否 索引或主键
        /// </summary>
        public ColumnKey Key { get; set; }
        public bool IsColumn { get; set; }
        public bool IsIdentity { get; set; }
        public ColumnAttribute(string name = null, ColumnKey key = ColumnKey.None, bool isIdentity = false, bool isColumn = true)
        {
            Name = name;
            Key = key;
            IsColumn = isColumn;
            IsIdentity = isIdentity;
        }
    }
}
