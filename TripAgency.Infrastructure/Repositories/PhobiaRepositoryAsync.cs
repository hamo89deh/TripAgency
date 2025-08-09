using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PhobiaRepositoryAsync : GenericRepositoryAsync<Phobia>, IPhobiaRepositoryAsync
    {
        public PhobiaRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
