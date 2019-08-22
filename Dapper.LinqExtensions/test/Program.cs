using Dapper.LinqExtensions.Attributes;
using Dapper.LinqExtensions.EntityFramework;
using Dapper.LinqExtensions.EntityFramework.Entities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace test
{
    [Table("zmd_ac_users")]
    public class Zmn_Ac_Users
    {
        public string UserName { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //DbContextFactory.AddDataSource(new DataSource()
            //{
            //    Default = true,
            //    DatasourceName = "mySql",
            //    ConnectionFacotry = () => new SqlConnection("Data Source=(localdb)\\ProjectsV13;Initial Catalog=UserAdminSystem;Integrated Security=True;Pooling=False;"),
            //    DatasourceType = DatasourceType.SQLSERVER,
            //    UseProxy = true
            //});

            //DbContextFactory.AddDataSource(new DataSource()
            //{
            //    Default = true,
            //    DatasourceName = "mySql",
            //    ConnectionFacotry = () => new OracleConnection("Data Source=(DESCRIPTION =(ADDRESS_LIST =( ADDRESS = (PROTOCOL = TCP)(HOST = 172.16.45.126)(PORT = 1530)))(CONNECT_DATA = (SID = rmsdb) ));User Id=ZMD;Password=ZMD;"),
            //    DatasourceType = DatasourceType.ORACLE,
            //    UseProxy = true
            //});

            //var nin = DbContextFactory.GetDbContext();
            //var dd = nin.From<Zmn_Ac_Users>()
            // .Where(c=>c.UserName== "JM01001000002")
            //.Select(c => new
            //{
            //    b = c.UserName + "sdfasdfa"
            //}).ToList();

            //变量表达式
            ParameterExpression i = Expression.Variable(typeof(int), "i");
            //变量表达式
            ParameterExpression sum = Expression.Variable(typeof(int), "sum");
            //跳出循环标志
            LabelTarget label = Expression.Label(typeof(int));
            //块表达式
            BlockExpression block =
                Expression.Block(
                    //添加局部变量
                    new[] { sum },
                    //为sum赋初值 sum=1
                    //Assign表示赋值运算符
                    Expression.Assign(sum, Expression.Constant(1, typeof(int))),
                    //loop循环
                    Expression.Loop(
                        //如果为true 然后求和，否则跳出循环
                        Expression.IfThenElse(
                        //如果i>=0
                        Expression.GreaterThanOrEqual(i, Expression.Constant(0, typeof(int))),
                                  //sum=sum+i;i++;
                                  Expression.AddAssign(sum, Expression.PostDecrementAssign(i)),
                            //跳出循环
                            Expression.Break(label, sum)
                        ), label
                     )  // Loop ends
                 );
            int resutl = Expression.Lambda<Func<int, int>>(block, i).Compile()(100);
            Console.WriteLine(resutl);
            Console.Read();
        }
    }
}
