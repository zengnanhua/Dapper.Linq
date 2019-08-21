using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Entities
{
    public class Logger
    {
        public string Text { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecuteTime { get; set; }
        public int? Timeout { get; set; }
        public bool? Buffered { get; set; }
    }
}
