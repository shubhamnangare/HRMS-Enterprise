using HRMS.Core.Entities;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface IAuditRepository : IRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entity, string entityId);
        Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}