using Dapper.LinqExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Entities
{
    public class EntityColumn
    {
        /// <summary>
        /// 数据的字段名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// C# 属性名称
        /// </summary>
        public string CSharpName { get; set; }
        /// <summary>
        /// 是否自增长类
        /// </summary>
        public bool Identity { get; set; }
        /// <summary>
        /// 键名称
        /// </summary>
        public ColumnKey ColumnKey { get; set; }
    }
}
