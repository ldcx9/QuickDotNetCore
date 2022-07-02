using System;

namespace QuickDotNetCore.Src.Attributrs
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        public Type TargetService { get; set; }

        public Type DAO { get; set; }

        public ServiceAttribute(Type TargetService, Type dao = null)
        {
            this.TargetService = TargetService;
            this.DAO = dao;
        }
    }
}
