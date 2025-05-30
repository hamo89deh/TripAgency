using Microsoft.EntityFrameworkCore.Storage;

namespace TripAgency.Infrastructure.InfrastructureBases
{
    public interface IGenericRepositoryAsync<T>
    {
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T?> GetByIdAsync(int id);
        Task DeleteRangeAsync(ICollection<T> entities);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();

        Task SaveChangesAsync();

    }
}
