using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface IDestinationRepositoryAsync : IGenericRepositoryAsync<Destination>
    {
        public Task<Destination?> GetDestinationByName(string Name);
        public IQueryable<Destination?> GetDestinationsByCityName(string CityName);
    }
}
