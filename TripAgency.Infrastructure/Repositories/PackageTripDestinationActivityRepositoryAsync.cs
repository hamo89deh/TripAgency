using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PackageTripDestinationActivityRepositoryAsync : GenericRepositoryAsync<PackageTripDestinationActivity>, IPackageTripDestinationActivityRepositoryAsync
    {
        public PackageTripDestinationActivityRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

    }

}
