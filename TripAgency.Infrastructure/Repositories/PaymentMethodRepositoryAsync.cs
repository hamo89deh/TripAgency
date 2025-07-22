using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PaymentMethodRepositoryAsync : GenericRepositoryAsync<PaymentMethod>, IPaymentMethodRepositoryAsync
    {
        public PaymentMethodRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }
}
