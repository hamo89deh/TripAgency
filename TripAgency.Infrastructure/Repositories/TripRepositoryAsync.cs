using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class TripRepositoryAsync : GenericRepositoryAsync<Trip>, ITripRepositoryAsync
    {
        public TripRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
      
        public async Task<Trip?> GetTripByName(string Name)
        {
            return await _dbContext.Set<Trip>().FirstOrDefaultAsync(x => x.Name.Contains(Name));
        }
    }
}
