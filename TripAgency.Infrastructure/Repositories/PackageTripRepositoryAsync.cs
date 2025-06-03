using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PackageTripRepositoryAsync : GenericRepositoryAsync<PackageTrip>, IPackageTripRepositoryAsync
    {
        public PackageTripRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

       
    }
}
