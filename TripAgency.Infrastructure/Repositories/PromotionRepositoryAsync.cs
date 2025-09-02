using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PromotionRepositoryAsync : GenericRepositoryAsync<Promotion>, IPromotionRepository
    {
        public PromotionRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
