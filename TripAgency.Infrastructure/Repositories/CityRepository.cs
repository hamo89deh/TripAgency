using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class CityRepositoryAsync : GenericRepositoryAsync<City>, ICityRepositoryAsync
    {
        public CityRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }

        public  async Task<City?> GetCityByName(string Name)
        {
           return await _dbContext.Set<City>().FirstOrDefaultAsync(x => x.Name .Contains(Name));  



        }
    }
    
}
