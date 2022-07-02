using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Config
{
    public static class SqlsugarSetup
    {

        /// <summary>
        /// 注入数据库 数据库连接字符串名称  appsettings.json 里面配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="dbName"></param>
        /// <param name="dbType"></param>
        public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration configuration,
        string dbName = "MySQL", DbType dbType = DbType.MySql)
        {
            SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = dbType,
                ConnectionString = configuration.GetConnectionString(dbName),
                IsAutoCloseConnection = true,
            },
                db =>
                {
                    //单例参数配置，所有上下文生效
                    db.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            Console.WriteLine(sql);//输出sql
                        };
                });
            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
