using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingPassenger.Commands;
using TripAgency.Service.Feature.BookingPassenger.Queries;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.Payment;

namespace TripAgency.Service.Implemetations
{
    public class BookingPassengerService : IBookingPassengerService
    {
        public IBookingPassengerRepositoryAsync _bookingPassengerRepositoryAsync { get; }
        public IBookingTripRepositoryAsync _bookingTripRepositoryAsync { get; }

        public BookingPassengerService(IBookingPassengerRepositoryAsync bookingPassengerRepositoryAsync,
                                       IBookingTripRepositoryAsync bookingTripRepositoryAsync
                                      )
        {
            _bookingPassengerRepositoryAsync = bookingPassengerRepositoryAsync;
            _bookingTripRepositoryAsync = bookingTripRepositoryAsync;
            
        }

        public async Task<Result<IEnumerable<GetBookingPassengersDto>>> GetBookingPassengers(int BookingTripId)
        {
            var BookingTrip = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                           .FirstOrDefaultAsync(b=>b.Id == BookingTripId);
            if (BookingTrip is null)
                return Result<IEnumerable<GetBookingPassengersDto>>.NotFound($"Not Found Any Booking With id : {BookingTripId}");

            var BookingPassengers = await _bookingPassengerRepositoryAsync.GetTableNoTracking()
                                                                          .Where(b => b.BookingTripId == BookingTripId)
                                                                          .ToListAsync();
            if (BookingPassengers.Count() == 0)
                return Result<IEnumerable<GetBookingPassengersDto>>.NotFound($"Not Found Any Passenger for Booking with Id :{BookingTripId}");

            var result = new List<GetBookingPassengersDto>();
            foreach (var BookingPassenger in BookingPassengers)
            {
                result.Add(new GetBookingPassengersDto
                {
                    Id = BookingPassenger.Id,
                    FullName = BookingPassenger.FullName,
                    Age = BookingPassenger.Age,
                    PhoneNumber = BookingPassenger.PhoneNumber
                });
            }
            return Result<IEnumerable<GetBookingPassengersDto>>.Success(result);
        }
        public async Task<Result> AddBookingPassengers(AddBookingPassengersDto addBookingPassengers)
        {
            var BookingTrip = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                           .FirstOrDefaultAsync(b => b.Id == addBookingPassengers.BookingTripId);
            if (BookingTrip is null)
                return Result.NotFound($"Not Found Any BookingTrip With id : {addBookingPassengers.BookingTripId}");

            if(BookingTrip.BookingStatus != BookingStatus.Completed)
            {
                return Result.BadRequest("Cann't Add New Passengers For Booking Not Completed");
            }

            var BookingPassenger = await _bookingPassengerRepositoryAsync.GetTableAsTracking()
                                                                         .Where(x => x.BookingTripId == BookingTrip.Id)
                                                                         .ToListAsync();
            if (BookingPassenger.Count() != 0)
            {
                return Result.BadRequest("Cann't Add List Of Passenger");//TODO Changer message
            }

            if(BookingTrip.PassengerCount != addBookingPassengers.BookingPassengersDto.Count())
            {
                if(BookingTrip.PassengerCount > addBookingPassengers.BookingPassengersDto.Count())
                {
                    return Result.BadRequest("Count of Passengers For Booking is Greater than the Number of Passengers Sent");

                }
                else
                {
                    return Result.BadRequest("Count of Passengers For Booking is Smaller than the Number of Passengers Sent");

                }
            }
            if ( BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                 BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed ||
                 BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled ||
                 BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Draft)
            {
                return Result.BadRequest($"Cann't Add new Passengers for {BookingTrip.PackageTripDate.Status} Trip");
            }
            var bookingPassengersMapping = new List<BookingPassenger>();
            foreach (var BookingPassengerDto in addBookingPassengers.BookingPassengersDto)
            {
                bookingPassengersMapping.Add(new BookingPassenger
                {
                    BookingTripId = BookingTrip.Id,
                    FullName = BookingPassengerDto.FullName,
                    Age = BookingPassengerDto.Age,
                    PhoneNumber = BookingPassengerDto.PhoneNumber
                });
            }
            await _bookingPassengerRepositoryAsync.AddRangeAsync(bookingPassengersMapping);
            return Result.Success("Adding Success");
        }
        public async Task<Result> AddBookingPassenger(AddBookingPassengerDto addBookingPassenger)
        {
            var BookingTrip = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                           .FirstOrDefaultAsync(b => b.Id == addBookingPassenger.BookingTripId);
            if (BookingTrip is null)
                return Result.NotFound($"Not Found Any BookingTrip With id : {addBookingPassenger.BookingTripId}");
          
            if (BookingTrip.BookingStatus != BookingStatus.Completed)
            {
                return Result.BadRequest("Cann't Add New Passenger For Booking Not Completed");
            }
            var BookingPassenger = await _bookingPassengerRepositoryAsync.GetTableAsTracking()
                                                                         .Where(x => x.BookingTripId == BookingTrip.Id)
                                                                         .ToListAsync();

            if(BookingTrip.PassengerCount == BookingPassenger.Count())
            {
                return Result.BadRequest("Count of Passengers For Booking is equal to the Number of Passengers");
            }
            if (BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Draft)
            {
                return Result.BadRequest($"Cann't Add new Passenger for {BookingTrip.PackageTripDate.Status} Trip");
            }
            var bookingPassengerMapping = new BookingPassenger
            {
                Age = addBookingPassenger.Age,
                BookingTripId = addBookingPassenger.BookingTripId,
                FullName = addBookingPassenger.FullName,
                PhoneNumber = addBookingPassenger.PhoneNumber
            };
            await _bookingPassengerRepositoryAsync.AddAsync(bookingPassengerMapping);
            return Result.Success("Adding Success");
        }
        public async Task<Result> UpdateBookingPassenger(UpdateBookingPassengerDto updateBookingPassengers)
        {
            var BookingPassenger = await _bookingPassengerRepositoryAsync.GetByIdAsync(updateBookingPassengers.Id);
            if (BookingPassenger is null)         
                return Result.NotFound($"Not Found BookingPassenger With Id :{updateBookingPassengers.Id}");
            var BookingTrip = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                                 .FirstOrDefaultAsync(x => x.Id == BookingPassenger.BookingTripId);
           
            if (BookingTrip!.BookingStatus != BookingStatus.Completed)
            {
                return Result.BadRequest("Cann't Update information for Passenger for Not Completed Booking");
            }

            if (BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Draft
                )
            {
                return Result.BadRequest($"Cann't Update information For Passenger for {BookingTrip.PackageTripDate.Status} Trip");
            }

            BookingPassenger.PhoneNumber = updateBookingPassengers.PhoneNumber;
            BookingPassenger.FullName= updateBookingPassengers.FullName;
            BookingPassenger.Age = updateBookingPassengers.Age;
            await _bookingPassengerRepositoryAsync.SaveChangesAsync();
            return Result.Success("Update Success");
        }
        public async Task<Result> DeleteBookingPassenger(int BookingPassengerId)
        {
            var BookingPassenger = await _bookingPassengerRepositoryAsync.GetByIdAsync(BookingPassengerId);
            if (BookingPassenger is null)         
                return Result.NotFound($"Not Found BookingPassenger With Id :{BookingPassengerId}");

            var BookingTrip = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                               .Where(x => x.Id == BookingPassenger.BookingTripId)
                                                               .Include(p=>p.PackageTripDate)
                                                               .FirstOrDefaultAsync();

            if (BookingTrip!.BookingStatus != BookingStatus.Completed)
            {
                return Result.BadRequest("Cann't Delete Passenger for Not Completed Booking");
            }
            if (BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled ||
                BookingTrip.PackageTripDate.Status == PackageTripDateStatus.Draft)
            {
                return Result.BadRequest($"Cann't Delete Passenger for {BookingTrip.PackageTripDate.Status} Trip");
            }

            await _bookingPassengerRepositoryAsync.DeleteAsync(BookingPassenger);
            return Result.Success("Delete Success");
        }
      
    }
}
