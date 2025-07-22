using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface ITypeTripRepositoryAsync : IGenericRepositoryAsync<TypeTrip>
    {
        public Task<TypeTrip?> GetTypeTripByName(string Name);
    }
}
