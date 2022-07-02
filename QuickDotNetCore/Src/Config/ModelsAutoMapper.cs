using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickDotNetCore.Src.Attributrs;
using QuickDotNetCore.Src.Utils.BDSeed;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Config
{
    public class ModelsAutoMapper
    {
        public static void Mapper(IConfiguration configuration,string dbName = "MySql")
        {
            int count = 0;
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                Attribute attribute = item.GetCustomAttribute(typeof(AutoMapperAttribute));
                List<Type> os = new();
                if (attribute != null)
                {
                    AutoMapperAttribute autoMapper = attribute as AutoMapperAttribute;
                    if (autoMapper.Mapper)
                    {
                        count++;
                        os.Add(item.Assembly.GetType(item.FullName));
                    }
                }
                DBSeed.Seed(configuration.GetConnectionString(dbName),os.ToArray());
            }
            if (count > 0)
            {
                Console.WriteLine($"Models实体映射完成\t\t:\t{count}");
            }
        }
    }
}
