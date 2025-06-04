using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PackageTripDestinationRepositoryAsync : GenericRepositoryAsync<PackageTripDestination>, IPackageTripDestinationRepositoryAsync
    {
        public PackageTripDestinationRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

    }

}
