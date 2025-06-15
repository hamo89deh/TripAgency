using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class BookingTrip : GenericRepositoryAsync<BookingTrip>, IGenericRepositoryAsync<BookingTrip>
    {
        public BookingTrip(TripAgencyDbContext dbContext) : base(dbContext)
        {

        }
    }
}
