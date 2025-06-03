using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface ITripRepositoryAsync : IGenericRepositoryAsync<Trip>
    {
        public Task<Trip?> GetTripByName(string Name);
    }
}
