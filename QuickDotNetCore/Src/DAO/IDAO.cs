using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QuickDotNetCore.Src.DAO
{
    public interface IDAO<E>
    {
        E Insert(E e);
        void InsertRange(List<E> es);

        E FindFirst(Expression<Func<E, bool>> exp);
        List<E> FindChird(Expression<Func<E, object>> exp, object id);

        bool Delete(E e);

        bool Update(E e);

        long Count { get; }

        List<E> All();

        List<E> FindAll(Expression<Func<E,bool>> exp);

        void AddOrUpdate(E e, Expression<Func<E, bool>> exp);

        /// <summary>
        /// 条件分页查询带排序
        /// </summary>
        /// <typeparam name="Tkey">排序类型</typeparam>
        /// <param name="pageIndex">页号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="whereExp">条件</param>
        /// <param name="orderExp">排序</param>
        /// <returns></returns>
        List<E> LimitPageWhereOrder<Tkey>(int pageIndex, int pageSize, Func<E, bool> whereExp, Func<E, Tkey> orderExp);

        List<E> LimitPageOrder<Tkey>(int pageIndex, int pageSize, Func<E, Tkey> orderExp);

        List<E> LimitPageWhere(int pageIndex, int pageSize, Func<E, bool> wheleExp);

        List<E> LimitPage(int pageIndex, int pageSize);
    }
}
