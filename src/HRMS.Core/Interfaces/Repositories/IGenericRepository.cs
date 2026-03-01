using HRMS.Core.Specifications;
using System.Linq.Expressions;

namespace HRMS.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Get operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // Add operations
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Delete operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Count operations
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        // Specification pattern
        Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> spec);
        Task<T?> GetSingleBySpecificationAsync(ISpecification<T> spec);

        // Pagination
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetPagedWithPredicateAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize);
    }
}