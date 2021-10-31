using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nero2021.Data;
using System.Linq.Expressions;

namespace Nero2021.BLL.Repository 
{
    public class RepositoryBase<T, ID> where T : class
    {
        protected internal static NeroDBEntities dbContext;

        public virtual int Count(Expression<Func<T, bool>> filter)
        {
            try
            {
                dbContext = dbContext ?? new NeroDBEntities();
                return dbContext.Set<T>().Where(filter).Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public virtual List<T> GetAll()
        {
            dbContext = new NeroDBEntities();
            return dbContext.Set<T>().ToList();
        }
        public virtual List<T> GetAll(Expression<Func<T, bool>> filter)
        {
            dbContext = new NeroDBEntities();
            return dbContext.Set<T>().Where(filter).ToList();
        }

        public virtual T GetByID(ID id)
        {
            dbContext = new NeroDBEntities();
            return dbContext.Set<T>().Find(id);
        }
        public virtual T GetByFilter(Expression<Func<T, bool>> filter)
        {
            try
            {
                dbContext = new NeroDBEntities();
                return dbContext.Set<T>().Where(filter).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public virtual int Insert(T entity)
        {
            try
            {
                dbContext = dbContext ?? new NeroDBEntities();
                dbContext.Set<T>().Add(entity);
                return dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual int Delete(T entity)
        {
            try
            {
                dbContext = dbContext ?? new NeroDBEntities();
                dbContext.Set<T>().Remove(entity);
                return dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual int Update()
        {
            try
            {
                dbContext = dbContext ?? new NeroDBEntities();
                return dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual IQueryable<T> Queryable()
        {
            try
            {
                dbContext =  new NeroDBEntities();
                return dbContext.Set<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
