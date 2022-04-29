using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Audit_service.DataAccess;
using Audit_service.Models.ExecuteModels;
using Microsoft.EntityFrameworkCore;


namespace Audit_service.Repositories
{
    public interface IRepository<T> where T : class
    {      
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        void Delete(T entity);
        void Add(T entity);
        void AddWithoutSave(T entity);
        void UpdateWithoutSave(T entity);
        void Update(T entity);
        int Save();
        void Insert(T entity);
        void Insert(ICollection<T> entities);
        void Update(ICollection<T> entities);
        void UpdateWithoutSave(ICollection<T> entities);
        void Delete(IEnumerable<T> entities);
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null);
        T Get(object key);
        T Get(Expression<Func<T, bool>> query);
        void Unchanged(T entity);
        IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly KitanoSqlContext _context;
        private const string EntityNotFound = "Data object is not found";
        protected DbSet<T> DbSet;

        public Repository(DbContext context)
        {
            if (context == null)
            {
                throw new Exception(EntityNotFound);
            }
            _context = (KitanoSqlContext)context;
            this.DbSet = this._context.Set<T>();
        }

        public void Insert(T entity)
        {
            this.DbSet.Add(entity);
        }

        public void Insert(ICollection<T> entities)
        {
            this.DbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            Save();
        }

        public void Update(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            Save();
        }

        public void Unchanged(T entity)
        {
            _context.Entry(entity).State = EntityState.Unchanged;
        }
        public void Delete(T entity)
        {
            this.DbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            this.DbSet.RemoveRange(entities);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null)
        {
            return expression == null ? this.DbSet : this.DbSet.Where(expression);
        }

        public T Get(object key)
        {
            return this.DbSet.Find(key);
        }
        public T Get(Expression<Func<T, bool>> query)
        {
            return this.DbSet.FirstOrDefault(query);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate).FirstOrDefault();
        }

        public void Add(T entity)
        {
            _context.Entry(entity).State = EntityState.Added;
            Save();
        }

        public void AddWithoutSave(T entity)
        {
            _context.Entry(entity).State = EntityState.Added;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void UpdateWithoutSave(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = this.DbSet;
            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }

            return query ?? this.DbSet;
        }

        public void UpdateWithoutSave(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
