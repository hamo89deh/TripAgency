using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PackageTripOffersRepositoryAsync : GenericRepositoryAsync<PackageTripOffers>, IPackageTripOffersRepositoryAsync
    {
        public PackageTripOffersRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
