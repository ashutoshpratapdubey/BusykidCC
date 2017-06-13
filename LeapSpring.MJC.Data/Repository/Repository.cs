using LeapSpring.MJC.Core.Domain;
using System;
using System.Linq;

namespace LeapSpring.MJC.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly MJCDbContext _dbContext;

        // Ctor
        public Repository()
        {
            _dbContext = new MJCDbContext();
        }

        /// <summary>
        /// Table
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns>T</returns>
        public IQueryable<T> Table<T>() where T : BaseEntity
        {
            return _dbContext.Set<T>();
        }

        /// <summary>
        /// Find entity using identifier
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="id">identifier</param>
        /// <returns>T</returns>
        public T Find<T>(int id) where T : BaseEntity, new()
        {
            return _dbContext.Set<T>().SingleOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Insert new entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param>        
        public void Insert<T>(T entity) where T : BaseEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param> 
        public void Update<T>(T entity) where T : BaseEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param>
        public void Delete<T>(T entity) where T : BaseEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}
