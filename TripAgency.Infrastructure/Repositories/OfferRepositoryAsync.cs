using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class OfferRepositoryAsync : GenericRepositoryAsync<Offer>, IOffersRepositoryAsync
    {
        public OfferRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
