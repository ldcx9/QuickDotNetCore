using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Attributrs;

namespace QuickDotNetCore.Src.Config
{
    public class RegisterDAOs
    {
        public static void Register(IServiceCollection services) {
            int count = 0;
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                Attribute attribute = item.GetCustomAttribute(typeof(DAOAttribute));
                if (attribute != null)
                {
                    count++;
                    DAOAttribute DAOAttributrs = attribute as DAOAttribute;
                    services.AddScoped(item, DAOAttributrs.TargetDAO);
                }
            }
            Console.WriteLine($"DAO层注入完成\t\t\t:\t{count}");
        }
    }
}
