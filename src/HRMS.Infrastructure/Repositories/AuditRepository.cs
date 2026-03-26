using HRMS.Core.Entities;
using HRMS.Core.Interfaces.Repositories;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Infrastructure.Repositories
{
    public class AuditRepository : GenericRepository<AuditLog>, IAuditRepository
    {
        public AuditRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity, string entityId)
        {
            return await _dbSet
                .Where(a => a.Entity == entity && a.EntityId == entityId && !a.IsDeleted)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dbSet.Where(a => a.UserId == userId && !a.IsDeleted);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}