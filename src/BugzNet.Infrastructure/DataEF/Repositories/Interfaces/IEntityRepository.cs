using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BugzNet.Core.SharedKernel;

namespace BugzNet.Infrastructure.DataEF.Repositories.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityRepository<T> where T : BaseEntity<int>
    {
        /// <summary>
        /// Returns the List of all Entities specified by the predicate
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetList(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Ruturns a list of entities based on the predicate and if not found tries to get the 
        /// default value which is the first entry in the store
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetListOrDefault(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Ruturns the entity based on the id and if not found tries to get the 
        /// default value which is the first entry in the store
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetByIdOrDefault(int Id);
        /// <summary>
        /// Returns the List of all Entities specified by the predicate as No Tracking
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetListNoTrack(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Returns an entity from the as no Tracked by the datacontext
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetByIdNoTrack(int Id);
        /// <summary>
        /// Returns an entity from the as Tracked by the datacontext
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetById(int Id);
    }
}
