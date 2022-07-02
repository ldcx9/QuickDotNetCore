using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SqlSugar;

namespace QuickDotNetCore.Src.DAO
{
    public class BaseDAO<E> : IDAO<E> where E : class, new()
    {
        private readonly ISqlSugarClient dbContext;

        public BaseDAO(ISqlSugarClient dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<E> FindChird(Expression<Func<E, object>> exp, object id)
        {
            //dbContext.Queryable<E>().ToChildList(exp, id);
            //db.Queryable<Student>().First(it=>it.Id==1)
            return dbContext.Queryable<E>().ToChildList(exp, id);
        }

        public E FindFirst(Expression<Func<E, bool>> exp)
        {
            //db.Queryable<Student>().First(it=>it.Id==1)
            return dbContext.Queryable<E>().First(exp);
        }

        public E Insert(E e)
        {
            try
            {
                E r = dbContext.Insertable<E>(e).ExecuteReturnEntity();
                return r;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void InsertRange(List<E> es) {
            if (es.Count > 500)
            {
                dbContext.Fastest<E>().BulkCopy(es);
            }
            else {
                dbContext.Insertable(es).ExecuteCommand();
            }
        }

        public bool Delete(E e)
        {
            bool b = dbContext.Deleteable(e).ExecuteCommandHasChange();
            return b;
        }

        public bool Delete(Expression<Func<E, bool>> expression)
        {
            bool b = dbContext.Deleteable<E>().Where(expression).ExecuteCommandHasChange();
            return b;
        }

        public bool Update(E e)
        {
            bool r = dbContext.Updateable(e).ExecuteCommandHasChange();
            return r;
        }

        public long Count {
            get
            {
                return dbContext.Queryable<E>().Count();
            }
        }


        public List<E> All()
        {
            return dbContext.Queryable<E>().ToList();
        }

        public List<E> FindAll(Expression<Func<E, bool>> exp)
        {
            return dbContext.Queryable<E>().Where(exp).ToList();
        }

        public List<E> LimitPageWhereOrder<Tkey>(int pageIndex, int pageSize, Func<E, bool> whereExp, Func<E, Tkey> orderExp)
        {
            ISugarQueryable<E> dbSet = dbContext.Queryable<E>();
            return dbSet.ToList().Where(whereExp).OrderBy(orderExp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<E> LimitPageOrder<Tkey>(int pageIndex, int pageSize, Func<E, Tkey> orderExp)
        {
            ISugarQueryable<E> dbSet = dbContext.Queryable<E>();
            return dbSet.ToList().OrderBy(orderExp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<E> LimitPageWhere(int pageIndex, int pageSize, Func<E, bool> whereExp)
        {
            ISugarQueryable<E> dbSet = dbContext.Queryable<E>();
            return dbSet.ToList().Where(whereExp).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<E> LimitPage(int pageIndex, int pageSize)
        {
            ISugarQueryable<E> dbSet = dbContext.Queryable<E>();
            return dbSet.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

         public void AddOrUpdate(E e, Expression<Func<E, bool>> exp)
        {
            ISugarQueryable<E> dbSet = dbContext.Queryable<E>();
            ISugarQueryable<E> queryable = dbSet.Where(exp);
            if (queryable.Count() < 1)
            {
                dbContext.Insertable(e).ExecuteCommand();
            }
            else
            {
                dbSet.Where(exp).ForEach(new Action<E>((d) => {
                    dbContext.Updateable<E>(e).ExecuteCommand();
                }));
            }
        }
    }
}
