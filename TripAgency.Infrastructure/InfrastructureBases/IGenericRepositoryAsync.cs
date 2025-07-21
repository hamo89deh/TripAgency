using Microsoft.EntityFrameworkCore.Storage;

namespace TripAgency.Infrastructure.InfrastructureBases
{
    public interface IGenericRepositoryAsync<T>
    {
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T?> GetByIdAsync(int id);
        Task DeleteRangeAsync(ICollection<T> entities);
        Task AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task Commit();
        Task RollBack();

        Task SaveChangesAsync();

    }
}
