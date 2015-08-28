using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using CarsCatalog.Models;

namespace CarsCatalog.Repository
{
    public class BaseRepository<TC, T> : IDisposable
        where T : class
        where TC : DbContext, new()
    {
        private TC _dataContext;

        public TC DataContext
        {
            get { return _dataContext ?? (_dataContext = new TC()); }
        }

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null) throw new ApplicationException("Predicate value must be passed to Get<T>");
            using (DataContext)
            {
                return DataContext.Set<T>().Where(predicate).SingleOrDefault();
            }
        }

        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return DataContext.Set<T>().Where(predicate);
        }

        public virtual IQueryable<T> GetAll()
        {
            return DataContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            DataContext.Set<T>().Add(entity);
            DataContext.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            DataContext.Set<T>().Attach(entity);
            DataContext.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            DataContext.Set<T>().Remove(entity);
            DataContext.SaveChanges();
        }

        public virtual void Save()
        {
            DataContext.SaveChanges();
        }

        public void Dispose()
        {
            if (_dataContext == null) return;
            _dataContext.Dispose();
            _dataContext = null;
        }
    }
}