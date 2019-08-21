using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Entities
{
    public class DataSource
    {
        /// <summary>
        /// 获取或设置数据库提供者
        /// </summary>
        public Func<IDbConnection> ConnectionFacotry { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatasourceType DatasourceType { get; set; }
        /// <summary>
        /// 提供名称
        /// </summary>
        public string DatasourceName { get; set; }
        /// <summary>
        /// 是否使用数据库上下文代理（）
        /// </summary>
        public bool UseProxy { get; set; }
        /// <summary>
        /// 默认不填是哪个数据库（DbContextProxy）可以查看日志
        /// </summary>
        public bool Default { get; set; }
    }
}
