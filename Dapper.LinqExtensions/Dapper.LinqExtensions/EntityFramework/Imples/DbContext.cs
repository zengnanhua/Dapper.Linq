using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Dapper.LinqExtensions.EntityFramework.Entities;
using static Dapper.SqlMapper;

namespace Dapper.LinqExtensions.EntityFramework.Imples
{
    public class DbContext : IDbContext
    {
        /// <summary>
        /// 数据源类型
        /// </summary>
        public DatasourceType SourceType { get; set; }
        public List<Logger> Loggers { get; set; }
        /// <summary>
        /// 数据库事物
        /// </summary>
        public IDbTransaction Transaction { get; private set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection Connection { get; }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool? Buffered { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int? Timeout { get; set; }
        /// <summary>
        /// 数据上下文状态
        /// </summary>
        public DbContextState State { get; private set; }
        public DbContext(IDbConnection connection, DatasourceType sourceType)
        {
            Connection = connection;
            SourceType = sourceType;
            State = DbContextState.Closed;
        }
        /// <summary>
        /// 打开数据库上下文连接
        /// </summary>
        /// <param name="beginTransaction"></param>
        /// <param name="level"></param>
        public void Open(bool beginTransaction, IsolationLevel? level = null)
        {
            if (!beginTransaction)
            {
                Connection.Open();
            }
            else
            {
                Connection.Open();
                Transaction = level == null ? Connection.BeginTransaction() : Connection.BeginTransaction(level.Value);
            }
            State = DbContextState.Open;
        }
        public async Task OpenAsync(bool beginTransaction, IsolationLevel? level = null)
        {
            State = DbContextState.Open;
            if (!(Connection is DbConnection))
            {
                throw new InvalidOperationException("Async operations require use of a DbConnection or an already-open IDbConnection");
            }
            if (!beginTransaction)
            {
                await (Connection as DbConnection).OpenAsync();
            }
            else
            {
                await (Connection as DbConnection).OpenAsync();
                Transaction = level == null ? Connection.BeginTransaction() : Connection.BeginTransaction(level.Value);
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            Connection.Close();
            State = DbContextState.Closed;
        }
        /// <summary>
        /// 提交数据库事物
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
                Transaction.Dispose();
                State = DbContextState.Commit;
            }
        }
        /// <summary>
        /// 回滚数据
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                State = DbContextState.Rollback;
            }
        }
        /// <summary>
        /// 关闭事物
        /// </summary>
        public void Dispose()
        {
            Close();
        }
        #region 原Dapper操作
        public GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultiple(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.QueryMultipleAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.Execute(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalar<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, text);
        }
        #endregion

        #region 原dapper 查询方法
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T>(sql, param, Transaction, Buffered != null ? Buffered.Value : buffered, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryAsync<T>(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query(sql, param, Transaction, Buffered != null ? Buffered.Value : buffered, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryAsync(sql, param, Transaction, Timeout != null ? Timeout.Value : commandTimeout, commandType);
        }
        #endregion

        public IQueryable<T> From<T>() where T : class
        {
            if (SourceType == DatasourceType.SQLSERVER)
            {
                //return new MySqlQuery<T>(this);
            }
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");
            //else if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new SqlQuery<T>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T>(this);
            //}

        }
        public IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class
        {
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");

            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T1, T2>(this);
            //}
            //else if (SourceType == DatasourceType.SQLSERVER)
            //{
            //    return new SqlQuery<T1, T2>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T1, T2>(this);
            //}
            //throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3> From<T1, T2, T3>() where T1 : class where T2 : class where T3 : class
        {
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");

            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T1, T2, T3>(this);
            //}
            //else if (SourceType == DatasourceType.SQLSERVER)
            //{
            //    return new SqlQuery<T1, T2, T3>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T1, T2, T3>(this);
            //}
            //throw new NotImplementedException();
        }
        public IQueryable<T1, T2, T3, T4> From<T1, T2, T3, T4>() where T1 : class where T2 : class where T3 : class where T4 : class
        {
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");
            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T1, T2, T3, T4>(this);
            //}
            //else if (SourceType == DatasourceType.SQLSERVER)
            //{
            //    return new SqlQuery<T1, T2, T3, T4>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T1, T2, T3, T4>(this);
            //}
            //throw new NotImplementedException();
        }
    }
}
