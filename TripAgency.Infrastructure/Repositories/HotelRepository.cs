using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class HotelRepository : GenericRepositoryAsync<Hotel>, IHotelRepositoryAsync
    {
        public HotelRepository(TripAgencyDbContext context) : base(context)
        {
           
        }

        public async Task<Hotel?> GetHotelByName(string name)
        {
            return await _dbContext.Set<Hotel>().FirstOrDefaultAsync(h=>h.Name == name);
        }

        public IQueryable<Hotel?> GetHotelsByCityName(string cityName)
        {
            return  _dbContext.Set<Hotel>().Include(h=>h.City).Where(h => h.City.Name == cityName);
        }
    }
}
