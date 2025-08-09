using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class TypeTripRepositoryAsync : GenericRepositoryAsync<TypeTrip>, ITripTypeRepositoryAsync
    {
        public TypeTripRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<TypeTrip?> GetTypeTripByName(string Name)
        {
            return await _dbContext.Set<TypeTrip>().FirstOrDefaultAsync(x => x.Name.Contains(Name));
        }
    }
}
