using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class TripDateRepositoryAsync : GenericRepositoryAsync<PackageTripDate>, IPackageTripDateRepositoryAsync
    {
        public TripDateRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
