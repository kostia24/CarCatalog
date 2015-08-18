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
            try
            {
                return DataContext.Set<T>().Where(predicate);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public virtual IQueryable<T> GetAll()
        {
            try
            {
                return DataContext.Set<T>();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public virtual OperationStatus Add(T entity)
        {
            OperationStatus status = new OperationStatus() { Status = true };
            try
            {
                DataContext.Set<T>().Add(entity);
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromExeption(string.Format("Error adding {0}.", typeof(T)), ex);
            }
            return status;
        }

        public virtual OperationStatus Update(T entity)
        {
            OperationStatus status = new OperationStatus() { Status = true };
                try
                {
                    DataContext.Set<T>().Attach(entity);
                    status.Status = DataContext.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    status = OperationStatus.CreateFromExeption(string.Format("Error updating {0}.", typeof(T)), ex);
                }
            return status;
        }

        public virtual OperationStatus Delete(T entity)
        {
            OperationStatus status = new OperationStatus() { Status = true };
            try
            {
                DataContext.Set<T>().Remove(entity);
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromExeption(string.Format("Error deleting {0}.", typeof(T)), ex);
            }
            return status;
        }
        public virtual OperationStatus Save()
        {
            OperationStatus status = new OperationStatus() { Status = true };
            try
            {
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromExeption(string.Format("Error saving {0}.", typeof(T)), ex);
            }
            return status;
        }

        public void Dispose()
        {
            if (_dataContext == null) return;
            _dataContext.Dispose();
            _dataContext = null;
        }
    }
}