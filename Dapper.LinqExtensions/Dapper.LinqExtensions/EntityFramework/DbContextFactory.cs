using Dapper.LinqExtensions.EntityFramework.Entities;
using Dapper.LinqExtensions.EntityFramework.Imples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework
{
    public class DbContextFactory
    {
        static DbContextFactory()
        {
            //数据库中带下划线的表字段自动匹配无下划线的Model字段。
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        /// <summary>
        /// 数据源列表
        /// </summary>
        private static List<DataSource> DataSource = new List<DataSource>();
        private static DataSource GetDataSource(string name = null)
        {
            DataSource tempDataSource = null;
            if (name == null)
            {
                tempDataSource= DataSource.Find(f => f.Default) ?? DataSource.FirstOrDefault();
            }
            else
            {
                tempDataSource= DataSource.Find(f => f.DatasourceName == name);
            }

            if (tempDataSource == null)
            {
                throw new Exception("没有数据库提供者");
            }
            return tempDataSource;

        }
        public static void AddDataSource(DataSource dataSource)
        {
            DataSource.Add(dataSource);
            if (dataSource.Default)
            {
                foreach (var item in DataSource)
                {
                    item.Default = false;
                }
            }
        }
        public static IDbContext GetDbContext(string name = null)
        {
            var datasource = GetDataSource(name);
            IDbContext session = null;

            if (datasource.UseProxy)
            {
                session = new DbContextProxy(new DbContext(datasource.ConnectionFacotry(), datasource.DatasourceType));
            }
            else
            {
                session = new DbContext(datasource.ConnectionFacotry(), datasource.DatasourceType);
            }
            return session;
        }
    }
}
