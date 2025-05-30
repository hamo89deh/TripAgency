using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class DestinationRepositoryAsync : GenericRepositoryAsync<Destination>, IDestinationRepositoryAsync
    {
        public DestinationRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Destination?> GetDestinationByName(string Name)
        {
           return await _dbContext.Set<Destination>().FirstOrDefaultAsync(x => x.Name == Name);

        }

        public IQueryable<Destination?> GetDestinationsByCityName(string CityName)
        {
           return  _dbContext.Set<Destination>()
                             .Include(x => x.City)
                             .Where(x => x.City.Name == CityName);

        }
    }
}
