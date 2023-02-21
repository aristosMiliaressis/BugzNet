using BugzNet.Core.SharedKernel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BugzNet.Infrastructure.DataEF.Repositories.Interfaces
{
    public interface IRepository
    {
        Task<List<T>> ListAsync<T>(CancellationToken token = default) where T : BaseEntity<int>;
        Task<List<T>> ListNoTrackingAsync<T>(CancellationToken token = default) where T : BaseEntity<int>;
        T Add<T>(T entity) where T : BaseEntity<int>;
        Task<T[]> AddAndSaveAsync<T>(params T[] entities) where T : BaseEntity<int>;
        Task<T> GetByIdAsync<T>(int id, CancellationToken token = default) where T : BaseEntity<int>;
        Task UpdateAsync<T>(params T[] entities) where T : BaseEntity<int>;
        Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : BaseEntity<int>;
        Task SaveAsync(CancellationToken token = default);
    }
 }
