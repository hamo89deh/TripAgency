using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class DestinationActivityRepositoryAsync : GenericRepositoryAsync<DestinationActivity>, IDestinationActivityRepositoryAsync
    {
        public DestinationActivityRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
