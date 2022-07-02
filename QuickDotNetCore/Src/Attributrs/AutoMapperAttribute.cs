using System;
namespace QuickDotNetCore.Src.Attributrs
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class AutoMapperAttribute:Attribute
    {
        /// <summary>
        /// 是否自动映射建表
        /// </summary>
        public bool Mapper { get; private set; } = true;
        public AutoMapperAttribute(bool mapper = true) {
            this.Mapper = mapper;
        }

        public override object TypeId => base.TypeId;

    }
}
