using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class ActivityPhobiasRepositoryAsync : GenericRepositoryAsync<ActivityPhobias >, IActivityPhobiasRepositoryAsync
    {
        public ActivityPhobiasRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
