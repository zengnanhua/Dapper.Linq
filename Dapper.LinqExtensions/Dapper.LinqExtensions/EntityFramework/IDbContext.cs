using Dapper.LinqExtensions.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper.LinqExtensions.EntityFramework
{
    public interface IDbContext:IDisposable
    {
        /// <summary>
        /// 是否缓存
        /// </summary>
        bool? Buffered { get; set; }
       /// <summary>
       /// 超时时间
       /// </summary>
        int? Timeout { get; set; }
        /// <summary>
        /// 数据库上下文执行Logger
        /// </summary>
        List<Logger> Loggers { get; }

        IQueryable<T> From<T>() where T : class;
        IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class;
        IQueryable<T1, T2, T3> From<T1, T2, T3>() where T1 : class where T2 : class where T3 : class;
        GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text);
        IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        DatasourceType SourceType { get; }
        DbContextState State { get; }
        void Open(bool beginTransaction, IsolationLevel? level = null);
        Task OpenAsync(bool beginTransaction, IsolationLevel? level = null);
        void Commit();
        void Rollback();
        void Close();
    }
}
