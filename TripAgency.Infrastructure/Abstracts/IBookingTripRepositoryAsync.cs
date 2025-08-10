using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface IBookingTripRepositoryAsync : IGenericRepositoryAsync<BookingTrip>
    {
        public IQueryable<BookingTrip> GetBookingTrips(int PackageTripDateId, PaymentStatus paymentStatus  );
       
    } 
}
