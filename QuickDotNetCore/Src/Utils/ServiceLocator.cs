using System;

namespace TingBao_API.Src.utils
{
    public class ServiceLocator
    {
        public static IServiceProvider Services { get; private set; }

        public static void SetServices(IServiceProvider services)
        {
            Services = services;
        }
    }
}
