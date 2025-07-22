using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class ActivityRepositoryAsync : GenericRepositoryAsync<Activity>, IActivityRepositoryAsync
    {
        public ActivityRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Activity?> GetActivityByName(string Name)
        {
            return await _dbContext.Set<Activity>().FirstOrDefaultAsync(x => x.Name.Contains(Name));
        }
    }
}
