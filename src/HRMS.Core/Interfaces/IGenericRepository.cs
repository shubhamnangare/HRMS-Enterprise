using HRMS.Core.Entities.Base;
using System.Linq.Expressions;

namespace HRMS.Core.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // Basic CRUD
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task DeleteAsync(int id);

    // Utility methods
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}