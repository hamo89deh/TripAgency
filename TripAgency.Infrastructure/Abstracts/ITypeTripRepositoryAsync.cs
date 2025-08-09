using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface ITripTypeRepositoryAsync : IGenericRepositoryAsync<TypeTrip>
    {
        public Task<TypeTrip?> GetTypeTripByName(string Name);
    }
}
