
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MissionskyOA.Core;

namespace MissionskyOA.Data
{
    public class EntityRespository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DbContext _context;

        private IDbSet<T> _entities;

        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        public T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Add(entity);
            this._context.SaveChanges();
        }

        public void Insert(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.Entities.Add(entity);
            }

            this._context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            this._context.Entry(entity).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this._context.Entry(entities).State = EntityState.Modified;
            }
            this._context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Remove(entity);
            this._context.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.Entities.Remove(entity);
            }

            this._context.SaveChanges();
        }

        public IQueryable<T> Table
        {
            get { return this.Entities; }
        }

        public IQueryable<T> NoTracingTable
        {
            get { return this.Entities.AsNoTracking(); }
        }
    }
}