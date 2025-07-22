using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class TripDestinationRepositoryAsync : GenericRepositoryAsync<TripDestination>, ITripDestinationRepositoryAsync
    {
        public TripDestinationRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
