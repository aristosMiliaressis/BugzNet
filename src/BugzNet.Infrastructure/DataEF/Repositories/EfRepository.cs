using BugzNet.Core.SharedKernel;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using BugzNet.Infrastructure.DataEF.Repositories.Interfaces;

namespace BugzNet.Infrastructure.DataEF
{
    /// <summary>
    /// this is the generic repository from EF Entities
    /// </summary>
    public class EfRepository : IRepository
    {
        protected readonly BugzNetDataContext _dbContext;

        public EfRepository(BugzNetDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetByIdAsync<T>(int id, CancellationToken token = default) where T : BaseEntity<int>
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == id, token);
        }

        public async Task<List<T>> ListAsync<T>(CancellationToken token = default) where T : BaseEntity<int>
        {
            return await _dbContext.Set<T>().ToListAsync(token);
        }

        public async Task<List<T>> ListNoTrackingAsync<T>(CancellationToken token = default) where T : BaseEntity<int>
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync(token);
        }

        public T Add<T>(T entity) where T : BaseEntity<int>
        {
            _dbContext.Set<T>().Add(entity);

            return entity;
        }
        public async Task<T[]> AddAndSaveAsync<T>(params T[] entities) where T : BaseEntity<int>
        {
            _dbContext.Set<T>().AddRange(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : BaseEntity<int>
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task UpdateAsync<T>(params T[] entities) where T : BaseEntity<int>
        {
            entities.ToList().ForEach(e => _dbContext.Entry(e).State = EntityState.Modified);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveAsync(CancellationToken token = default)
        {
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
