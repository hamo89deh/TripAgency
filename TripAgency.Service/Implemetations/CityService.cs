using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.ServiceBases;

namespace TripAgency.Service.Implemetations
{
    public class CityService : GenericServices<City>, ICityService
    {
        private ICityRepositoryAsync _cityRepository {  get; set; } 
        public CityService(IGenericRepositoryAsync<City> repository ,
                           ICityRepositoryAsync cityRepository ) : base(repository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<City?> GetCityByNameAsync(string name)
        {
            return await _cityRepository.GetCityByName(name);           
        }
    }

    public class DestinationService : GenericServices<Destination>, IDestinationService
    {
        public DestinationService(IGenericRepositoryAsync<Destination> repository,
                                  IDestinationRepositoryAsync destinationRepository) : base(repository)
        {
            _destinationRepository = destinationRepository;
        }

        public IDestinationRepositoryAsync _destinationRepository { get; }

        public IQueryable<Destination?> GetDestinationByCityName(string name)
        {
            return _destinationRepository.GetDestinationsByCityName(name);
        }
    }
}
