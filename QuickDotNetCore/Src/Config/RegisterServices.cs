using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QuickDotNetCore.Src.Attributrs;

namespace QuickDotNetCore.Src.Config
{
    public class RegisterServices
    {
        public static void Register(IServiceCollection services) {
            int count = 0;
            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                Attribute attribute = item.GetCustomAttribute(typeof(ServiceAttribute));
                if (attribute != null)
                {
                    count++;
                    ServiceAttribute serviceAttribute = attribute as ServiceAttribute;
                    services.AddScoped(item,serviceAttribute.TargetService);
                }
            }
            Console.WriteLine($"Service层注入完成\t\t:\t{count}");
        }
    }
}
