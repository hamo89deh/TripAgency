using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.BookingTrip.Commands;


namespace TripAgency.Service.Abstracts
{
    public interface IBookingTripService : IGenericService<BookingTrip, GetBookingTripByIdDto, GetBookingTripsDto , AddBookingTripDto, UpdateBookingTripDto>                                  
    {
    
    }
}
