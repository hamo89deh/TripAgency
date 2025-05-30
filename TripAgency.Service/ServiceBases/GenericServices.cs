using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Service.ServiceBases

{
    public class GenericServices<T> : IGenericServices<T> where T : class
    {
        protected IGenericRepositoryAsync<T> _repository { get; set; }
        public GenericServices(IGenericRepositoryAsync<T> repository)
        {
            _repository = repository;
        }

        public IQueryable<T> GetAll()
        {
            return _repository.GetTableNoTracking();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(T entity)
        {
             await _repository.AddAsync(entity);

        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);

        }

        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }
    }
}
