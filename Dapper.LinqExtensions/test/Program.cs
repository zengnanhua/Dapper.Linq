using Dapper.LinqExtensions.EntityFramework;
using Dapper.LinqExtensions.EntityFramework.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace test
{
    public class Zmn_Ac_Users
    {
        public string UserName { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            DbContextFactory.AddDataSource(new DataSource()
            {
                Default = true,
                DatasourceName = "mySql",
                ConnectionFacotry = () => new SqlConnection("Data Source=(localdb)\\ProjectsV13;Initial Catalog=UserAdminSystem;Integrated Security=True;Pooling=False;"),
                DatasourceType = DatasourceType.SQLSERVER,
                UseProxy = true
            });
            var nin = DbContextFactory.GetDbContext();
            var dd = nin.From<Zmn_Ac_Users>()
            .Select(c => new
            {
                b = c.UserName + "sdfasdfa"
            }).ToList();
        }
    }
}
