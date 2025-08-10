using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.BookingPassenger.Commands;
using TripAgency.Service.Feature.BookingPassenger.Queries;

namespace TripAgency.Service.Abstracts
{
    public interface IBookingPassengerService
    {
        Task<Result<IEnumerable<GetBookingPassengers>>> GetBookingPassengers(int BookingTripId);
        Task<Result> AddBookingPassenger(AddBookingPassengerDto addBookingPassenger);
        Task<Result> AddBookingPassengers(AddBookingPassengersDto addBookingPassengers);
        Task<Result> UpdateBookingPassenger(UpdateBookingPassengerDto updateBookingPassengers);
        Task<Result> DeleteBookingPassenger(int  BookingPassengerId);

    }
}
