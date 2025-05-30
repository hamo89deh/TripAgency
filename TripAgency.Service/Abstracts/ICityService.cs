using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;

using TripAgency.Service.ServiceBases;

namespace TripAgency.Service.Abstracts
{
    public interface ICityService : IGenericServices<City>
    {
        public Task<City> GetCityByNameAsync(string name);
    }
}
