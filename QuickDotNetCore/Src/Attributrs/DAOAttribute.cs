using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickDotNetCore.Src.Attributrs
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public class DAOAttribute:Attribute
    {
        public Type TargetDAO { get; set; }


        public DAOAttribute(Type targetDAO)
        {
            this.TargetDAO = targetDAO;
        }
    }
}
