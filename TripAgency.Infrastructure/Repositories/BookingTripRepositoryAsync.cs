using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{

    public class BookingTripRepositoryAsync : GenericRepositoryAsync<BookingTrip>, IBookingTripRepositoryAsync
    {
        public BookingTripRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {

        }

    

        public IQueryable<BookingTrip> GetBookingTrips(int TripDateId, PaymentStatus paymentStatus)
        {
            return _dbContext.Set<BookingTrip>()
                                        .Include(x=>x.Payment)
                                        .Where(x => x.PackageTripDateId == TripDateId && x.Payment.PaymentStatus == paymentStatus);
        }
    }
}
