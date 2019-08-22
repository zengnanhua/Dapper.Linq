﻿using Dapper.LinqExtensions.Attributes;
using Dapper.LinqExtensions.EntityFramework.Entities;
using Dapper.LinqExtensions.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.LinqExtensions.EntityFramework.DataBaseImples
{
    public class OracleProvider<T> : IQueryable<T> where T : class
    {
        #region property
        public Dictionary<string, object> _param { get; set; }
        public Dictionary<string, string> _columns = new Dictionary<string, string>();
        public StringBuilder _columnBuffer = new StringBuilder();
        public List<string> _filters = new List<string>();
        public StringBuilder _setBuffer = new StringBuilder();
        public StringBuilder _havingBuffer = new StringBuilder();
        public StringBuilder _whereBuffer = new StringBuilder();
        public StringBuilder _groupBuffer = new StringBuilder();
        public StringBuilder _orderBuffer = new StringBuilder();
        public StringBuilder _distinctBuffer = new StringBuilder();
        public StringBuilder _countBuffer = new StringBuilder();
        public StringBuilder _sumBuffer = new StringBuilder();
        public StringBuilder _lock = new StringBuilder();
        public EntityTable _table = EntityUtil.GetTable<T>();
        public int? pageIndex = null;
        public int? pageCount = null;
        #endregion

        #region 构造函数
        public IDbContext _dbcontext { get; }
        /// <summary>
        /// 数据库变量的前缀
        /// </summary>
        public string _prefix { get; }
        public OracleProvider(IDbContext dbcontext = null)
        {
            if (dbcontext == null)
            {
                throw new Exception("dbContext为空");
            }
            _dbcontext = dbcontext;
            _prefix = ":";
            _param = new Dictionary<string, object>();
        }
        public OracleProvider(Dictionary<string, object> param)
        {
            _param = param;
            _prefix = ":";
        }
        #endregion

        #region 实现方法
        public IQueryable<T> With(string lockType, bool condition = true)
        {
            if (condition)
            {
                _lock.Append(lockType);
            }
            return this;
        }
        /// <summary>
        /// 加上数据库锁
        /// </summary>
        /// <param name="lockType"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> With(LockType lockType, bool condition = true)
        {
            if (condition)
            {
                var temp = string.Empty;
                if (lockType == LockType.UPDLOCK)
                {
                    With("UPDLOCK");
                }
                else if (lockType == LockType.NOLOCK)
                {
                    With("NOLOCK");
                }
            }
            return this;
        }

        /// <summary>
        /// 去掉重复
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> Distinct(bool condition = true)
        {
            if (condition)
            {
                _distinctBuffer.Append("DISTINCT");
            }
            return this;
        }
        /// <summary>
        /// 根据过滤条件匹配添加
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> Filter<TResult>(Expression<Func<T, TResult>> columns, bool condition = true)
        {
            if (condition)
            {
                _filters.AddRange(ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => s.Value));
            }
            return this;
        }
        public IQueryable<T> GroupBy(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_groupBuffer.Length > 0)
                {
                    _groupBuffer.Append(",");
                }
                _groupBuffer.Append(expression);
            }
            return this;
        }

        public IQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));
            }
            return this;
        }

        public IQueryable<T> Having(string expression, bool condition = true)
        {
            if (condition)
            {
                _havingBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> Having(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));
            }
            return this;
        }
        public IQueryable<T> OrderBy(string orderBy, bool condition = true)
        {
            if (condition)
            {
                if (_orderBuffer.Length > 0)
                {
                    _orderBuffer.Append(",");
                }
                _orderBuffer.Append(orderBy);
            }
            return this;
        }
        public IQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} ASC", s.Value))));
            }
            return this;
        }
        public IQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool condition = true)
        {
            if (condition)
            {
                OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} DESC", s.Value))));
            }
            return this;
        }
        public IQueryable<T> Page(int index, int count, out long total, bool condition = true)
        {
            total = 0;
            if (condition)
            {
                Skip(count * (index - 1), count);
                total = Count();
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, ISubQuery subquery, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
                _setBuffer.AppendFormat("{0} = {1}", columns.Value, subquery.Build(_param, _prefix));
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
                var key = string.Format("{0}{1}", columns.Key, _param.Count);
                _param.Add(key, value);
                _setBuffer.AppendFormat("{0} = @{1}", columns.Value, key);
            }
            return this;
        }
        public IQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> value, bool condition = true)
        {
            if (condition)
            {
                if (_setBuffer.Length > 0)
                {
                    _setBuffer.Append(",");
                }
                var columnName = ExpressionUtil.BuildColumn(column, _param, _prefix).First().Value;
                var expression = ExpressionUtil.BuildExpression(value, _param, _prefix);
                _setBuffer.AppendFormat("{0} = {1}", columnName, expression);
            }
            return this;
        }
        public IQueryable<T> Skip(int index, int count, bool condition = true)
        {
            if (condition)
            {
                pageIndex = index;
                pageCount = count;
            }
            return this;
        }
        public IQueryable<T> Take(int count, bool condition = true)
        {
            if (condition)
            {
                Skip(0, count);
            }
            return this;
        }
        public IQueryable<T> Where(string expression, bool condition = true)
        {
            if (condition)
            {
                if (_whereBuffer.Length > 0)
                {
                    _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
                }
                _whereBuffer.Append(expression);
            }
            return this;
        }
        public IQueryable<T> Where(Expression<Func<T, bool?>> expression, bool condition = true)
        {
            if (condition)
            {
                Where(ExpressionUtil.BuildExpression(expression, _param, _prefix));
            }
            return this;
        }
        public int Delete(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildDelete();
                return _dbcontext.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public async Task<int> DeleteAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildDelete();
                return await _dbcontext.ExecuteAsync(sql, _param, timeout);
            }
            return 0;
        }
        public int Insert(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return _dbcontext.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return await _dbcontext.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public long InsertReturnId(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return _dbcontext.ExecuteScalar<long>(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<long> InsertReturnIdAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                return await _dbcontext.ExecuteScalarAsync<long>(sql, entity, timeout);
            }
            return 0;
        }
        public int Insert(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return _dbcontext.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> InsertAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildInsert();
                return await _dbcontext.ExecuteAsync(sql, entitys, timeout);
            }
            return 0;
        }
        public int Update(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return _dbcontext.Execute(sql, _param, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null && _setBuffer.Length > 0)
            {
                var sql = BuildUpdate(false);
                return await _dbcontext.ExecuteAsync(sql, _param, timeout);
            }
            return 0;
        }
        public int Update(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return _dbcontext.Execute(sql, entity, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(T entity, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return await _dbcontext.ExecuteAsync(sql, entity, timeout);
            }
            return 0;
        }
        public int Update(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return _dbcontext.Execute(sql, entitys, timeout);
            }
            return 0;
        }
        public async Task<int> UpdateAsync(IEnumerable<T> entitys, bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildUpdate();
                return await _dbcontext.ExecuteAsync(sql, entitys, timeout);
            }
            return 0;
        }
        public T Single(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<T> SingleAsync(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        public TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns(columns, _param, _prefix);
            return Single<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key))),
                buffered,
                timeout);
        }
        public Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns(columns, _param, _prefix);
            return SingleAsync<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key))),
                timeout);
        }
        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<T>(sql, _param, buffered, timeout);
            }
            return new List<T>();
        }
        public async Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<T>(sql, _param, timeout);
            }
            return new List<T>();
        }
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return _dbcontext.Query<TResult>(sql, _param, buffered, timeout);
            }
            return new List<TResult>();
        }
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            if (_dbcontext != null)
            {
                var sql = BuildSelect();
                return await _dbcontext.QueryAsync<TResult>(sql, _param, timeout);
            }
            return new List<TResult>();
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns(columns, _param, _prefix);
            return Select<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key)))
                , buffered, timeout);
        }
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            _columns = ExpressionUtil.BuildColumns(columns, _param, _prefix);
            return SelectAsync<TResult>(string.Join(",",
                _columns.Select(s => string.Format("{0} AS {1}", s.Value, s.Key)))
                , timeout);
        }
        public long Count(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return _dbcontext.ExecuteScalar<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public async Task<long> CountAsync(string columns = null, bool codition = true, int? timeout = null)
        {
            if (codition)
            {
                if (columns != null)
                {
                    _columnBuffer.Append(columns);
                }
                if (_dbcontext != null)
                {
                    var sql = BuildCount();
                    return await _dbcontext.ExecuteScalarAsync<long>(sql, _param, timeout);
                }
            }
            return 0;
        }
        public long Count<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return Count(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), condition, timeout);
        }
        public Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            return CountAsync(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), condition, timeout);
        }
        public bool Exists(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildExists();
                return _dbcontext.ExecuteScalar<int>(sql, _param, timeout) > 0;
            }
            return false;
        }
        public async Task<bool> ExistsAsync(bool condition = true, int? timeout = null)
        {
            if (condition && _dbcontext != null)
            {
                var sql = BuildExists();
                return await _dbcontext.ExecuteScalarAsync<int>(sql, _param, timeout) > 0;
            }
            return false;
        }
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
                _sumBuffer.AppendFormat("{0}", column);
                if (_dbcontext != null)
                {
                    var sql = BuildSum();
                    return _dbcontext.ExecuteScalar<TResult>(sql, _param, timeout);
                }
            }
            return default;
        }
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, bool condition = true, int? timeout = null)
        {
            if (condition)
            {
                var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
                _sumBuffer.AppendFormat("{0}", column);
                if (_dbcontext != null)
                {
                    var sql = BuildSum();
                    return await _dbcontext.ExecuteScalarAsync<TResult>(sql, _param, timeout);
                }
            }
            return default;
        }
        #endregion

        #region 编译成sql
        public string BuildInsert()
        {
            var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                _table.TableName,
                string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => s.ColumnName))
                , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("@{0}", s.CSharpName))));
            return sql;
        }
        public string BuildUpdate(bool allColumn = true)
        {
            if (allColumn)
            {
                var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
                var colums = _table.Columns.FindAll(f => f.ColumnKey != ColumnKey.Primary && !_filters.Exists(e => e == f.ColumnName));
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    _table.TableName,
                    string.Join(",", colums.Select(s => string.Format("{0} = @{1}", s.ColumnName, s.CSharpName))),
                     _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
                return sql;
            }
            else
            {
                var sql = string.Format("UPDATE {0} SET {1}{2}",
                    _table.TableName,
                    _setBuffer,
                    _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
                return sql;
            }

        }
        public string BuildDelete()
        {
            var sql = string.Format("DELETE FROM {0}{1}",
                _table.TableName,
                _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
            return sql;
        }
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder();
            if (pageIndex == 0 && pageCount > 0)
            {
                sqlBuffer.AppendFormat("SELECT TOP {0}", pageCount);
            }
            else
            {
                sqlBuffer.AppendFormat("SELECT");
            }
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            if (_columnBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _columnBuffer.Replace("+","||"));
            }
            else
            {
                sqlBuffer.AppendFormat(" {0}", string.Join(",", _table.Columns.FindAll(f => !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("{0} AS {1}", s.ColumnName, s.CSharpName))));
            }
            if (pageIndex > 0)
            {
                sqlBuffer.AppendFormat(",ROW_NUMBER() OVER (ORDER BY {0}) AS RowNum",
                    _orderBuffer.Length > 0 ? _orderBuffer.ToString() :
                    _groupBuffer.Length > 0 ? _groupBuffer.ToString() :
                    _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary).ColumnName);
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_lock.Length > 0)
            {
                sqlBuffer.AppendFormat(" WITH({0})", _lock);
            }
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0 && (pageIndex == null || pageIndex == 0))
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex > 0)
            {
                return string.Format("SELECT TOP {0} {1} FROM ({2}) AS T WHERE RowNum > {3}", pageCount,
                    _columns.Count > 0 ? string.Join(",", _columns.Keys) : "*",
                    sqlBuffer,
                    pageIndex);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {
                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        public string BuildExists()
        {
            var sqlBuffer = new StringBuilder();

            sqlBuffer.AppendFormat("SELECT 1 FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            var sql = string.Format("SELECT 1 WHERE EXISTS({0})", sqlBuffer);
            return sql;
        }
        public string BuildSum()
        {
            var sqlBuffer = new StringBuilder();
            sqlBuffer.AppendFormat("SELECT SUM({0}) FROM {1}", _sumBuffer, _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            return sqlBuffer.ToString();
        }
        #endregion
    }
}
