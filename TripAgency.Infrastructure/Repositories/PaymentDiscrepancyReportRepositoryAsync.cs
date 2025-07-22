using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PaymentDiscrepancyReportRepositoryAsync : GenericRepositoryAsync<PaymentDiscrepancyReport>, IPaymentDiscrepancyReportRepositoryAsync
    {
        public PaymentDiscrepancyReportRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
