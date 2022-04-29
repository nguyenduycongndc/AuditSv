using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Audit_service.Repositories;
using Audit_service.DataAccess;
using Audit_service.Models.MigrationsModels;

namespace Audit_service.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        IRepository<T> Repository<T>() where T : class;
    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly KitanoSqlContext _dbContext;
        public UnitOfWork(KitanoSqlContext dbcontext)
        {
            _dbContext = dbcontext;
            InitializeContext();
        }
        protected void InitializeContext()
        {
            _dbContext.ChangeTracker.LazyLoadingEnabled = true;
            _dbContext.Database.EnsureCreated();
            _dbContext.SaveChanges();
        }
        //transaction
        public ITransaction BeginTransaction()
        {
            return new Transaction(this);
        }
        public void EndTransaction(ITransaction transaction)
        {
            if (transaction != null)
            {
                (transaction as IDisposable).Dispose();
                transaction = null;
            }
        }
        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(_dbContext);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
        //Dispposable
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
