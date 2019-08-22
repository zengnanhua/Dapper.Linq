using Dapper.LinqExtensions.EntityFramework.DataBaseImples;
using Dapper.LinqExtensions.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper.LinqExtensions.EntityFramework.Imples
{
    /// <summary>
    /// 数据库上下文代理类
    /// </summary>
    public class DbContextProxy: IDbContext
    {
        public IDbContext _target = null;
        public List<Logger> Loggers { get; set; }

        public IDbConnection Connection => _target.Connection;

        public IDbTransaction Transaction => _target.Transaction;

        public DbContextState State => _target.State;

        public DatasourceType SourceType => _target.SourceType;

        public bool? Buffered { get => _target.Buffered; set => _target.Buffered = value; }
        public int? Timeout { get => _target.Timeout; set => _target.Timeout = value; }

        public DbContextProxy(IDbContext target)
        {
            _target = target;
            Loggers = new List<Logger>();
        }
        public void Close()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Close();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = nameof(DbContextProxy.Close),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Commit()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Commit();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = nameof(Commit),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Dispose()
        {
            _target.Dispose();
        }
        public int Execute(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Execute(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteAsync(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteScalar<T>(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.ExecuteScalarAsync<T>(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IQueryable<T> From<T>() where T : class
        {
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlServerProvider<T>(this);
            }
            else if (SourceType == DatasourceType.ORACLE)
            {
                return new OracleProvider<T>(this);
            }
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");
            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T>(this);
            //}
            //else if (SourceType == DatasourceType.SQLSERVER)
            //{
            //    return new SqlQuery<T>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T>(this);
            //}
            //throw new NotImplementedException();
        }
        public IQueryable<T1, T2> From<T1, T2>() where T1 : class where T2 : class
        {
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlServerProvider<T1,T2>();
            }
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
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlServerProvider<T1, T2, T3>();
            }
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");

            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T1, T2, T3>(this);
            //}
            //if (SourceType == DatasourceType.SQLSERVER)
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
            if (SourceType == DatasourceType.SQLSERVER)
            {
                return new SqlServerProvider<T1, T2, T3,T4>();
            }
            throw new NotImplementedException($@"暂未提供此数据库{SourceType.ToString()}类型的实现类");

            //if (SourceType == DatasourceType.MYSQL)
            //{
            //    return new MySqlQuery<T1, T2, T3, T4>(this);
            //}
            //if (SourceType == DatasourceType.SQLSERVER)
            //{
            //    return new SqlQuery<T1, T2, T3, T4>(this);
            //}
            //else if (SourceType == DatasourceType.SQLITE)
            //{
            //    return new SQLiteQuery<T1, T2, T3, T4>(this);
            //}
            //throw new NotImplementedException();
        }
        public void Open(bool beginTransaction, IsolationLevel? level = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Open(beginTransaction, level);
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = string.Format("{0}:Transaction:{1}", nameof(Open), beginTransaction ? "ON" : "OFF"),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public async Task OpenAsync(bool beginTransaction, IsolationLevel? level = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                await _target.OpenAsync(beginTransaction, level);
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    Text = string.Format("{0}:Transaction:{1}", nameof(Open), beginTransaction ? "ON" : "OFF"),
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IEnumerable<T> Query<T>(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Query<T>(sql, param, buffered, commandTimeout, commandType);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryAsync<T>(sql, param, commandTimeout, commandType);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public IEnumerable<dynamic> Query(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.Query(sql, param, buffered, commandTimeout, commandType);

            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryAsync(sql, param, commandTimeout, commandType);

            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public GridReader QueryMultiple(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryMultiple(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public Task<GridReader> QueryMultipleAsync(string sql, object param = null, int? commandTimeout = null, CommandType text = CommandType.Text)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                return _target.QueryMultipleAsync(sql, param, commandTimeout, text);
            }
            finally
            {
                watch.Stop();
                Loggers.Add(new Logger()
                {
                    Value = param,
                    Text = sql,
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
        public void Rollback()
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                _target.Rollback();
                watch.Stop();
            }
            finally
            {
                Loggers.Add(new Logger()
                {
                    Value = null,
                    Text = nameof(Rollback),
                    Buffered = Buffered,
                    Timeout = Timeout,
                    ExecuteTime = watch.ElapsedMilliseconds
                });
            }
        }
    }
}
