using TripAgency.Data.Entities;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface IHotelRepositoryAsync : IGenericRepositoryAsync<Hotel>
    {
        public Task<Hotel?> GetHotelByName(string name);
        public IQueryable<Hotel?> GetHotelsByCityName(string cityName);
    }
}
