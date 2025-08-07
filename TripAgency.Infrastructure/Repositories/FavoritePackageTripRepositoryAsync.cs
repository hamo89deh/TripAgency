using TripAgency.Data.Entities.Identity;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class FavoritePackageTripRepositoryAsync : GenericRepositoryAsync<FavoritePackageTrip>, IFavoritePackageTripRepositoryAsync
    {
        public FavoritePackageTripRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }
}
