using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Implementations;


namespace TripAgency.Service.Abstracts
{
    public interface IBookingTripService : IGenericService<BookingTrip, GetBookingTripByIdDto, GetBookingTripsDto , AddBookingTripDto, UpdateBookingTripDto>                                  
    {
        Task<Result<PaymentInitiationResponseDto>> InitiateBookingAndPaymentAsync(AddBookingPackageTripDto bookPackageDto);
        Task<Result> CancellingBookingAndRefundPayemntAsync(int bookingId);
     
    }


}
