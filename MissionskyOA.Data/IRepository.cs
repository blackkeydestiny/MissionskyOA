using System.Collections.Generic;
using System.Linq;
using MissionskyOA.Core;

namespace MissionskyOA.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);

        /// <summary>
        /// insert
        /// </summary>
        /// <param name="entity"></param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// update entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// Delete entity;
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Delete Entities;
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        ///
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        ///
        /// </summary>
        IQueryable<T> NoTracingTable { get; }
    }
}