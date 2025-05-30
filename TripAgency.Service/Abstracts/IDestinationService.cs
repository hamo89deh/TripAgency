using TripAgency.Data.Entities;
using TripAgency.Service.ServiceBases;

namespace TripAgency.Service.Abstracts
{
    public interface IDestinationService : IGenericServices<Destination>
    {
        public IQueryable<Destination?> GetDestinationByCityName(string name);
    }
}
