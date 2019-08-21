using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Entities
{
    public class EntityTable
    {
        /// <summary>
        /// C#列名属性
        /// </summary>
        public Type CSharpType { get; set; }
        /// <summary>
        /// 数据库的表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// C# 类名称
        /// </summary>
        public string CSharpName { get; set; }
        public List<EntityColumn> Columns { get; set; }
    }
}
