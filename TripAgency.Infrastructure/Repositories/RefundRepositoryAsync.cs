using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class RefundRepositoryAsync : GenericRepositoryAsync<Refund>, IRefundRepositoryAsync
    {
        public RefundRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }
}
