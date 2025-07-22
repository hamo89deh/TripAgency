using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PaymentRepositoryAsync : GenericRepositoryAsync<Payment>, IPaymentRepositoryAsync
    {
        public PaymentRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }
}
