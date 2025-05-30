using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface ICityRepositoryAsync : IGenericRepositoryAsync<City>
    {
        public Task<City?> GetCityByName(string Name);
    }
}
