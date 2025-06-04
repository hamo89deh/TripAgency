using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface IActivityRepositoryAsync : IGenericRepositoryAsync<Activity>
    {
        public Task<Activity?> GetActivityByName(string Name);
    }
}
