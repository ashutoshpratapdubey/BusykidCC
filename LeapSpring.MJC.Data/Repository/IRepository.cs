using LeapSpring.MJC.Core.Domain;
using System.Linq;

namespace LeapSpring.MJC.Data.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Table
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns>T</returns>
        IQueryable<T> Table<T>() where T : BaseEntity;

        /// <summary>
        /// Find entity using identifier
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="id">identifier</param>
        /// <returns>T</returns>
        T Find<T>(int id) where T : BaseEntity, new();

        /// <summary>
        /// Insert new entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param>
        void Insert<T>(T entity) where T : BaseEntity, new();

        /// <summary>
        /// Update entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param>        
        void Update<T>(T entity) where T : BaseEntity, new();

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entity">Entity</param>
        void Delete<T>(T entity) where T : BaseEntity, new();
    }
}
